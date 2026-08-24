import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { PERMISSIONS, PERMISSION_LABELS } from '../../../../core/constants/permission.constants';
import { PermissionAssignItemDto } from '../../../../core/models/manage-permissions.models';
import { UserManagementService } from '../../services/user-management.service';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-update-permission-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    PopupDialog,
    ActionButton,
  ],
  templateUrl: './update-permission-dialog.html',
  styleUrl: './update-permission-dialog.scss',
})
export class UpdatePermissionDialog implements OnInit {
  private readonly service = inject(UserManagementService);
  private readonly dialogRef = inject(MatDialogRef<UpdatePermissionDialog>);
  private readonly userId: string = inject(MAT_DIALOG_DATA);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly permissionsList = Object.values(PERMISSIONS);

  permissionLabel(perm: string): string {
    return PERMISSION_LABELS[perm] ?? perm;
  }

  readonly permissionState = signal<Record<string, boolean>>({});
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly success = signal('');

  private originalState: Record<string, boolean> = {};

  ngOnInit(): void {
    this.loadPermissions();
  }

  private loadPermissions(): void {
    this.loading.set(true);
    this.service.getUserPermissions(this.userId).subscribe({
      next: (res) => {
        const state: Record<string, boolean> = {};
        for (const perm of this.permissionsList) {
          state[perm] = res.permissions.includes(perm);
        }
        this.permissionState.set(state);
        this.originalState = { ...state };
        this.loading.set(false);
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_LOAD_PERMISSIONS);
        this.initState();
        this.loading.set(false);
      },
    });
  }

  onToggle(permission: string): void {
    this.permissionState.update((state) => ({
      ...state,
      [permission]: !state[permission],
    }));
  }

  hasChanges(): boolean {
    const current = this.permissionState();
    for (const perm of this.permissionsList) {
      if (current[perm] !== this.originalState[perm]) {
        return true;
      }
    }
    return false;
  }

  onSave(): void {
    const changes = this.buildChangedPermissions();
    if (changes.length === 0) { return; }

    this.saving.set(true);
    this.error.set('');
    this.success.set('');

    this.service.assignPermissions({ userId: this.userId, permissions: changes }).subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_UPDATE_PERMISSIONS);
        this.saving.set(false);
      },
    });
  }

  private buildChangedPermissions(): PermissionAssignItemDto[] {
    const current = this.permissionState();
    const changes: PermissionAssignItemDto[] = [];
    for (const perm of this.permissionsList) {
      if (current[perm] !== this.originalState[perm]) {
        changes.push({ permissionCode: perm, isAllowed: current[perm] });
      }
    }
    return changes;
  }

  private initState(): void {
    const state: Record<string, boolean> = {};
    for (const perm of this.permissionsList) {
      state[perm] = false;
    }
    this.permissionState.set(state);
    this.originalState = { ...state };
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
