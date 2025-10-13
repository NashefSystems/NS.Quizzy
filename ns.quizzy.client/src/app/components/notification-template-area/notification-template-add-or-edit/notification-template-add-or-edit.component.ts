import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NotificationTemplatesService } from '../../../services/backend/notification-templates.service';
import { AppNotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { INotificationTemplatePayloadDto } from '../../../models/backend/notification-template.dto';
import { UsersService } from '../../../services/backend/users.service';
import { IUserDto, IUserDtoExt } from '../../../models/backend/user.dto';
import { forkJoin } from 'rxjs';
import { UserRoles } from '../../../enums/user-roles.enum';
import { AppTranslateService } from '../../../services/app-translate.service';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';

@Component({
  standalone: false,
  selector: 'app-notification-template-add-or-edit',
  templateUrl: './notification-template-add-or-edit.component.html',
  styleUrl: './notification-template-add-or-edit.component.scss'
})
export class NotificationTemplateAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _notificationTemplatesService = inject(NotificationTemplatesService);
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
    title: ['', [Validators.required]],
    body: ['', [Validators.required]],
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
          this._notificationTemplatesService.getById(this.id).subscribe({
            next: data => {
              const { name, title, body } = data;
              const newValue = {
                ...this.form.value,
                name, title, body
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
    const { name, title, body } = this.form.value;
    const payload: INotificationTemplatePayloadDto = {
      name,
      title,
      body
    };

    if (this.id) {
      this._notificationTemplatesService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._notificationTemplatesService.insert(payload).subscribe({
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
    this._router.navigate(['/notification-templates']);
  }
}
