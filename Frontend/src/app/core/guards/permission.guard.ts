import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { PermissionService } from '../services/permission.service';
import { Permission } from '../constants/permission.constants';
import { APP_CONSTANTS } from '../constants/app.constants';

export function requirePermission(permission: Permission): CanActivateFn {
  return () => {
    const permissionService = inject(PermissionService);
    const router = inject(Router);

    if (!permissionService.loaded()) {
      return true;
    }

    if (permissionService.hasPermission(permission)) {
      return true;
    }

    router.navigate([APP_CONSTANTS.ROUTES.DASHBOARD]);
    return false;
  };
}
