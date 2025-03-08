import { inject, Injectable } from '@angular/core';
import { SecurityUtils } from '../utils/security.utils';
import { LocalStorageKeys } from '../enums/local-storage-keys.enum';
import { UserDetailsDto, UserRoles } from '../models/backend/user-details.dto';
import { AccountService } from './backend/account.service';


@Injectable({
  providedIn: 'root'
})
export class UserRolesService {
  checkRoleRequirement(userDetails: UserDetailsDto | null, roleRequirement: UserRoles): boolean {
    if (!userDetails) {
      return false;
    }

    const roleRequirementId = this.getRoleId(roleRequirement);
    const userRoleId = this.getRoleId(userDetails.role);

    return userRoleId >= roleRequirementId;
  }

  getRoleId(role: UserRoles): number {
    switch (role) {
      case UserRoles.STUDENT:
        return 1;
      case UserRoles.TEACHER:
        return 2;
      case UserRoles.ADMIN:
        return 3;
      case UserRoles.DEVELOPER:
        return 4;
      case UserRoles.SUPERADMIN:
        return 5;
      default: return 0;
    }
  }
}
