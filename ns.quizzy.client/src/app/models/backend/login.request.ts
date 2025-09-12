export class LoginRequest {
    email: string;
    password: string;
    deviceId?: string | null;
    notificationToken?: string | null;
    appVersion?: string | null;
}