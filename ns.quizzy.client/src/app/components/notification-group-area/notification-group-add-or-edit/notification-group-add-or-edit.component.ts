import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NotificationGroupsService } from '../../../services/backend/notification-groups.service';
import { AppNotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { INotificationGroupPayloadDto } from '../../../models/backend/notification-group.dto';
import { UsersService } from '../../../services/backend/users.service';
import { IUserDto, IUserDtoExt } from '../../../models/backend/user.dto';
import { forkJoin } from 'rxjs';
import { UserRoles } from '../../../enums/user-roles.enum';
import { AppTranslateService } from '../../../services/app-translate.service';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';

@Component({
  standalone: false,
  selector: 'app-notification-group-add-or-edit',
  templateUrl: './notification-group-add-or-edit.component.html',
  styleUrl: './notification-group-add-or-edit.component.scss'
})
export class NotificationGroupAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _notificationGroupsService = inject(NotificationGroupsService);
  private readonly _usersService = inject(UsersService);
  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _classesService = inject(ClassesService);
  private readonly _notificationsService = inject(AppNotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;
  classes: IClassDto[] = [];
  users: IUserDtoExt[] = [];
  filteredUsers: IUserDtoExt[] = [];
  userIdsSearchCtrls: FormControl = new FormControl('');

  form: FormGroup = this._fb.group({
    name: ['', [Validators.required]],
    userIds: ['[]', [Validators.required]],
  });

  ngOnInit(): void {
    this.userIdsSearchCtrls.valueChanges.subscribe((search: string | null) => this.specificUserSearch(search));
    forkJoin([
      this._usersService.get(),
      this._classesService.get(),
    ]).subscribe(([users, classes]) => {
      this.classes = classes;
      const usersEx = users.map(u => {
        return {
          ...u,
          text: `${u.fullName} (${this.getUserInfo(u)})`
        };
      })
      this.users = [...usersEx];
      this.specificUserSearch(null);

      this._activatedRoute.paramMap.subscribe(params => {
        this.id = params.get('id');
        if (this.id) {
          this._notificationGroupsService.getById(this.id).subscribe({
            next: data => {
              const { name, userIds } = data;
              const newValue = {
                ...this.form.value,
                name, userIds
              };
              this.form.setValue(newValue);
            }
          });
        }
      });
    });
  }

  specificUserSearch(search: string | null) {
    if (!search?.trim()) {
      this.filteredUsers = [...this.users];
    } else {
      this.filteredUsers = this.users.filter(u =>
        u.text.toLowerCase().includes(search.toLowerCase())
      );
    }
  }

  getUserInfo(item: IUserDto): string {
    if (item.role === UserRoles.TEACHER) {
      return this._appTranslateService.translate('USER_ROLES.TEACHER');
    }
    const classFullCode = this.classes.find(x => x.id === item.classId)?.fullCode;
    return classFullCode?.toString() || '';
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { name, userIds } = this.form.value;
    const payload: INotificationGroupPayloadDto = {
      name,
      userIds
    };

    if (this.id) {
      this._notificationGroupsService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._notificationGroupsService.insert(payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_ADDED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    }
  }

  navigateToList() {
    this._router.navigate(['/notification-groups']);
  }
}
