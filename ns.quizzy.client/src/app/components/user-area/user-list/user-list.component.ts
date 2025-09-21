import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { UsersService } from '../../../services/backend/users.service';
import { IUserDto } from '../../../models/backend/user.dto';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';
import { AppTranslateService } from '../../../services/app-translate.service';
import { NotificationsService } from '../../../services/notifications.service';

@Component({
  selector: 'app-user-list',
  standalone: false,

  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private readonly _usersService = inject(UsersService);
  private readonly _classesService = inject(ClassesService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);
  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _notificationsService = inject(NotificationsService);

  @ViewChild('fileInput') fileInput!: ElementRef;
  selectedFile: File | null = null;

  uploadFileProgressPercentage: number | null = null;
  classes: IClassDto[] = [];
  items: IUserDto[] | null = null;
  columns: TableColumnInfo[] = [
    {
      key: 'idNumber',
      title: 'USER_AREA.ID_NUMBER'
    }, {
      key: 'fullName',
      title: 'USER_AREA.FULL_NAME'
    },
    {
      key: 'role',
      title: 'USER_AREA.ROLE',
      converter: (item: IUserDto) => {
        const role = item?.role;
        if (!role) {
          return '';
        }
        return this._appTranslateService.translate(`USER_ROLES.${role.toUpperCase()}`);
      }
    },
    {
      key: 'classId',
      title: 'USER_AREA.CLASS',
      converter: (item: IUserDto) => {
        const parent = this.classes.find(x => x.id === item.classId);
        return parent?.name;
      }
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.items = null;
    this._classesService.get().subscribe({ next: (data) => this.classes = data });
    this._usersService.get().subscribe({ next: (data) => this.items = data });
  }

  onAdd() {
    this._router.navigate(['/users/new']);
  };

  onEdit(item: IUserDto) {
    this._router.navigate([`/users/edit/${item?.id}`]);
  };

  onDelete(selected: IUserDto) {
    const item = this.items?.find(x => x.id == selected.id);
    if (!item) {
      return;
    }

    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.fullName
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._usersService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };

  onExport() {
    window.location.href = '/api/users/download';
  }

  onImport() {
    this.fileInput.nativeElement.click();
  }

  checkUploadFileIsCompleted(messageId: string) {
    setTimeout(() => {

      this._usersService.uploadFileStatus(messageId).subscribe({
        next: (response) => {
          if (response.isCompleted) {
            this.uploadFileProgressPercentage = null;
            this.loadData();
          } else {
            this.uploadFileProgressPercentage = response?.progressPercentage ?? 0;
            this.checkUploadFileIsCompleted(messageId);
          }
        },
        error: (err) => {
          console.log("checkUploadFileIsCompleted error: ", err);
          this.uploadFileProgressPercentage = null;
          this.loadData();
        }
      });

    }, 1000);
  }

  onFileSelected(event: any) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const selectedFile = input.files[0];

      if (!selectedFile) {
        return;
      }

      this._usersService.upload(selectedFile).subscribe({
        next: (response) => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.items = null;
          this.checkUploadFileIsCompleted(response.messageId);
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });

      input.value = '';
    }
  }
}
