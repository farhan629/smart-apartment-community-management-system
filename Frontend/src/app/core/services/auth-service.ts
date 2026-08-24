import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, switchMap, of, map, timeout, catchError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AUTH_ENDPOINTS, AUTH_STORAGE_KEYS } from '../constants/auth.constants';
import { APP_CONSTANTS } from '../constants/app.constants';
import {
  LoginRequestDto,
  LoginResponseDto,
  RegisterRequestDto,
  SuccessResponseDto,
  RefreshTokenResponseDto,
  PermissionResponseDto,
  ForgotPasswordRequestDto,
  VerifyOtpDto,
  VerifyOtpResponseDto,
  ResetPasswordDto,
  ChangePasswordDto,
} from '../models/auth.models';
import { PermissionService } from './permission.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly permissionService = inject(PermissionService);
  private readonly baseUrl = environment.apiBaseUrl;
  private readonly HTTP = APP_CONSTANTS.HTTP;

  readonly isLoggedIn = signal<boolean>(this.hasToken());



  private readonly httpOptions = { withCredentials: true };

  private getAuthOptions(): { headers: Record<string, string>; withCredentials: boolean } {
    const token = this.getSessionToken();
    return {
      headers: token ? { Authorization: `${this.HTTP.AUTH_HEADER_PREFIX}${token}` } : {},
      withCredentials: true,
    };
  }

  login(request: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http
      .post<LoginResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.LOGIN}`, request, this.httpOptions)
      .pipe(
        timeout(this.HTTP.TIMEOUT),
        tap((response) => {
          if (response?.token) {
            sessionStorage.setItem(AUTH_STORAGE_KEYS.SESSION_TOKEN, response.token);
            this.isLoggedIn.set(true);
          }
        }),

        switchMap((response) =>
          response?.token
            ? this.fetchUserPermissions().pipe(
                map(() => response),
                catchError(() => of(response)),
              )
            : of(response),
        ),
      );
  }

  register(request: RegisterRequestDto): Observable<SuccessResponseDto> {
    const formData = new FormData();
    formData.append('UserName', request.userName);
    formData.append('Email', request.email);
    formData.append('Password', request.password);
    formData.append('Phone', request.phone);
    formData.append('Role_id', request.role_id);
    formData.append('Flat_id', request.flat_id);
    if (request.photo) {
      formData.append('Photo', request.photo);
    }
    return this.http
      .post<SuccessResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.SIGNUP}`, formData)
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  refreshToken(): Observable<RefreshTokenResponseDto> {
    return this.http
      .post<RefreshTokenResponseDto>(
        `${this.baseUrl}${AUTH_ENDPOINTS.REFRESH_TOKEN}`,
        {},
        this.httpOptions,
      )
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  fetchUserPermissions(): Observable<PermissionResponseDto | null> {
    return this.permissionService.load();
  }

  httpLogout(): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}${AUTH_ENDPOINTS.LOGOUT}`, {}, this.getAuthOptions())
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  clearSession(): void {
    sessionStorage.removeItem(AUTH_STORAGE_KEYS.SESSION_TOKEN);
    this.isLoggedIn.set(false);
    this.permissionService.clear();
  }

  forgotPassword(request: ForgotPasswordRequestDto): Observable<SuccessResponseDto> {
    return this.http
      .post<SuccessResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.FORGOT_PASSWORD}`, request)
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  verifyOtp(request: VerifyOtpDto): Observable<VerifyOtpResponseDto> {
    return this.http
      .post<VerifyOtpResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.VERIFY_OTP}`, request)
      .pipe(timeout(15000));
  }

  resetPassword(request: ResetPasswordDto): Observable<SuccessResponseDto> {
    return this.http
      .post<SuccessResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.RESET_PASSWORD}`, request)
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  changePassword(request: ChangePasswordDto): Observable<SuccessResponseDto> {
    return this.http
      .post<SuccessResponseDto>(`${this.baseUrl}${AUTH_ENDPOINTS.CHANGE_PASSWORD}`, request)
      .pipe(timeout(this.HTTP.TIMEOUT));
  }

  getSessionToken(): string | null {
    return sessionStorage.getItem(AUTH_STORAGE_KEYS.SESSION_TOKEN);
  }

  getAccessToken(): string | null {
    return this.getSessionToken();
  }

  getUserRole(): string | null {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }
    try {
      const payloadSegment = token.split('.')[1];
      const payload = JSON.parse(atob(payloadSegment));
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    } catch {
      return null;
    }
  }

  getUserId(): string | null {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }
    try {
      const payloadSegment = token.split('.')[1];
      const payload = JSON.parse(atob(payloadSegment));
      return payload['sub'] ?? null;
    } catch {
      return null;
    }
  }

  private hasToken(): boolean {
    return !!sessionStorage.getItem(AUTH_STORAGE_KEYS.SESSION_TOKEN);
  }
}
