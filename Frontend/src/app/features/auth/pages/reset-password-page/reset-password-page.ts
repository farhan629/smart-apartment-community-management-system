import { Component, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';
import { AuthService } from '../../../../core/services/auth-service';
import { AUTH_ROUTES } from '../../../../core/constants/auth.constants';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';

@Component({
  selector: 'app-reset-password-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password-page.html',
  styleUrl: './reset-password-page.scss',
})
export class ResetPasswordPage implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  readonly loginRoute = AUTH_ROUTES.LOGIN;
  readonly strings = APP_CONSTANTS.STRINGS;
  readonly validation = APP_CONSTANTS.VALIDATION;
  resetToken: string;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isSubmitting = false;

  resetPasswordForm = this.fb.nonNullable.group({
    newPassword: ['', [Validators.required, Validators.pattern(this.validation.PASSWORD_PATTERN)]],
    confirmPassword: ['', [Validators.required]],
  });

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras.state as { resetToken: string } | null;
    this.resetToken = state?.resetToken || '';
    if (!this.resetToken) {
      this.router.navigate(['/', this.loginRoute]);
    }
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    const { newPassword, confirmPassword } = this.resetPasswordForm.getRawValue();

    if (newPassword !== confirmPassword) {
      this.errorMessage = this.strings.PASSWORDS_MISMATCH;
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    this.isSubmitting = true;

    this.authService
      .resetPassword({
        resetToken: this.resetToken,
        newPassword,
        confirmPassword,
      })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.isSubmitting = false)),
      )
      .subscribe({
        next: (response) => {
          this.successMessage = response.message;
          setTimeout(() => this.router.navigate(['/', this.loginRoute]), 1200);
        },
        error: (err) => {
          this.errorMessage = err.error?.error || this.strings.RESET_PASSWORD_FAILED;
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
