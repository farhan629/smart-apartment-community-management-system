import { Component, OnInit, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../../../../environments/environment';
import { AmenityService, AmenityResponseDto, AmenityListResponseDto, UpdateAmenityRequestDto } from '../../../../core/services/aminety-service';
import { PermissionService } from '../../../../core/services/permission.service';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { AmenityUpdate } from '../amenity-update/amenity-update';
import { AmenityDelete } from '../amenity-delete/amenity-delete';
import { SLOT_TYPES, SLOT_TYPE_LABELS, AMENITY_STATUS, AMENITY_STATUS_CLASSES, AMENITY_CARDS_STRINGS, AMENITY_ROUTES, API_GATEWAY_REPLACE, STATUS_KEYWORDS, UPDATE_AMENITY_STRINGS, DELETE_AMENITY_STRINGS } from '../../../../core/constants/amenity.constants';
import { AmenitySlot } from '../amenity-slot/amenity-slot';

export interface AmenityUIDetails {
  description: string;
  capacity?: number;
  hours: string; 
  image: string;
  statusText: string;
  statusClass: string;
}

@Component({
  selector: 'app-amenity-cards',
  standalone: true,
  imports: [CommonModule, AmenityUpdate, AmenityDelete, MatButtonModule, MatSnackBarModule, AmenitySlot],
  templateUrl: './amenity-cards.html',
  styleUrl: './amenity-cards.scss',
})
export class AmenityCards implements OnInit {
  cardStrings = AMENITY_CARDS_STRINGS;
  updateStrings = UPDATE_AMENITY_STRINGS;
  deleteStrings = DELETE_AMENITY_STRINGS;

  amenitylist = signal<AmenityResponseDto[]>([]);

  isAdmin = computed(() => this.permissionService.hasPermission(PERMISSIONS.AMENITY_MANAGE));

  activeDropdownId = signal<string | null>(null);
  selectedAmenityForUpdate = signal<AmenityResponseDto | null>(null);
  selectedAmenityForDelete = signal<AmenityResponseDto | null>(null);
  selectedAmenityForSlot = signal<AmenityResponseDto | null>(null)
  isUpdating = signal<boolean>(false);
  isDeleting = signal<boolean>(false);

  constructor(
    private amenityService: AmenityService,
    private permissionService: PermissionService,
    private router: Router,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit() {
    this.loadAmenity();
  }

  onCardClick(amenityId: string | undefined): void {
    if (!amenityId) return;
    this.router.navigate([AMENITY_ROUTES.BASE, amenityId, AMENITY_ROUTES.BOOK_SUFX]);
  }

  loadAmenity(): void {
    this.amenityService.getApiAmenity().subscribe({
      next: (response: AmenityListResponseDto) => {
        this.amenitylist.set(response.data ?? []);
      },
      error: (err: any) => {
        console.error('Error loading data', err);
        this.amenitylist.set([]);
      },
    });
  }

  toggleDropdown(id: string | undefined, event: Event): void {
    event.stopPropagation();
    if (!id) return;
    if (this.activeDropdownId() === id) {
      this.activeDropdownId.set(null);
    } else {
      this.activeDropdownId.set(id);
    }
  }

  openUpdateDialog(amenity: AmenityResponseDto, event: Event): void {
    event.stopPropagation();
    this.selectedAmenityForUpdate.set(amenity);
    this.activeDropdownId.set(null);
  }

  openDeleteDialog(amenity: AmenityResponseDto, event: Event): void {
    event.stopPropagation();
    this.selectedAmenityForDelete.set(amenity);
    this.activeDropdownId.set(null);
  }

  openSlotDialog(amenity: AmenityResponseDto, event: Event): void {
    event.stopPropagation();
    this.selectedAmenityForSlot.set(amenity);
    this.activeDropdownId.set(null);
  }

  closeSlotDialog(): void {
    this.selectedAmenityForSlot.set(null);
  }

  closeUpdateDialog(): void {
    this.selectedAmenityForUpdate.set(null);
  }

  closeDeleteDialog(): void {
    this.selectedAmenityForDelete.set(null);
  }

  executeUpdate(request: UpdateAmenityRequestDto): void {
    const id = this.selectedAmenityForUpdate()?.id;
    if (!id) return;

    this.isUpdating.set(true);
    this.amenityService.putApiAmenity(id, request).subscribe({
      next: () => {
        this.isUpdating.set(false);
        this.closeUpdateDialog();
        this.snackBar.open(this.updateStrings.TOAST_SUCCESS, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        this.loadAmenity();
      },
      error: (err: any) => {
        console.error('Error updating amenity', err);
        alert(this.updateStrings.ERROR_SUBMIT);
        this.isUpdating.set(false);
      }
    });
  }

  executeDelete(): void {
    const id = this.selectedAmenityForDelete()?.id;
    if (!id) return;

    this.isDeleting.set(true);
    this.amenityService.deleteApiAmenity(id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.closeDeleteDialog();
        this.snackBar.open(this.deleteStrings.TOAST_SUCCESS, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        this.loadAmenity();
      },
      error: (err: any) => {
        console.error('Error deleting amenity', err);
        alert(this.deleteStrings.ERROR_SUBMIT);
        this.isDeleting.set(false);
      }
    });
  }

  @HostListener('document:click')
  closeDropdowns(): void {
    this.activeDropdownId.set(null);
  }

  getAmenityUIDetails(amenity: AmenityResponseDto): AmenityUIDetails {
    const name = amenity.name ?? '';
    const normName = name.toLowerCase();

    let description = amenity.rules ?? '';
    let capacity: number | undefined = undefined;
    let image = '';

    if (amenity.imageUrl) {
      if (amenity.imageUrl.startsWith('http')) {
        image = amenity.imageUrl;
      } else {
        const base = environment.apiBaseUrl;
        image = `${base}${amenity.imageUrl}`;
      }
    }

    const slotType = amenity.slotType ?? '';
    let hoursText: string = SLOT_TYPE_LABELS.PRIVATE;
    if (slotType === SLOT_TYPES.TIME_COUNT) {
      hoursText = SLOT_TYPE_LABELS.SHARED;
    } else if (slotType === SLOT_TYPES.TIME) {
      hoursText = SLOT_TYPE_LABELS.PRIVATE;
    }

    const status = (amenity.status ?? 'Available').toLowerCase();
    let statusText: string = AMENITY_STATUS.AVAILABLE;
    let statusClass: string = AMENITY_STATUS_CLASSES.AVAILABLE;

    if (status.includes(STATUS_KEYWORDS.LIMIT)) {
      statusText = AMENITY_STATUS.LIMITED;
      statusClass = AMENITY_STATUS_CLASSES.LIMITED;
    } else if (STATUS_KEYWORDS.RESERVED.some(k => status.includes(k))) {
      statusText = AMENITY_STATUS.RESERVED;
      statusClass = AMENITY_STATUS_CLASSES.RESERVED;
    }

    return {
      description,
      capacity,
      hours: hoursText,
      image,
      statusText,
      statusClass,
    };
  }
}
