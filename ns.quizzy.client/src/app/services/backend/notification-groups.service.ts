import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { INotificationGroupDto, INotificationGroupPayloadDto } from '../../models/backend/notification-group.dto';

@Injectable({
  providedIn: 'root'
})
export class NotificationGroupsService extends EntityBaseService<INotificationGroupPayloadDto, INotificationGroupDto> {
  controllerName: string = 'NotificationGroups';
}
