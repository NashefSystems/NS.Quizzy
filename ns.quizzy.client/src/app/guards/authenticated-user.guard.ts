import { CanActivateFn } from '@angular/router';

export const authenticatedUserGuard: CanActivateFn = (route, state) => {
  return true;
};
