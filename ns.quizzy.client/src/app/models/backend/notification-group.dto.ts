import { IBaseEntityDto } from './base-entity.dto';

export interface INotificationGroupPayloadDto {
    name: string;
    userIds: string[];
}

export interface INotificationGroupDto extends INotificationGroupPayloadDto, IBaseEntityDto { }