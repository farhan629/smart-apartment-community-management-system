import { Component, inject, signal, ChangeDetectorRef, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { Subject, takeUntil, filter, take } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';

import { PermissionService } from '../../../core/services/permission.service';
import { APP_CONSTANTS, ThemeMode } from '../../../core/constants/app.constants';
import { UserManagementService } from '../../user-management/services/user-management.service';
import { UploadResponseDto } from '../../../core/models/user-management.models';
import { AuthService } from '../../../core/services/auth-service';
import { ThemeService } from '../../../core/services/theme.service';

type SettingsTab = 'profile' | 'appearance' | 'security';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatIconModule],
  templateUrl: './settings-page.html',
  styleUrl: './settings-page.scss',
})
export class SettingsPageComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly permissionService = inject(PermissionService);
  private readonly userManagementService = inject(UserManagementService);
  private readonly authService = inject(AuthService);
  private readonly themeService = inject(ThemeService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroy$ = new Subject<void>();

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly validation = APP_CONSTANTS.VALIDATION;

  readonly activeTab = signal<SettingsTab>('profile');

  isProfileLoading = false;
  isSubmittingProfile = false;
  isUploadingPhoto = false;
  isChangingPassword = false;

  userEmail = '';
  photoUrl = '';

  profileForm = this.fb.nonNullable.group({
    userName: ['', Validators.required],
    phone: ['', [Validators.required, Validators.pattern(this.validation.PHONE_PATTERN)]],
    photoUrl: [''],
  });

  securityForm = this.fb.nonNullable.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.pattern(this.validation.PASSWORD_PATTERN)]],
    confirmPassword: ['', Validators.required],
  }, { validators: [this.passwordsMatch, this.newPasswordDifferentFromCurrent] });

  private passwordsMatch(group: AbstractControl): ValidationErrors | null {
    const newPw = group.get('newPassword')?.value;
    const confirmPw = group.get('confirmPassword')?.value;
    return newPw !== confirmPw ? { passwordsMismatch: true } : null;
  }

  private newPasswordDifferentFromCurrent(group: AbstractControl): ValidationErrors | null {
    const currentPw = group.get('currentPassword')?.value;
    const newPw = group.get('newPassword')?.value;
    return currentPw && newPw && currentPw === newPw ? { newPasswordSameAsCurrent: true } : null;
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  private loadProfile(): void {
    const userId = this.permissionService.userId();
    if (userId && this.permissionService.loaded()) {
      this.fetchProfile(userId);
      return;
    }

    toObservable(this.permissionService.loaded).pipe(
      filter(Boolean),
      take(1),
      takeUntil(this.destroy$),
    ).subscribe(() => {
      const uid = this.permissionService.userId();
      if (uid) {
        this.fetchProfile(uid);
      }
    });
  }

  private fetchProfile(userId: string): void {
    this.isProfileLoading = true;
    this.userManagementService.getUserById(userId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (user) => {
          this.profileForm.reset({
            userName: user.userName ?? '',
            phone: user.phone ?? '',
            photoUrl: user.photoUrl ?? '',
          });
          this.userEmail = user.email ?? '';
          this.photoUrl = this.userManagementService.resolvePhotoUrl(user.photoUrl ?? '');
          this.isProfileLoading = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.isProfileLoading = false;
          this.cdr.detectChanges();
          this.snackBar.open(this.strings.FAILED_TO_LOAD_USER_DETAILS, this.strings.CLOSE, {
            duration: 5000,
            panelClass: 'snackbar-error',
          });
        },
      });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) { return; }

    this.isUploadingPhoto = true;
    this.userManagementService.uploadImage(input.files[0])
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: UploadResponseDto) => {
          try {
            const url = res?.imageUrl ?? '';
            if (url) {
              this.photoUrl = this.userManagementService.resolvePhotoUrl(url);
              this.profileForm.patchValue({ photoUrl: url });
              this.profileForm.markAsDirty();
            }
          } finally {
            this.isUploadingPhoto = false;
          }
        },
        error: () => {
          this.isUploadingPhoto = false;
          this.snackBar.open(this.strings.FAILED_TO_UPLOAD_IMAGE, this.strings.CLOSE, {
            duration: 5000,
            panelClass: 'snackbar-error',
          });
        },
      });
  }

  onUpdateProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const userId = this.permissionService.userId();
    if (!userId) { return; }

    this.isSubmittingProfile = true;
    const { userName, phone, photoUrl } = this.profileForm.getRawValue();

    this.userManagementService.updateUser(userId, { userName, phone, photoUrl })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isSubmittingProfile = false;
          this.profileForm.markAsPristine();
          this.snackBar.open(
            this.strings.USER_UPDATED_SUCCESS,
            this.strings.CLOSE,
            { duration: 3000, panelClass: 'snackbar-success' },
          );
        },
        error: () => {
          this.isSubmittingProfile = false;
          this.snackBar.open(this.strings.FAILED_TO_UPDATE_USER, this.strings.CLOSE, {
            duration: 5000,
            panelClass: 'snackbar-error',
          });
        },
      });
  }

  onChangePassword(): void {
    if (this.securityForm.invalid) {
      this.securityForm.markAllAsTouched();
      return;
    }

    this.isChangingPassword = true;
    const { currentPassword, newPassword, confirmPassword } = this.securityForm.getRawValue();

    this.authService.changePassword({ currentPassword, newPassword, confirmPassword })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.isChangingPassword = false;
          this.securityForm.reset();
          this.snackBar.open(res.message, this.strings.CLOSE, {
            duration: 3000,
            panelClass: 'snackbar-success',
          });
        },
        error: () => {
          this.isChangingPassword = false;
          this.snackBar.open(this.strings.CHANGE_PASSWORD_FAILED, this.strings.CLOSE, {
            duration: 5000,
            panelClass: 'snackbar-error',
          });
        },
      });
  }

  readonly themeModes = APP_CONSTANTS.THEME.MODE_OPTIONS;
  readonly colorPalette = APP_CONSTANTS.THEME.COLOR_OPTIONS;
  readonly fontFamilies = APP_CONSTANTS.THEME.FONT_OPTIONS;

  readonly preference = this.themeService.preference;

  onSelectMode(mode: ThemeMode): void {
    this.themeService.setMode(mode);
    this.notifyAppearanceSaved();
  }

  onSelectColor(colorKey: string): void {
    this.themeService.setColor(colorKey);
    this.notifyAppearanceSaved();
  }

  onSelectFont(fontKey: string): void {
    this.themeService.setFont(fontKey);
    this.notifyAppearanceSaved();
  }

  private notifyAppearanceSaved(): void {
    this.snackBar.open(this.strings.APPEARANCE_SAVED, this.strings.CLOSE, {
      duration: 2000,
      panelClass: 'snackbar-success',
    });
  }

  handleImageError(): void {
    this.photoUrl = '';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
