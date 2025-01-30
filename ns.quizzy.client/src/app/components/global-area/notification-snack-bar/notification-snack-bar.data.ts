export declare type NotificationSnackBarType = 'info' | 'success' | 'warning' | 'error' | 'fatal';

export interface INotificationSnackBarData {
    message: string;
    type: NotificationSnackBarType;
}