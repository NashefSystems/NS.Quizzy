export enum UserRoles {
    STUDENT = "Student",
    TEACHER = "Teacher",
    ADMIN = "Admin",
    DEVELOPER = "Developer",
    SUPERADMIN = "SuperAdmin",
}

export class UserDetailsDto {
    id: string;
    fullName: string;
    idNumber?: string;
    email: string;
    role: UserRoles;
    classId!: string;
    pushNotificationIsEnabled: boolean
}