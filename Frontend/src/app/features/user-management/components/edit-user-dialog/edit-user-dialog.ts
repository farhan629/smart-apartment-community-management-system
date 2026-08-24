import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { UserListItem, UploadResponseDto } from '../../../../core/models/user-management.models';
import { UserManagementService } from '../../services/user-management.service';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-edit-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    PopupDialog,
    ActionButton,
  ],
  templateUrl: './edit-user-dialog.html',
  styleUrl: './edit-user-dialog.scss',
})
export class EditUserDialog implements OnInit {
  private readonly service = inject(UserManagementService);
  private readonly dialogRef = inject(MatDialogRef<EditUserDialog>);
  private readonly data: UserListItem = inject(MAT_DIALOG_DATA);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;

  userName = '';
  phone = '';
  photoUrl = '';
  readonly loadingDetails = signal(false);
  readonly submitting = signal(false);
  readonly uploading = signal(false);
  readonly error = signal('');

  ngOnInit(): void {
    this.userName = this.data.userName;
    this.phone = this.data.phone ?? '';
    this.loadUserDetails();
  }

  private loadUserDetails(): void {
    this.loadingDetails.set(true);
    this.service.getUserById(this.data.id).subscribe({
      next: (user) => {
        this.photoUrl = this.service.resolvePhotoUrl(user.photoUrl ?? '');
        this.loadingDetails.set(false);
      },
      error: () => {
        this.loadingDetails.set(false);
      },
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) { return; }

    this.uploading.set(true);
    this.error.set('');
    this.service.uploadImage(input.files[0]).subscribe({
      next: (res: UploadResponseDto) => {
        this.photoUrl = this.service.resolvePhotoUrl(res.imageUrl);
        this.uploading.set(false);
      },
      error: (err) => {
        const body = err.error;
        const message = body?.detail || body?.title || body?.message || err.message || this.strings.FAILED_TO_UPLOAD_IMAGE;
        this.error.set(message);
        this.uploading.set(false);
      },
    });
  }

  onSubmit(): void {
    if (!this.userName || !this.phone) {
      this.error.set(this.strings.REQUIRED_FIELDS);
      return;
    }

    this.submitting.set(true);
    this.error.set('');

    this.service
      .updateUser(this.data.id, {
        userName: this.userName,
        phone: this.phone,
        photoUrl: this.photoUrl,
      })
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          const body = err.error;
          const message = body?.detail || body?.title || body?.message || err.message || this.strings.FAILED_TO_UPDATE_USER;
          this.error.set(message);
          this.submitting.set(false);
        },
      });
  }

  resolvePhotoUrl(path: string): string {
    return this.service.resolvePhotoUrl(path);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
