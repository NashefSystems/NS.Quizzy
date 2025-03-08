import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';
import { UserRoles } from '../models/backend/user-details.dto';
import { NotificationsService } from '../services/notifications.service';

export const developerGuard: CanActivateFn = async (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const notificationsService = inject(NotificationsService);

  let isAuthenticated = false;
  let res = false;
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    isAuthenticated = !!(userDetailsDto?.id);
    const allowRoles = [UserRoles.DEVELOPER, UserRoles.SUPERADMIN];
    res = !!(userDetailsDto?.id) && allowRoles.includes(userDetailsDto.role);
  } catch (error) {
    console.error('developerGuard error:', error);
  }

  if (!isAuthenticated) {
    router.navigate(['/exam-schedule'])
  }

  if (!res) {
    notificationsService.error("ERRORS.FORBID");
  }

  return res;
};
