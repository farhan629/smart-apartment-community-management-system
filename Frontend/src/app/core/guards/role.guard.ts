import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { APP_CONSTANTS, Role } from '../constants/app.constants';

export function requireRole(...roles: Role[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (roles.includes(authService.getUserRole() as Role)) {
      return true;
    }

    router.navigate([APP_CONSTANTS.ROUTES.DASHBOARD]);
    return false;
  };
}
