export class LoginWithIdNumberRequest {
    idNumber: string;
    deviceId?: string | null;
    appVersion?: string | null;
    notificationToken?: string | null;
}