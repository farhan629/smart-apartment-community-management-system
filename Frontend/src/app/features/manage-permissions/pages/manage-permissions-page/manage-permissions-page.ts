import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { UserSearchResultDto, PermissionAssignItemDto } from '../../../../core/models/manage-permissions.models';
import { ManagePermissionsService } from '../../services/manage-permissions.service';
import { UserSearchComponent } from '../../components/user-search/user-search';
import { PermissionListComponent } from '../../components/permission-list/permission-list';
import { EmptyState } from '../../../../shared/components/empty-state/empty-state';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';

@Component({
  selector: 'app-manage-permissions-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    UserSearchComponent,
    PermissionListComponent,
    EmptyState,
  ],
  templateUrl: './manage-permissions-page.html',
  styleUrl: './manage-permissions-page.scss',
})
export class ManagePermissionsPage implements OnInit {
  private readonly service = inject(ManagePermissionsService);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly permissionsList = Object.values(PERMISSIONS);

  readonly selectedUser = signal<UserSearchResultDto | null>(null);
  readonly permissionState = signal<Record<string, boolean>>({});
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly success = signal('');

  private originalState: Record<string, boolean> = {};

  ngOnInit(): void {
    this.initPermissionState();
  }

  onUserSelected(user: UserSearchResultDto): void {
    this.selectedUser.set(user);
    this.loading.set(true);
    this.error.set('');
    this.success.set('');

    this.service.getUserPermissions(user.id).subscribe({
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
        this.initPermissionState();
        this.loading.set(false);
      },
    });
  }

  onCleared(): void {
    this.selectedUser.set(null);
    this.initPermissionState();
    this.error.set('');
    this.success.set('');
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

  onUpdate(): void {
    const user = this.selectedUser();
    if (!user) { return; }

    const changes = this.buildChangedPermissions();
    if (changes.length === 0) { return; }

    this.saving.set(true);
    this.error.set('');
    this.success.set('');

    this.service.assignPermissions({ userId: user.id, permissions: changes }).subscribe({
      next: () => {
        this.originalState = { ...this.permissionState() };
        this.success.set(this.strings.PERMISSIONS_UPDATED);
        this.saving.set(false);
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

  private initPermissionState(): void {
    const state: Record<string, boolean> = {};
    for (const perm of this.permissionsList) {
      state[perm] = false;
    }
    this.permissionState.set(state);
    this.originalState = { ...state };
  }
}