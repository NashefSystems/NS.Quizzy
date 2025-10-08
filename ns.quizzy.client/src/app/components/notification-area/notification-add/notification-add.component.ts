import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NotificationsService } from '../../../services/notifications.service';
import { NotificationsService as BackendNotificationsService } from '../../../services/backend/notifications.service';
import { Router } from '@angular/router';
import { INotificationPayloadDto, NotificationTarget } from '../../../models/backend/notification.dto';
import { IUserDto } from '../../../models/backend/user.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { ClassesService } from '../../../services/backend/classes.service';
import { GradesService } from '../../../services/backend/grades.service';
import { UsersService } from '../../../services/backend/users.service';
import { AccountService } from '../../../services/backend/account.service';
import { UserRoles } from '../../../enums/user-roles.enum';
import { AppTranslateService } from '../../../services/app-translate.service';
import { forkJoin } from 'rxjs';

export interface IUserDtoExt extends IUserDto {
  text: string;
}

@Component({
  selector: 'app-notification-add',
  standalone: false,
  templateUrl: './notification-add.component.html',
  styleUrl: './notification-add.component.scss'
})
export class NotificationAddComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _classesService = inject(ClassesService);
  private readonly _gradesService = inject(GradesService);
  private readonly _usersService = inject(UsersService);
  private readonly _accountService = inject(AccountService);
  private readonly _backendNotificationsService = inject(BackendNotificationsService);
  private readonly _router = inject(Router);
  private readonly _appTranslateService = inject(AppTranslateService);

  id: string | null = null;
  targets: IUserDtoExt[] | IGradeDto[] | IClassDto = [];
  users: IUserDtoExt[] = [];
  filteredUsers: IUserDtoExt[] = [];
  grades: IGradeDto[] = [];
  classes: IClassDto[] = [];
  pushNotificationIsEnabled: boolean;
  myUserId: string;

  targetIdsSearchCtrl = new FormControl('');

  form: FormGroup = this._fb.group({
    title: ['', [Validators.required]],
    body: ['', [Validators.required]],
    target: ['', [Validators.required]],
    targetIds: ['[]'],
  });

  ngOnInit(): void {
    forkJoin([
      this._usersService.get(),
      this._gradesService.get(),
      this._classesService.get(),
    ]).subscribe(([users, grades, classes]) => {
      this.grades = grades;
      this.classes = classes;

      const usersEx = users.map(u => {
        return {
          ...u,
          text: `${u.fullName} (${this.getUserInfo(u)})`
        };
      })
      this.users = [...usersEx];
      this.filteredUsers = [...usersEx];
    });

    this._accountService.userChange.subscribe(x => {
      this.pushNotificationIsEnabled = x?.pushNotificationIsEnabled ?? false;
      this.myUserId = x?.id ?? '';
    });

    this.targetIdsSearchCtrl.valueChanges
      .subscribe((search: string | null) => {
        if (!search?.trim()) {
          this.filteredUsers = [...this.users]
        } else {
          this.filteredUsers = this.users.filter(u =>
            u.text.toLowerCase().includes(search.toLowerCase())
          );
        }
      });
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { title, body, target, targetIds } = this.form.value;

    const payload: INotificationPayloadDto = {
      title: title,
      body: body,
      target: target,
      targetIds: targetIds,
    };

    this._backendNotificationsService.insert(payload).subscribe({
      next: responseBody => {
        this._notificationsService.success('ITEM_ADDED_SUCCESSFULLY');
        this.navigateToList();
      },
      error: err => {
        this._notificationsService.httpErrorHandler(err);
      }
    });
  }

  navigateToList() {
    this._router.navigate(['/notifications']);
  }

  onTargetChange() {
    this.form.patchValue({ targetIds: [] });
  }

  isSpecificUsers() {
    const { target } = this.form.value;
    return target === NotificationTarget.SPECIFIC_USERS
  }

  isClasses() {
    const { target } = this.form.value;
    return target === NotificationTarget.CLASSES
  }

  isGrades() {
    const { target } = this.form.value;
    return target === NotificationTarget.GRADES
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
      target: NotificationTarget.SPECIFIC_USERS,
      targetIds: [this.myUserId],
      data: {
        isTest: 'true',
        source: '/notifications/new'
      }
    };

    this._backendNotificationsService.insert(payload).subscribe({
      next: responseBody => {
        this._notificationsService.success('NOTIFICATION_AREA.PUSH_NOTIFICATION_SENT_SUCCESSFULLY');
      },
      error: err => {
        this._notificationsService.httpErrorHandler(err);
      }
    });
  }
}
