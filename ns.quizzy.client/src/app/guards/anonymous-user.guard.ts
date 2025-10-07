import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';
import { AppSettingsService } from '../services/app-settings.service';
import { CheckPermissionsUtils } from '../utils/check-permissions.utils';

export const anonymousUserGuard: CanActivateFn = async (route, state) => {
  const appSettingsService = inject(AppSettingsService);
  const accountService = inject(AccountService);
  const router = inject(Router);
  let res = true;
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    res = CheckPermissionsUtils.isAnonymousUser(userDetailsDto);
  } catch (error) {
    console.error('💂🏿 anonymousUserGuard error:', error);
  }

  if (!res) {
    router.navigate([appSettingsService.homeUrl]);
  }
  console.log(`💂🏿 anonymousUserGuard => ${res}`);
  return res;
};
