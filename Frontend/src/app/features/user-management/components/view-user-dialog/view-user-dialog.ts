import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { FlatItemDto } from '../../../../core/models/auth.models';
import { UserDetailDto } from '../../../../core/models/user-management.models';
import { UserManagementService } from '../../services/user-management.service';
import { FlatService } from '../../../../core/services/flat-service';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-view-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    PopupDialog,
    ActionButton,
  ],
  templateUrl: './view-user-dialog.html',
  styleUrl: './view-user-dialog.scss',
})
export class ViewUserDialog implements OnInit {
  private readonly service = inject(UserManagementService);
  private readonly flatService = inject(FlatService);
  private readonly dialogRef = inject(MatDialogRef<ViewUserDialog>);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly dateFormats = APP_CONSTANTS.DATE_FORMATS;

  readonly user = signal<UserDetailDto | null>(null);
  readonly flat = signal<FlatItemDto | null>(null);
  readonly loading = signal(false);
  readonly flatLoading = signal(false);
  readonly error = signal('');

  constructor(@Inject(MAT_DIALOG_DATA) public userId: string) {}

  ngOnInit(): void {
    this.loadUser();
  }

  private loadUser(): void {
    this.loading.set(true);
    this.error.set('');
    this.service.getUserById(this.userId).subscribe({
      next: (user) => {
        this.user.set(user);
        this.loading.set(false);
        if (user.flatId) {
          this.loadFlat(user.flatId);
        }
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_LOAD_USER_DETAILS);
        this.loading.set(false);
      },
    });
  }

  private loadFlat(flatId: string): void {
    this.flatLoading.set(true);
    this.flatService.getFlatById(flatId).subscribe({
      next: (flat) => {
        this.flat.set(flat);
        this.flatLoading.set(false);
      },
      error: () => {
        this.flatLoading.set(false);
      },
    });
  }

  onEdit(): void {
    this.dialogRef.close('edit');
  }

  onPermission(): void {
    this.dialogRef.close('permission');
  }

  onDelete(): void {
    this.dialogRef.close('delete');
  }

  resolvePhotoUrl(path: string): string {
    return this.service.resolvePhotoUrl(path);
  }

  onClose(): void {
    this.dialogRef.close();
  }

  shouldShowFlat(): boolean {
    const role = this.user()?.role;
    return role !== 'Staff' && role !== 'Security';
  }
}
