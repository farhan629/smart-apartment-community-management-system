import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { OVERLAY_DEFAULT_CONFIG } from '@angular/cdk/overlay';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { lastValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { loaderInterceptor } from './core/interceptors/loader.interceptor';
import { AuthService } from './core/services/auth-service';
import { PermissionService } from './core/services/permission.service';
import { AUTH_STORAGE_KEYS } from './core/constants/auth.constants';
import { ThemeService } from './core/services/theme.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([authInterceptor, loaderInterceptor])),
    provideNativeDateAdapter(),
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
    { provide: OVERLAY_DEFAULT_CONFIG, useValue: { usePopover: false } },
    provideAppInitializer(async () => {
      inject(ThemeService);

      const authService = inject(AuthService);
      const permissionService = inject(PermissionService);

      if (authService.getAccessToken()) {
        await lastValueFrom(permissionService.load()).catch(() => {});
        return;
      }

      try {
        const response = await lastValueFrom(authService.refreshToken());
        if (response?.token) {
          sessionStorage.setItem(AUTH_STORAGE_KEYS.SESSION_TOKEN, response.token);
          authService.isLoggedIn.set(true);
          await lastValueFrom(permissionService.load()).catch(() => {});
        }
      } catch {
        // No refresh token available — user will be redirected to login by authGuard
      }
    }),
  ],
};
