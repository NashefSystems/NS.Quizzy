import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';
import { NotificationsService } from '../services/notifications.service';
import { AppSettingsService } from '../services/app-settings.service';
import { CheckPermissionsUtils } from '../utils/check-permissions.utils';

export const adminUserGuard: CanActivateFn = async (route, state) => {
  const appSettingsService = inject(AppSettingsService);
  const accountService = inject(AccountService);
  const router = inject(Router);
  const notificationsService = inject(NotificationsService);

  let isAuthenticated = false;
  let res = false;
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    isAuthenticated = !!(userDetailsDto?.id);
    res = CheckPermissionsUtils.isAdminUser(userDetailsDto);
  } catch (error) {
    console.error('💂🏿 adminUserGuard error:', error);
  }

  if (!isAuthenticated) {
    router.navigate([appSettingsService.loginUrl]);
  }

  if (!res) {
    notificationsService.error("ERRORS.FORBID");
    router.navigate([appSettingsService.homeUrl]);
  }
  console.log(`💂🏿 adminUserGuard => ${res}`);
  return res;
};
