import { IBaseEntityDto } from './base-entity.dto';

export enum NotificationTarget {
    SPECIFIC_USERS = "SpecificUsers",
    CLASSES = "Classes",
    GRADES = "Grades",
    STUDENTS = "Students",
    TEACHERS = "Teachers",
    TEACHERS_AND_STUDENTS = "TeachersAndStudents",
    ALL_USERS = "AllUsers"
}

export interface INotificationBasePayloadDto {
    title: string;
    body: string;
    data?: { [key: string]: string };
}

export interface INotificationPayloadDto extends INotificationBasePayloadDto {
    target: NotificationTarget;
    targetIds?: string[];
}

export interface INotificationDto extends INotificationPayloadDto, IBaseEntityDto {
    createdTime: Date;
    totalUsers?: number;
    totalSeen?: number;
    seeingPercentage?: number;
    numberOfPushNotificationsReceived?: number;
    pushNotificationReceivedPercentage?: number;
    read: boolean;
}

export interface IMyNotificationItem extends INotificationBasePayloadDto, IBaseEntityDto {
    createdTime: Date;
    author: string;
    read: boolean;
}