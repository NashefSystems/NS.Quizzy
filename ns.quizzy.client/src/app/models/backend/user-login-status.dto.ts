export interface IUserLoginStatusDto {
    idNumber: string;
    fullName: string;
    role: string;
    class?: number | null;
    lastLogin?: Date | null;
    isAllowNotifications?: string | null;
}