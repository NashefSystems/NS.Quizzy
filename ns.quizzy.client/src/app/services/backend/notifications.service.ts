import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IMyNotificationItem, INotificationDto, INotificationPayloadDto } from '../../models/backend/notification.dto';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService extends EntityBaseService<INotificationPayloadDto, INotificationDto> {
  controllerName: string = 'Notifications';

  getNumberOfMyNewNotifications() {
    return this.httpClient.get<number>(`${this.getBaseUrl()}/NumberOfMyNewNotifications`);
  }

  getMyNotifications(limit: number | null = null) {
    let url = `${this.getBaseUrl()}/MyNotifications`;
    if (limit && limit > 0) {
      url += `?limit=${limit}`;
    }
    return this.httpClient.get<IMyNotificationItem[]>(url);
  }

  markAsRead(notificationId: string) {
    return this.httpClient.put<IMyNotificationItem>(`${this.getBaseUrl()}/${notificationId}/read`, null);
  }
}
