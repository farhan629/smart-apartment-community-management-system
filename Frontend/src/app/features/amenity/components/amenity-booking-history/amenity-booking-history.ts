import { Component, OnInit, signal, computed, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { BookingService, BookingResponseDto, BookingListResponseDto, ReportResponseDto } from '../../../../core/services/aminety-service';
import { AuthService } from '../../../../core/services/auth-service';
import { PermissionService } from '../../../../core/services/permission.service';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { APP_CONSTANTS, Role } from '../../../../core/constants/app.constants';
import { VIEW_MODE, BOOKING_STATUS, BADGE_CLASSES, AMENITY_ICONS, BOOKING_CANCEL, BOOKING_HISTORY_STRINGS, AMENITY_ICON_KEYWORDS, AMENITY_ROUTES, PAGINATION_NUMBERS, CALENDER_NUMBERS } from '../../../../core/constants/amenity.constants';

export interface AmenityIconDetails {
  icon: string;
  bg: string;
}

@Component({
  selector: 'app-amenity-booking-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './amenity-booking-history.html',
  styleUrl: './amenity-booking-history.scss',
  host: {
    '(document:click)': 'closeMenu()'
  }
})
export class AmenityBookingHistory implements OnInit {
  @Input() limit: number | null = null;

  historyStrings = BOOKING_HISTORY_STRINGS;
  bookings = signal<BookingResponseDto[]>([]);
  currentRole!: Role;
  isAdmin = false;
  viewMode = signal<'mine' | 'all'>('mine');
  openMenuId = signal<string | null>(null);

  filterStatus = signal<string>('');
  filterFromDate = signal<string>('');
  filterToDate = signal<string>('');
  serverTotalPages = signal<number>(1);

  pageNumber = signal<number>(PAGINATION_NUMBERS.DEFAULT_PAGE);
  pageSize = PAGINATION_NUMBERS.PAGE_SIZE;
  totalPages = computed(() => this.serverTotalPages());

  readonly displayedBookings = computed(() => {
    const list = this.bookings();
    if (this.limit && this.limit > 0) {
      return list.slice(CALENDER_NUMBERS.ZERO, this.limit);
    }
    return list;
  });

  constructor(
    private bookingService: BookingService,
    private authService: AuthService,
    private permissionService: PermissionService,
    private router: Router,
  ) {}

  navigateToAllHistory(): void {
    this.router.navigate([AMENITY_ROUTES.BASE, AMENITY_ROUTES.BOOKINGS_SUFX]);
  }

  ngOnInit(): void {
    this.currentRole =
      (this.permissionService.roleName() as Role) ||
      (this.authService.getUserRole() as Role) ||
      APP_CONSTANTS.ROLES.RESIDENT;

    this.isAdmin =
      this.currentRole === APP_CONSTANTS.ROLES.ADMIN ||
      this.permissionService.hasPermission(PERMISSIONS.AMENITY_MANAGE) ||
      this.permissionService.hasPermission(PERMISSIONS.REPORT_VIEW);
    
    this.viewMode.set(this.isAdmin ? VIEW_MODE.ALL : VIEW_MODE.MINE);

    if (this.isAdmin) {
      this.loadBookingAdmin();
    } else {
      this.loadBookingsusers();
    }
  }

  loadBookingsusers(): void {
    const statusVal = this.filterStatus() || undefined;
    const fromDateVal = this.filterFromDate() || undefined;
    const toDateVal = this.filterToDate() || undefined;

    this.bookingService.getApiBooking(
      statusVal,
      fromDateVal,
      toDateVal,
      this.pageNumber(),
      this.pageSize
    ).subscribe({
      next: (response: BookingListResponseDto) => {
        this.bookings.set(response.data ?? []);
        if (response.pagination) {
          this.serverTotalPages.set(response.pagination.totalPages ?? 1);
        } else {
          this.serverTotalPages.set(1);
        }
      },
      error: (err: any) => {
        console.error(BOOKING_HISTORY_STRINGS.ERROR_MSG, err);
        this.bookings.set([]);
      }
    });
  }

  loadBookingAdmin(): void {
    const fromDateVal = this.filterFromDate() || undefined;
    const toDateVal = this.filterToDate() || undefined;

    this.bookingService.getApiBookingReport(
      undefined, // amenityId
      undefined, // slotType
      fromDateVal,
      toDateVal,
      this.pageNumber(),
      this.pageSize
    ).subscribe({
      next: (response: ReportResponseDto) => {
        this.bookings.set(response.bookings ?? []);
        if (response.pagination) {
          this.serverTotalPages.set(response.pagination.totalPages ?? 1);
        } else {
          this.serverTotalPages.set(1);
        }
      },
      error: (err: any) => {
        console.error(BOOKING_HISTORY_STRINGS.ERROR_MSG2, err);
        this.bookings.set([]);
      }
    });
  }

  showMine() {
    this.viewMode.set('mine');
    this.openMenuId.set(null);
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.loadBookingsusers();
  }

  showAll() {
    this.viewMode.set('all');
    this.openMenuId.set(null);
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.loadBookingAdmin();
  }

  goToPage(page: number): void {
    if (page >= CALENDER_NUMBERS.FIRST_DAY && page <= this.totalPages()) {
      this.pageNumber.set(page);
      this.reload();
    }
  }

  onStatusFilterChange(val: string): void {
    this.filterStatus.set(val);
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.loadBookingsusers();
  }

  onFromDateFilterChange(val: string): void {
    this.filterFromDate.set(val);
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.reload();
  }

  onToDateFilterChange(val: string): void {
    this.filterToDate.set(val);
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.reload();
  }

  resetFilters(): void {
    this.filterStatus.set('');
    this.filterFromDate.set('');
    this.filterToDate.set('');
    this.pageNumber.set(PAGINATION_NUMBERS.DEFAULT_PAGE);
    this.reload();
  }

  reload(): void {
    if (this.viewMode() === 'all') {
      this.loadBookingAdmin();
    } else {
      this.loadBookingsusers();
    }
  }

  toggleMenu(bookingId: string | undefined, event: MouseEvent): void {
    event.stopPropagation();
    if (!bookingId) return;
    if (this.openMenuId() === bookingId) {
      this.openMenuId.set(null);
    } else {
      this.openMenuId.set(bookingId);
    }
  }

  closeMenu(): void {
    this.openMenuId.set(null);
  }

  cancelBooking(bookingId: string | undefined): void {
    if (!bookingId) return;
    this.router.navigate([AMENITY_ROUTES.BASE, AMENITY_ROUTES.BOOKINGS_SUFX, bookingId, AMENITY_ROUTES.CANCEL_SUFX]);
  }

  canCancel(status: string | null | undefined): boolean {
    if (!status) return false;
    const s = status.trim().toLowerCase();
    return s === BOOKING_STATUS.BOOKED || s === BOOKING_STATUS.CONFIRMED || s === BOOKING_STATUS.PENDING || s === BOOKING_STATUS.APPROVED || s === BOOKING_STATUS.UPCOMING;
  }

  getStatusClass(status: string | null | undefined): string {
    const s = (status ?? '').trim().toLowerCase();
    if (s === BOOKING_STATUS.CONFIRMED || s === BOOKING_STATUS.APPROVED || s === BOOKING_STATUS.COMPLETED || s === BOOKING_STATUS.SUCCESS) {
      return BADGE_CLASSES.SUCCESS;
    } else if (s === BOOKING_STATUS.PENDING || s === BOOKING_STATUS.IN_PROGRESS || s === BOOKING_STATUS.UPCOMING) {
      return BADGE_CLASSES.INFO;
    } else if (s === BOOKING_STATUS.CANCELLED || s === BOOKING_STATUS.REJECTED) {
      return BADGE_CLASSES.DANGER;
    }
    return BADGE_CLASSES.NEUTRAL;
  }

  getAmenityIcon(name: string | null | undefined): AmenityIconDetails {
    const n = (name ?? '').toLowerCase();
    if (n.includes(AMENITY_ICON_KEYWORDS.POOL)) {
      return AMENITY_ICONS.POOL;
    } else if (AMENITY_ICON_KEYWORDS.GYM.some(k => n.includes(k))) {
      return AMENITY_ICONS.GYM;
    } else if (AMENITY_ICON_KEYWORDS.COURT.some(k => n.includes(k))) {
      return AMENITY_ICONS.COURT;
    } else if (n.includes(AMENITY_ICON_KEYWORDS.CLUBHOUSE)) {
      return AMENITY_ICONS.CLUBHOUSE;
    }
    return AMENITY_ICONS.DEFAULT;
  }

  formatGuests(count: number | undefined): string {
    if (!count || count === 0) return this.historyStrings.NO_GUESTS;
    return count === 1 ? this.historyStrings.ONE_GUEST : `${count}${this.historyStrings.GUESTS_SUFFIX}`;
  }
}