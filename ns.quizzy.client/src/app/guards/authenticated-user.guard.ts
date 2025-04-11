import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';

export const authenticatedUserGuard: CanActivateFn = async (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  let res = false;
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    res = !!(userDetailsDto?.id);
  } catch (error) {
    console.error('authenticatedUserGuard error:', error);
  }

  if (!res) {
    router.navigate(['/login']);
  }

  return res;
};
