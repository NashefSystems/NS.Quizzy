import { Component, inject, OnInit } from '@angular/core';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { NotificationTemplatesService } from '../../../services/backend/notification-templates.service';
import { UsersService } from '../../../services/backend/users.service';
import { INotificationTemplateDto } from '../../../models/backend/notification-template.dto';
import { IUserDto } from '../../../models/backend/user.dto';
import { forkJoin } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-notification-template-list',
  templateUrl: './notification-template-list.component.html',
  styleUrl: './notification-template-list.component.scss'
})
export class NotificationTemplateListComponent implements OnInit {
  private readonly _notificationTemplatesService = inject(NotificationTemplatesService);
  private readonly _userService = inject(UsersService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  users: IUserDto[] = [];
  items: INotificationTemplateDto[] = [];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'NOTIFICATION_TEMPLATE_AREA.NAME'
    },
    {
      key: 'title',
      title: 'NOTIFICATION_TEMPLATE_AREA.TITLE'
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    forkJoin([
      this._notificationTemplatesService.get(),
      this._userService.get(),
    ]).subscribe(([notificationTemplates, users]) => {
      this.users = users;
      this.items = notificationTemplates;
    });
  }

  onAdd() {
    this._router.navigate(['/notification-templates/new']);
  };

  onEdit(item: INotificationTemplateDto) {
    this._router.navigate([`/notification-templates/edit/${item?.id}`]);
  };

  onDelete(selected: INotificationTemplateDto) {
    const item = this.items.find(x => x.id == selected.id);
    if (!item) {
      return;
    }

    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.name
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._notificationTemplatesService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
