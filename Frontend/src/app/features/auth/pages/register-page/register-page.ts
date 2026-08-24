import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';
import { AuthService } from '../../../../core/services/auth-service';
import { FlatService } from '../../../../core/services/flat-service';
import { RoleService } from '../../../../core/services/role-service';
import { AUTH_MESSAGES, AUTH_ROUTES } from '../../../../core/constants/auth.constants';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { OccupantRoleDto, FlatItemDto } from '../../../../core/models/auth.models';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss',
})
export class RegisterPage implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly flatService = inject(FlatService);
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  readonly loginRoute = AUTH_ROUTES.LOGIN;
  readonly strings = APP_CONSTANTS.STRINGS;
  readonly validation = APP_CONSTANTS.VALIDATION;

  roles: OccupantRoleDto[] = [];
  flats: FlatItemDto[] = [];
  filteredFlats: FlatItemDto[] = [];
  blocks: string[] = [];
  flatSearchText = '';
  flatDropdownOpen = false;
  selectedFlat: FlatItemDto | null = null;

  errorMessage: string | null = null;
  successMessage: string | null = null;
  isSubmitting = false;
  rolesLoading = false;
  flatsLoading = false;

  registerForm = this.fb.group({
    userName: ['', Validators.required],
    email: ['', [Validators.required, Validators.pattern(this.validation.EMAIL_PATTERN)]],
    password: ['', [Validators.required, Validators.pattern(this.validation.PASSWORD_PATTERN)]],
    phone: ['', [Validators.required, Validators.pattern(this.validation.PHONE_PATTERN)]],
    role_id: ['', Validators.required],
    flat_id: ['', Validators.required],
    photo: [null as File | null],
  });

  ngOnInit(): void {
    this.loadRoles();
    this.loadFlats();
  }

  private loadRoles(): void {
    this.rolesLoading = true;
    this.roleService
      .getOccupantRoles()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.rolesLoading = false)),
      )
      .subscribe({
        next: (roles) => {
          this.roles = roles;
        },
        error: () => {},
      });
  }

  private loadFlats(): void {
    this.flatsLoading = true;
    this.flatService
      .getFlats()
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.flatsLoading = false)),
      )
      .subscribe({
        next: (response) => {
          this.flats = response.items;
          this.blocks = [...new Set(response.items.map((f) => f.block))].sort();
          this.filteredFlats = [...this.flats];
        },
        error: () => {},
      });
  }

  selectRole(roleId: string): void {
    this.registerForm.patchValue({ role_id: roleId });
  }

  onFlatSearchInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.flatSearchText = input.value;
    const term = this.flatSearchText.toLowerCase();
    const [blockPart, numPart] = term.split(' ').filter(Boolean);

    this.filteredFlats = this.flats.filter((f) => {
      const matchesBlock = !blockPart || f.block.toLowerCase().includes(blockPart);
      const matchesNumber = !numPart || f.number.toLowerCase().includes(numPart);
      const matchesSearch = !term || `${f.block} ${f.number}`.toLowerCase().includes(term);
      return matchesBlock && matchesNumber && matchesSearch;
    });

    this.flatDropdownOpen = true;
  }

  selectFlat(flat: FlatItemDto): void {
    this.selectedFlat = flat;
    this.flatSearchText = `${flat.block} - ${flat.number}`;
    this.registerForm.patchValue({ flat_id: flat.id });
    this.flatDropdownOpen = false;
  }

  toggleFlatDropdown(): void {
    this.flatDropdownOpen = !this.flatDropdownOpen;
    if (this.flatDropdownOpen) {
      this.filteredFlats = [...this.flats];
    }
  }

  closeFlatDropdown(): void {
    setTimeout(() => {
      this.flatDropdownOpen = false;
    }, 200);
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      const photoControl = this.registerForm.get('photo');
      if (photoControl) {
        photoControl.setValue(file as any);
      }
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    this.isSubmitting = true;

    const formValue = this.registerForm.value;
    const request = {
      userName: formValue.userName!,
      email: formValue.email!,
      password: formValue.password!,
      phone: formValue.phone!,
      role_id: formValue.role_id!,
      flat_id: formValue.flat_id!,
      photo: formValue.photo ?? undefined,
    };

    this.authService
      .register(request)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.isSubmitting = false)),
      )
      .subscribe({
        next: () => {
          this.successMessage = AUTH_MESSAGES.REGISTER_SUCCESS;
          setTimeout(() => this.router.navigate(['/', this.loginRoute]), 1200);
        },
        error: () => {
          this.errorMessage = AUTH_MESSAGES.REGISTER_FAILED;
        },
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
