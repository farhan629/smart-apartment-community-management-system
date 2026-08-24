import {
  HttpInterceptorFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';

import { AuthService } from '../services/auth-service';
import { APP_CONSTANTS } from '../constants/app.constants';
import { AUTH_ENDPOINTS, AUTH_STORAGE_KEYS } from '../constants/auth.constants';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.getAccessToken();

  let authorizedReq = req;
  if (token) {
    authorizedReq = req.clone({
      setHeaders: { Authorization: `${APP_CONSTANTS.HTTP.AUTH_HEADER_PREFIX}${token}` },
    });
  }

  return next(authorizedReq).pipe(
    catchError((error) => {
      if (
        !(error instanceof HttpErrorResponse)
        || error.status !== 401
        || !token
        || req.url.includes(AUTH_ENDPOINTS.REFRESH_TOKEN)
      ) {
        return throwError(() => error);
      }

      if (!isRefreshing) {
        isRefreshing = true;
        refreshTokenSubject.next(null);

        return authService.refreshToken().pipe(
          switchMap((response) => {
            isRefreshing = false;

            if (!response?.token) {
              authService.clearSession();
              router.navigate(['/home']);
              return throwError(() => error);
            }

            sessionStorage.setItem(AUTH_STORAGE_KEYS.SESSION_TOKEN, response.token);

            refreshTokenSubject.next(response.token);

            const retryReq = req.clone({
              setHeaders: {
                Authorization: `${APP_CONSTANTS.HTTP.AUTH_HEADER_PREFIX}${response.token}`,
              },
            });
            return next(retryReq);
          }),
          catchError((refreshError) => {
            isRefreshing = false;
            refreshTokenSubject.next(null);
            authService.clearSession();
            router.navigate(['/home']);
            return throwError(() => refreshError);
          }),
        );
      }

      return refreshTokenSubject.pipe(
        filter((newToken): newToken is string => newToken !== null),
        take(1),
        switchMap((newToken) => {
          const retryReq = req.clone({
            setHeaders: {
              Authorization: `${APP_CONSTANTS.HTTP.AUTH_HEADER_PREFIX}${newToken}`,
            },
          });
          return next(retryReq);
        }),
      );
    }),
  );
};