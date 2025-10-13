import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { INotificationTemplateDto, INotificationTemplatePayloadDto } from '../../models/backend/notification-template.dto';

@Injectable({
  providedIn: 'root'
})
export class NotificationTemplatesService extends EntityBaseService<INotificationTemplatePayloadDto, INotificationTemplateDto> {
  controllerName: string = 'NotificationTemplates';
}
