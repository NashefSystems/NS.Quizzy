import { IBaseEntityDto } from './base-entity.dto';

export interface INotificationTemplatePayloadDto {
    name: string;
    title: string;
    body: string;
}

export interface INotificationTemplateDto extends INotificationTemplatePayloadDto, IBaseEntityDto { }