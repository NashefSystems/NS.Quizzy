import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';
import { AppSettingsService } from '../services/app-settings.service';
import { CheckPermissionsUtils } from '../utils/check-permissions.utils';

export const authenticatedUserGuard: CanActivateFn = async (route, state) => {
  const appSettingsService = inject(AppSettingsService);
  const accountService = inject(AccountService);
  const router = inject(Router);
  let res = false;
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    res = CheckPermissionsUtils.isAuthenticatedUser(userDetailsDto);
  } catch (error) {
    console.error('💂🏿 authenticatedUserGuard error:', error);
  }

  if (!res) {
    router.navigate([appSettingsService.loginUrl]);
  }
  console.log(`💂🏿 authenticatedUserGuard => ${res}`);
  return res;
};
