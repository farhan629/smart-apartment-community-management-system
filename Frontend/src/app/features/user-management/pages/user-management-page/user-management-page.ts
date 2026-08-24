import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  UserListItem,
  UserDetailDto,
  RoleOptionDto,
} from '../../../../core/models/user-management.models';
import { UserManagementService } from '../../services/user-management.service';
import { AddUserDialog } from '../../components/add-user-dialog/add-user-dialog';
import { EditUserDialog } from '../../components/edit-user-dialog/edit-user-dialog';
import { UpdatePermissionDialog } from '../../components/update-permission-dialog/update-permission-dialog';
import { ViewUserDialog } from '../../components/view-user-dialog/view-user-dialog';
import {
  ActionButton,
  ActionMenuItem,
} from '../../../../shared/components/action-button/action-button';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../../../shared/components/empty-state/empty-state';
import { SearchBar } from '../../../../shared/components/search-bar/search-bar';
import { CapitalizeFirstPipe } from '../../../../shared/pipes/capitalize-first.pipe';
import { StaffAvailabilityDialog } from '../../components/staff-availability-dialog/staff-availability-dialog';
import { ApprovalSlideWindow } from '../../components/approval-slide-window/approval-slide-window';

@Component({
  selector: 'app-user-management-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatPaginatorModule,
    ActionButton,
    EmptyState,
    SearchBar,
    CapitalizeFirstPipe,
    ApprovalSlideWindow,
  ],
  templateUrl: './user-management-page.html',
  styleUrl: './user-management-page.scss',
})
export class UserManagementPage implements OnInit {
  readonly showApprovalPanel = signal(false);

  private readonly DIALOG_WIDTH = APP_CONSTANTS.DIALOG_WIDTHS;
  private readonly service = inject(UserManagementService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly actions = APP_CONSTANTS.ACTIONS;
  readonly pagination = APP_CONSTANTS.PAGINATION;

  readonly users = signal<UserListItem[]>([]);
  readonly total = signal(0);
  readonly loading = signal(false);
  readonly error = signal('');

  readonly roles = signal<RoleOptionDto[]>([]);
  readonly rolesLoading = signal(false);

  readonly searchTerm = signal('');
  readonly selectedRoleId = signal('');
  readonly page = signal(0);
  readonly pageSize = signal(this.pagination.DEFAULT_PAGE_SIZE);

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.error.set('');
    const apiPage = this.page() + 1;
    this.service
      .getUsers(apiPage, this.pageSize(), this.searchTerm(), this.selectedRoleId())
      .subscribe({
        next: (res) => {
          this.users.set(res.items);
          this.total.set(res.total);
          this.loading.set(false);
        },
        error: () => {
          this.error.set(this.strings.FAILED_TO_LOAD_USERS);
          this.loading.set(false);
        },
      });
  }

  private loadRoles(): void {
    this.rolesLoading.set(true);
    this.service.getAllRoles().subscribe({
      next: (roles) => {
        this.roles.set(roles);
        this.rolesLoading.set(false);
      },
      error: () => {
        this.rolesLoading.set(false);
      },
    });
  }

  toggleApprovalPanel(): void {
    this.showApprovalPanel.update((v) => !v);
  }

  closeApprovalPanel(): void {
    this.showApprovalPanel.set(false);
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.page.set(0);
    this.loadUsers();
  }

  onRoleFilterChange(event: Event): void {
    this.selectedRoleId.set((event.target as HTMLSelectElement).value);
    this.page.set(0);
    this.loadUsers();
  }

  onPageChange(event: PageEvent): void {
    this.page.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.loadUsers();
  }

  getActionMenuItems(user: UserListItem): ActionMenuItem[] {
    const items: ActionMenuItem[] = [
      { label: this.strings.VIEW_USER, icon: this.icons.OPEN_IN_NEW, action: this.actions.VIEW },
      { label: this.strings.EDIT_USER, icon: this.icons.EDIT, action: this.actions.EDIT },
      {
        label: this.strings.UPDATE_USER_PERMISSION,
        icon: this.icons.SETTINGS,
        action: this.actions.PERMISSION,
      },
    ];

    if (user.role === this.strings.ROLE_STAFF) {
      items.splice(2, 0, {
        label: this.strings.AVAILABILITY,
        icon: this.icons.AVAILABILITY,
        action: this.actions.AVAILABILITY,
      });
    }

    items.push({
      label: this.strings.DELETE_USER,
      icon: this.icons.DELETE,
      action: this.actions.DELETE,
    });
    return items;
  }

  onMenuAction(user: UserListItem, action: string): void {
    switch (action) {
      case this.actions.VIEW:
        this.openViewDialog(user);
        break;
      case this.actions.EDIT:
        this.openEditDialog(user);
        break;
      case this.actions.AVAILABILITY:
        this.openAvailabilityDialog(user);
        break;
      case this.actions.PERMISSION:
        this.openPermissionDialog(user.id);
        break;
      case this.actions.DELETE:
        this.confirmDelete(user);
        break;
    }
  }

  openAddDialog(): void {
    const ref = this.dialog.open(AddUserDialog, {
      width: this.DIALOG_WIDTH.MEDIUM,
      disableClose: true,
    });
    ref.afterClosed().subscribe((result) => {
      if (result) {
        this.loadUsers();
        this.snackBar.open(this.strings.USER_ADDED_SUCCESS, this.strings.CLOSE, {
          duration: 3000,
          panelClass: 'snackbar-success',
        });
      }
    });
  }

  openViewDialog(user: UserListItem): void {
    const ref = this.dialog.open(ViewUserDialog, {
      width: this.DIALOG_WIDTH.MEDIUM,
      data: user.id,
      disableClose: true,
    });
    ref.afterClosed().subscribe((action) => {
      if (action === this.actions.EDIT) {
        this.openEditDialog(user);
      } else if (action === this.actions.PERMISSION) {
        this.openPermissionDialog(user.id);
      } else if (action === this.actions.DELETE) {
        this.confirmDelete(user);
      } else if (action === this.actions.AVAILABILITY) {
        this.openAvailabilityDialog(user);
      }
    });
  }

  private openAvailabilityDialog(user: UserListItem): void {
    this.dialog.open(StaffAvailabilityDialog, {
      width: this.DIALOG_WIDTH.MEDIUM,
      data: { staffId: user.id, categoryName: user.userName },
      disableClose: true,
    });
  }

  private openEditDialog(user: UserListItem): void {
    const ref = this.dialog.open(EditUserDialog, {
      width: this.DIALOG_WIDTH.SMALL,
      data: user,
      disableClose: true,
    });
    ref.afterClosed().subscribe((result) => {
      if (result) {
        this.loadUsers();
        this.snackBar.open(this.strings.USER_UPDATED_SUCCESS, this.strings.CLOSE, {
          duration: 3000,
          panelClass: 'snackbar-success',
        });
      }
    });
  }

  private openPermissionDialog(userId: string): void {
    this.dialog.open(UpdatePermissionDialog, {
      width: this.DIALOG_WIDTH.LARGE,
      data: userId,
      disableClose: true,
    });
  }

  private confirmDelete(user: UserListItem): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.strings.DELETE_USER,
        message: this.strings.DELETE_USER_CONFIRM,
        variant: 'danger',
      },
    });
    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.deleteUser(user.id);
      }
    });
  }

  private deleteUser(id: string): void {
    this.service.deleteUser(id).subscribe({
      next: () => {
        this.loadUsers();
        this.snackBar.open(this.strings.USER_DELETED_SUCCESS, this.strings.CLOSE, {
          duration: 3000,
          panelClass: 'snackbar-success',
        });
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_DELETE_USER);
        this.snackBar.open(this.strings.FAILED_TO_DELETE_USER, this.strings.CLOSE, {
          duration: 5000,
          panelClass: 'snackbar-error',
        });
      },
    });
  }
}
