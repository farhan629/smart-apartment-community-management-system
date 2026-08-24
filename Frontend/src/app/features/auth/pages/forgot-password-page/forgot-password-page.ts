import { Component, inject, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';
import { AuthService } from '../../../../core/services/auth-service';
import { AUTH_ROUTES } from '../../../../core/constants/auth.constants';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';

@Component({
  selector: 'app-forgot-password-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.scss',
})
export class ForgotPasswordPage implements OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroy$ = new Subject<void>();

  readonly loginRoute = AUTH_ROUTES.LOGIN;
  readonly strings = APP_CONSTANTS.STRINGS;
  readonly validation = APP_CONSTANTS.VALIDATION;

  step: 'phone' | 'otp' = 'phone';
  phoneNumber = '';
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isSubmitting = false;

  phoneForm = this.fb.nonNullable.group({
    phone: ['', [Validators.required, Validators.pattern(this.validation.PHONE_PATTERN)]],
  });

  otpForm = this.fb.nonNullable.group({
    otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
  });

  onSendOtp(): void {
    if (this.phoneForm.invalid) {
      this.phoneForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    this.isSubmitting = true;
    this.phoneNumber = this.phoneForm.getRawValue().phone;

    this.authService
      .forgotPassword({ phone: this.phoneNumber })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSubmitting = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (response) => {
          this.successMessage = response.message;
          this.step = 'otp';
        },
        error: (err) => {
          this.errorMessage = err.error?.error || this.strings.SEND_OTP_FAILED;
        },
      });
  }

  onVerifyOtp(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.isSubmitting = true;
    const otp = this.otpForm.getRawValue().otp;

    this.authService
      .verifyOtp({ phone: this.phoneNumber, otp })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.isSubmitting = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (response) => {
          this.router.navigate(['/', AUTH_ROUTES.RESET_PASSWORD], {
            state: { resetToken: response.resetToken },
          });
        },
        error: (err) => {
          this.errorMessage = err.error?.error || this.strings.INVALID_OTP;
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
