import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogRef } from '@angular/material/dialog';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  ManagementRoleDto,
  CategoryDto,
} from '../../../../core/models/user-management.models';
import { UserManagementService } from '../../services/user-management.service';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-add-user-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    PopupDialog,
    ActionButton,
  ],
  templateUrl: './add-user-dialog.html',
  styleUrl: './add-user-dialog.scss',
})
export class AddUserDialog implements OnInit {
  private readonly service = inject(UserManagementService);
  private readonly dialogRef = inject(MatDialogRef<AddUserDialog>);

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  private readonly ROLES = APP_CONSTANTS.ROLES;
  private readonly sentinels = APP_CONSTANTS.SENTINELS;

  userName = '';
  email = '';
  password = '';
  phone = '';
  selectedRole: ManagementRoleDto | null = null;
  selectedCategory: CategoryDto | null = null;
  photo: File | null = null;
  photoPreview: string | null = null;

  readonly roles = signal<ManagementRoleDto[]>([]);
  readonly categories = signal<CategoryDto[]>([]);
  readonly loadingRoles = signal(false);
  readonly loadingCategories = signal(false);
  readonly submitting = signal(false);
  readonly error = signal('');

  ngOnInit(): void {
    this.loadRoles();
  }

  private loadRoles(): void {
    this.loadingRoles.set(true);
    this.service.getManagementRoles().subscribe({
      next: (roles) => {
        this.roles.set(roles.filter((r) => r.termValue !== this.ROLES.ADMIN));
        this.loadingRoles.set(false);
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_LOAD_ROLES);
        this.loadingRoles.set(false);
      },
    });
  }

  onRoleSelect(role: ManagementRoleDto): void {
    this.selectedRole = role;
    this.selectedCategory = null;
    if (role.termValue === this.ROLES.STAFF) {
      this.loadCategories();
    } else {
      this.categories.set([]);
    }
  }

  private loadCategories(): void {
    this.loadingCategories.set(true);
    this.service.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.loadingCategories.set(false);
      },
      error: () => {
        this.error.set(this.strings.FAILED_TO_LOAD_CATEGORIES);
        this.loadingCategories.set(false);
      },
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.photo = input.files[0];
      const reader = new FileReader();
      reader.onload = () => (this.photoPreview = reader.result as string);
      reader.readAsDataURL(this.photo);
    }
  }

  removePhoto(): void {
    this.photo = null;
    this.photoPreview = null;
  }

  onSubmit(): void {
    if (!this.userName || !this.email || !this.password || !this.phone || !this.selectedRole) {
      this.error.set(this.strings.REQUIRED_FIELDS);
      return;
    }

    this.submitting.set(true);
    this.error.set('');

    this.service
      .registerUser({
        userName: this.userName,
        email: this.email,
        password: this.password,
        phone: this.phone,
        role_id: this.selectedRole.id,
        category_id: this.selectedCategory?.id ?? this.sentinels.DUMMY_GUID,
        photo: this.photo ?? undefined,
      })
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (err) => {
          const body = err.error;
          const message = body?.detail || body?.title || body?.message || err.message || this.strings.FAILED_TO_ADD_USER;
          this.error.set(message);
          this.submitting.set(false);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
