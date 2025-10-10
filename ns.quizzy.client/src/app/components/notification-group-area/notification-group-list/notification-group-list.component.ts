import { Component, inject, OnInit } from '@angular/core';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { NotificationGroupsService } from '../../../services/backend/notification-groups.service';
import { UsersService } from '../../../services/backend/users.service';
import { INotificationGroupDto } from '../../../models/backend/notification-group.dto';
import { IUserDto } from '../../../models/backend/user.dto';
import { forkJoin } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-notification-group-list',
  templateUrl: './notification-group-list.component.html',
  styleUrl: './notification-group-list.component.scss'
})
export class NotificationGroupListComponent implements OnInit {
  private readonly _notificationGroupsService = inject(NotificationGroupsService);
  private readonly _userService = inject(UsersService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  users: IUserDto[] = [];
  items: INotificationGroupDto[] = [];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'QUESTIONNAIRE_AREA.NAME'
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    forkJoin([
      this._notificationGroupsService.get(),
      this._userService.get(),
    ]).subscribe(([notificationGroups, users]) => {
      this.users = users;
      this.items = notificationGroups;
    });
  }

  onAdd() {
    this._router.navigate(['/notification-groups/new']);
  };

  onEdit(item: INotificationGroupDto) {
    this._router.navigate([`/notification-groups/edit/${item?.id}`]);
  };

  onDelete(selected: INotificationGroupDto) {
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
          this._notificationGroupsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
