import { UserDetailsDto, UserRoles } from "../models/backend/user-details.dto";

export class CheckPermissionsUtils {

    public static isAdminUser(userDetailsDto: UserDetailsDto | null) {
        const allowRoles = [UserRoles.ADMIN, UserRoles.DEVELOPER, UserRoles.SUPERADMIN];
        return !!(userDetailsDto?.id) && allowRoles.includes(userDetailsDto.role);
    }

    public static isAnonymousUser(userDetailsDto: UserDetailsDto | null) {
        return !(userDetailsDto?.id);
    }

    public static isAuthenticatedUser(userDetailsDto: UserDetailsDto | null) {
        return !!(userDetailsDto?.id);
    }

    public static isDeveloperUser(userDetailsDto: UserDetailsDto | null) {
        const allowRoles = [UserRoles.DEVELOPER, UserRoles.SUPERADMIN];
        return !!(userDetailsDto?.id) && allowRoles.includes(userDetailsDto.role);
    }
}