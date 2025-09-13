export class LoginWithIdNumberRequest {
    idNumber: string;
    platform?: string | null;
    deviceId?: string | null;
    appVersion?: string | null;
    notificationToken?: string | null;
}