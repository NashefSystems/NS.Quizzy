import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/backend/account.service';

export const adminUserGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  return accountService.isAuthenticatedUser();
};
