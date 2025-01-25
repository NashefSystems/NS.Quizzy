export class LoginResponse {
    requiresTwoFactor: boolean;
    contextId!: string;
    twoFactorUrl!: string;
    twoFactorQrCode!: string;
    twoFactorSecretKey!: string;
}