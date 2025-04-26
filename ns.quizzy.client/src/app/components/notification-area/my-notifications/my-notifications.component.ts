import { Component, inject } from '@angular/core';
import { NotificationsService } from '../../../services/backend/notifications.service';
import { IMyNotificationItem } from '../../../models/backend/notification.dto';
import { AppSettingsService } from '../../../services/app-settings.service';

@Component({
  selector: 'app-my-notifications',
  standalone: false,
  templateUrl: './my-notifications.component.html',
  styleUrl: './my-notifications.component.scss'
})
export class MyNotificationsComponent {
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _appSettingsService = inject(AppSettingsService);
  notifications: IMyNotificationItem[] = [];

  ngOnInit() {
    this.fetchNotifications();
  }

  fetchNotifications() {
    this._notificationsService.getMyNotifications(100)
      .subscribe(data => {
        this.notifications = data;
      });
  }

  markAsRead(notification: IMyNotificationItem) {
    if (!notification.read) {
      this._notificationsService.markAsRead(notification.id).subscribe(() => {
        notification.read = true;
        this._appSettingsService.reCalculateUnReadNotifications("MyNotificationsComponent");
      });
    }
  }
}
