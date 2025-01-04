export class LoginResponse {
    requiresTwoFactor: boolean;
    requestId!: string;
    twoFactorUrl!: string;
    twoFactorQrCode!: string;
    twoFactorSecretKey!: string;
}