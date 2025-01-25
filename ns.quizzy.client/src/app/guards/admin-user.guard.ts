import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/backend/account.service';
import { firstValueFrom } from 'rxjs';

export const adminUserGuard: CanActivateFn = async (route, state) => {
  const accountService = inject(AccountService);
  try {
    const userDetailsDto = await firstValueFrom(accountService.getDetails());
    return !!(userDetailsDto?.id);
  } catch (error) {
    console.error('adminUserGuard error:', error);
    return false;
  }
};
