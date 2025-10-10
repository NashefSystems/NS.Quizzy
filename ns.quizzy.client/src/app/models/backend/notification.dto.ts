import { IBaseEntityDto } from './base-entity.dto';

export enum NotificationTargetTypes {
    SPECIFIC_USERS = "SpecificUsers",
    CLASSES = "Classes",
    GRADES = "Grades",
    NOTIFICATION_GROUPS = "NotificationGroups",
    STUDENTS = "Students",
    TEACHERS = "Teachers",
    TEACHERS_AND_STUDENTS = "TeachersAndStudents",
    ADMINS = "Admins",
    ALL_USERS = "AllUsers",
}

export interface INotificationBasePayloadDto {
    title: string;
    body: string;
    data?: { [key: string]: string };
}

export interface INotificationTarget {
    type: NotificationTargetTypes;
    ids?: string[];
}

export interface INotificationPayloadDto extends INotificationBasePayloadDto {
    targets: INotificationTarget[];
}

export interface INotificationDto extends INotificationPayloadDto, IBaseEntityDto {
    createdTime: Date;
    totalUsers?: number;
    totalRead?: number;
    readPercentage?: number;
    numberOfPushNotificationsReceived?: number;
    pushNotificationReceivedPercentage?: number;
    read: boolean;
}

export interface IMyNotificationItem extends INotificationBasePayloadDto, IBaseEntityDto {
    createdTime: Date;
    author: string;
    read: boolean;
}