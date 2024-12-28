import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthenticatUserService } from '../services/authenticat-user.service';

export const adminUserGuard: CanActivateFn = (route, state) => {
  const authenticatUserService = inject(AuthenticatUserService);

  return !!(authenticatUserService.getUser()?.id);
};
