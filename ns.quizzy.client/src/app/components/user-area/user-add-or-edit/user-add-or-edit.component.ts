import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../../../services/backend/users.service';
import { IUserPayloadDto } from '../../../models/backend/user.dto';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';
import { IClassDto } from '../../../models/backend/class.dto';
import { ClassesService } from '../../../services/backend/classes.service';
import { UserRoles } from '../../../models/backend/user-details.dto';

@Component({
  selector: 'app-user-add-or-edit',
  standalone: false,

  templateUrl: './user-add-or-edit.component.html',
  styleUrl: './user-add-or-edit.component.scss'
})
export class UserAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _usersService = inject(UsersService);
  private readonly _classesService = inject(ClassesService);
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  classes: IClassDto[] = [];

  private _idNumberEmailDomain: string;
  id: string | null = null;

  form: FormGroup = this._fb.group({
    idNumber: ['', [Validators.required]],
    fullName: ['', [Validators.required]],
    role: ['', [Validators.required]],
    classId: ['']
  });

  ngOnInit(): void {
    this._classesService.get().subscribe(classes => this.classes = classes);
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromId(this.id);
    });
    this._clientAppSettingsService.get().subscribe(data => this._idNumberEmailDomain = data?.IdNumberEmailDomain);
  }

  loadDataFromId(id: string | null) {
    if (!id) {
      return;
    }

    this._usersService.getById(id).subscribe({
      next: data => {
        const { email, fullName, role, classId } = data;
        const idNumber = this.getIdNumberFromEmail(email);
        const newValue = {
          ...this.form.value,
          idNumber, fullName, role, classId
        };
        this.form.setValue(newValue);
      }
    });
  }

  getEmailFromIdNumber(idNumber: string) {
    return `${idNumber}@${this._idNumberEmailDomain}`;
  }

  getIdNumberFromEmail(email: string) {
    return email.replace(`@${this._idNumberEmailDomain}`, '');
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { idNumber, fullName, role, classId } = this.form.value;
    const email = this.getEmailFromIdNumber(idNumber);
    const payload: IUserPayloadDto = {
      email: email,
      idNumber: idNumber,
      fullName: fullName,
      role: role,
      classId: role === UserRoles.STUDENT ? classId : null,
    };

    if (this.id) {
      this._usersService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._usersService.insert(payload).subscribe({
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
    this._router.navigate(['/users']);
  }
}
