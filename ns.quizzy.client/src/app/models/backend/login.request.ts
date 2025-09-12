export class LoginRequest {
    email: string;
    password: string;
    deviceId?: string | null;
    appVersion?: string | null;
    notificationToken?: string | null;
}