import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  private readonly _backendNotificationsService = inject(BackendNotificationsService);
  private readonly _router = inject(Router);
  
  id: string | null = null;
  targets: IUserDto[] | IGradeDto[] | IClassDto = [];
  users: IUserDto[];
  grades: IGradeDto[];
  classes: IClassDto[];

  form: FormGroup = this._fb.group({
    title: ['', [Validators.required]],
    body: ['', [Validators.required]],
    target: ['', [Validators.required]],
    targetIds: ['[]'],
  });

  ngOnInit(): void {
    this._usersService.get().subscribe(x => this.users = x);
    this._gradesService.get().subscribe(x => this.grades = x);
    this._classesService.get().subscribe(x => this.classes = x);
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
    const { title, body, target } = this.form.value;
    this.form.setValue({
      title,
      body,
      target,
      targetIds: []
    });
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
}
