import { UserRoles } from "../../enums/user-roles.enum";

export class UserDetailsDto {
    id: string;
    fullName: string;
    idNumber?: string;
    email: string;
    role: UserRoles;
    classId!: string;
    pushNotificationIsEnabled: boolean
}