import { Component, inject, OnInit } from '@angular/core';
import { NotificationsService } from '../../../services/backend/notifications.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { INotificationDto } from '../../../models/backend/notification.dto';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { AppTranslateService } from '../../../services/app-translate.service';

@Component({
  selector: 'app-notification-list',
  standalone: false,
  templateUrl: './notification-list.component.html',
  styleUrl: './notification-list.component.scss'
})
export class NotificationListComponent implements OnInit {
  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  items: INotificationDto[];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._notificationsService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    this._router.navigate(['/notifications/new']);
  };

  onDelete(selected: INotificationDto) {
    const item = this.items.find(x => x.id == selected.id);
    if (!item) {
      return;
    }

    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.title
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._notificationsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
