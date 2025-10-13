import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AppNotificationsService } from '../../../services/notifications.service';
import { NotificationsService } from '../../../services/backend/notifications.service';
import { Router } from '@angular/router';
import { INotificationPayloadDto, NotificationTargetTypes } from '../../../models/backend/notification.dto';
import { IUserDto, IUserDtoExt } from '../../../models/backend/user.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { ClassesService } from '../../../services/backend/classes.service';
import { GradesService } from '../../../services/backend/grades.service';
import { UsersService } from '../../../services/backend/users.service';
import { AccountService } from '../../../services/backend/account.service';
import { UserRoles } from '../../../enums/user-roles.enum';
import { AppTranslateService } from '../../../services/app-translate.service';
import { forkJoin } from 'rxjs';
import { NotificationGroupsService } from '../../../services/backend/notification-groups.service';
import { INotificationGroupDto } from '../../../models/backend/notification-group.dto';
import { NotificationTemplatesService } from '../../../services/backend/notification-templates.service';
import { INotificationTemplateDto } from '../../../models/backend/notification-template.dto';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { SelectNotificationTemplateDialogComponent } from '../../notification-template-area/select-notification-template-dialog/select-notification-template-dialog.component';

@Component({
  selector: 'app-notification-add',
  standalone: false,
  templateUrl: './notification-add.component.html',
  styleUrl: './notification-add.component.scss'
})
export class NotificationAddComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _appNotificationsService = inject(AppNotificationsService);
  private readonly _classesService = inject(ClassesService);
  private readonly _gradesService = inject(GradesService);
  private readonly _usersService = inject(UsersService);
  private readonly _accountService = inject(AccountService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _notificationTemplatesService = inject(NotificationTemplatesService);
  private readonly _notificationGroupsService = inject(NotificationGroupsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);
  private readonly _appTranslateService = inject(AppTranslateService);

  duplicateItemId: string | null = null;
  NotificationTargetTypes = NotificationTargetTypes;
  id: string | null = null;
  users: IUserDtoExt[] = [];
  filteredUsers: IUserDtoExt[][] = [];
  notificationTemplates: INotificationTemplateDto[] = [];
  notificationGroups: INotificationGroupDto[] = [];
  grades: IGradeDto[] = [];
  classes: IClassDto[] = [];
  pushNotificationIsEnabled: boolean;
  myUserId: string;

  targetIdsSearchCtrls: FormControl[] = [];

  form: FormGroup = this._fb.group({
    title: ['', [Validators.required]],
    body: ['', [Validators.required]],
    targets: this._fb.array([], [Validators.required, Validators.minLength(1)]),
  });

  constructor() {
    const navigation = this._router.getCurrentNavigation();
    if (navigation?.extras?.state) {
      this.duplicateItemId = navigation.extras.state['DuplicateItemId'];
    }
  }

  ngOnInit(): void {
    forkJoin([
      this._usersService.get(),
      this._gradesService.get(),
      this._classesService.get(),
      this._notificationGroupsService.get(),
      this._notificationTemplatesService.get(),
    ]).subscribe(([users, grades, classes, notificationGroups, notificationTemplates]) => {
      this.grades = grades;
      this.classes = classes;
      this.notificationGroups = notificationGroups;
      this.notificationTemplates = notificationTemplates;

      const usersEx = users.map(u => {
        return {
          ...u,
          text: `${u.fullName} (${this.getUserInfo(u)})`
        };
      })
      this.users = [...usersEx];
      this.targetIdsSearchCtrls.forEach((ctrl, index) => {
        this.specificUserSearch(ctrl.value, index);
      });
    });

    this._accountService.userChange.subscribe(x => {
      this.pushNotificationIsEnabled = x?.pushNotificationIsEnabled ?? false;
      this.myUserId = x?.id ?? '';
    });

    // Add initial target section
    this.addTarget();

    if (this.duplicateItemId) {
      this._notificationsService.getById(this.duplicateItemId).subscribe({
        next: duplicateItem => {
          if (duplicateItem.targets.length > 1) {
            for (let i = 1; i < duplicateItem.targets.length; i++) {
              this.addTarget();
            }
          }
          const newValue = {
            ...this.form.value,
            title: duplicateItem.title,
            body: duplicateItem.body,
            targets: duplicateItem.targets,
          };
          this.form.setValue(newValue);
        }
      });
    }
  }

  targetTypeOptions() {
    return Object.values(NotificationTargetTypes);
  }

  get targets(): FormArray {
    return this.form.get('targets') as FormArray;
  }

  addTarget(): void {
    const index = this.targets.length;
    const group = this._fb.group({
      type: ['', [Validators.required]],
      ids: [[]],
    });
    this.targets.push(group);
    this.filteredUsers.push([...this.users]);

    // Create search control for this section
    const searchCtrl = new FormControl('');
    this.targetIdsSearchCtrls.push(searchCtrl);

    // Subscribe to search changes
    searchCtrl.valueChanges.subscribe((search: string | null) => this.specificUserSearch(search, index));
  }

  specificUserSearch(search: string | null, index: number) {
    if (!search?.trim()) {
      this.filteredUsers[index] = [...this.users];
    } else {
      this.filteredUsers[index] = this.users.filter(u =>
        u.text.toLowerCase().includes(search.toLowerCase())
      );
    }
  }

  removeTarget(index: number): void {
    this.targets.removeAt(index);
    this.targetIdsSearchCtrls.splice(index, 1);
    this.filteredUsers.splice(index, 1);
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { title, body, targets } = this.form.value;

    const payload: INotificationPayloadDto = {
      title: title,
      body: body,
      targets: targets,
    };

    this._notificationsService.insert(payload).subscribe({
      next: responseBody => {
        this._appNotificationsService.success('ITEM_ADDED_SUCCESSFULLY');
        this.navigateToList();
      },
      error: err => {
        this._appNotificationsService.httpErrorHandler(err);
      }
    });
  }

  navigateToList() {
    this._router.navigate(['/notifications']);
  }

  onTargetChange(index: number) {
    const targetSection = this.targets.at(index);
    targetSection.patchValue({ targetIds: [] });
  }

  isSpecificUsers(index: number) {
    const target = this.targets.at(index);
    return target.get('type')?.value === NotificationTargetTypes.SPECIFIC_USERS;
  }

  isClasses(index: number) {
    const targetSection = this.targets.at(index);
    return targetSection.get('type')?.value === NotificationTargetTypes.CLASSES;
  }

  isGrades(index: number) {
    const targetSection = this.targets.at(index);
    return targetSection.get('type')?.value === NotificationTargetTypes.GRADES;
  }

  isNotificationGroups(index: number) {
    const targetSection = this.targets.at(index);
    return targetSection.get('type')?.value === NotificationTargetTypes.NOTIFICATION_GROUPS;
  }

  getUserInfo(item: IUserDto): string {
    if (item.role === UserRoles.TEACHER) {
      return this._appTranslateService.translate('USER_ROLES.TEACHER');
    }
    const classFullCode = this.classes.find(x => x.id === item.classId)?.fullCode;
    return classFullCode?.toString() || '';
  }

  onTest() {
    if (!this.form.valid) {
      return;
    }
    const { title, body } = this.form.value;

    const payload: INotificationPayloadDto = {
      title: title,
      body: body,
      targets: [{
        type: NotificationTargetTypes.SPECIFIC_USERS,
        ids: [this.myUserId]
      }],
      data: {
        isTest: 'true',
        source: '/notifications/new'
      }
    };

    this._notificationsService.insert(payload).subscribe({
      next: responseBody => {
        this._appNotificationsService.success('NOTIFICATION_AREA.PUSH_NOTIFICATION_SENT_SUCCESSFULLY');
      },
      error: err => {
        this._appNotificationsService.httpErrorHandler(err);
      }
    });
  }

  onLoadFromTemplate() {
    const dialogPayload: OpenDialogPayload = {
      component: SelectNotificationTemplateDialogComponent,
      isFullDialog: false,
      data: [...this.notificationTemplates]
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(selectedItem => {
        if (selectedItem) {
          const { title, body } = selectedItem as INotificationTemplateDto;
          this.form.setValue({
            ...this.form.value,
            title, body
          });
        }
      });
  }
}
