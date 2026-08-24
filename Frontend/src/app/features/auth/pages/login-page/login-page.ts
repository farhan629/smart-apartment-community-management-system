import { Component, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../../../core/services/auth-service';
import { AUTH_MESSAGES, AUTH_ROUTES } from '../../../../core/constants/auth.constants';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroy$ = new Subject<void>();

  readonly registerRoute = AUTH_ROUTES.REGISTER;
  readonly forgotPasswordRoute = AUTH_ROUTES.FORGOT_PASSWORD;
  readonly strings = APP_CONSTANTS.STRINGS;
  readonly validation = APP_CONSTANTS.VALIDATION;
  readonly routes = APP_CONSTANTS.ROUTES;

  errorMessage: string | null = null;
  isSubmitting = false;

  loginForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.pattern(this.validation.EMAIL_PATTERN)]],
    password: ['', [Validators.required, Validators.pattern(this.validation.PASSWORD_PATTERN)]],
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.isSubmitting = true;

    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.snackBar.open(this.strings.LOGIN_SUCCESS, this.strings.CLOSE, {
            duration: 3000,
            panelClass: 'snackbar-success',
          });
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || this.routes.DASHBOARD;
          this.router.navigate([returnUrl]);
        },
        error: () => {
          this.isSubmitting = false;
          this.errorMessage = AUTH_MESSAGES.LOGIN_FAILED;
          this.snackBar.open(AUTH_MESSAGES.LOGIN_FAILED, this.strings.CLOSE, {
            duration: 5000,
            panelClass: 'snackbar-error',
          });
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
