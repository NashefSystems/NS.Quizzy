export class LoginRequest {
    email: string;
    password: string;
    platform?: string | null;
    deviceId?: string | null;
    appVersion?: string | null;
    notificationToken?: string | null;
}