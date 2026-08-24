import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

import { 
  SlotService, 
  BookingService, 
  AvailableSlotResponseDto, 
  AmenityResponseDto,
  AvailableSlotsResponseDto
} from '../../../../core/services/aminety-service';
import { AmenityDownbar } from '../../components/amenity-downbar/amenity-downbar';
import { AmenityCalender } from '../../components/amenity-calender/amenity-calender';
import { AmenityBooking } from '../../components/amenity-booking/amenity-booking';
import { AmenityActionDialog, DialogBookingDetails } from '../../components/amenity-action-dialog/amenity-action-dialog';
import { AMENITY_BOOKING_PAGE_STRINGS, AMENITY_ROUTES, AMENITY_DEFAULTS, BOOKING_PAGE_NUMBERS, SLOT_HOURS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-booking-page',
  standalone: true,
  imports: [CommonModule, AmenityDownbar, AmenityCalender, AmenityBooking, AmenityActionDialog, MatSnackBarModule],
  templateUrl: './amenity-booking-page.html',
  styleUrl: './amenity-booking-page.scss',
})
export class AmenityBookingPage implements OnInit {
  amenityId!: string;
  amenityName = signal<string>('');
  amenityDetails = signal<AmenityResponseDto | null>(null);

  selectedDate: Date = new Date();
  allSlots: AvailableSlotResponseDto[] = [];
  morningSlots: AvailableSlotResponseDto[] = [];
  afternoonSlots: AvailableSlotResponseDto[] = [];
  eveningSlots: AvailableSlotResponseDto[] = [];
  selectedSlot: AvailableSlotResponseDto | null = null;
  loadingSlots = signal<boolean>(false);

  pageStrings = AMENITY_BOOKING_PAGE_STRINGS;

  showConfirmDialog = signal<boolean>(false);
  isSubmittingConfirm = signal<boolean>(false);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private slotService: SlotService,
    private bookingService: BookingService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any) => {
      const id = params.get('amenityId');
      if (id) {
        this.amenityId = id;
        this.loadSlots();
      } else {
        this.router.navigate([AMENITY_ROUTES.BASE]);
      }
    });
  }

  onDateSelected(newDate: Date): void {
    this.selectedDate = newDate;
    this.selectedSlot = null;
    this.loadSlots();
  }

  onSlotSelected(slot: AvailableSlotResponseDto): void {
    this.selectedSlot = slot;
  }

  loadSlots(): void {
    this.loadingSlots.set(true);
    const tzOffset = this.selectedDate.getTimezoneOffset() * BOOKING_PAGE_NUMBERS.TZ_MS_MULTIPLIER;
    const localISODate = (new Date(this.selectedDate.getTime() - tzOffset)).toISOString().split('T')[0];

    this.slotService.getApiAmenitiesSlotsAvailable(this.amenityId, localISODate).subscribe({
      next: (response: AvailableSlotsResponseDto) => {
        this.allSlots = response.slots ?? [];
        this.categorizeSlots(this.allSlots);

        this.amenityName.set(response.amenityName ?? AMENITY_DEFAULTS.NAME);
        this.amenityDetails.set({
          id: response.amenityId,
          name: response.amenityName,
          slotType: response.slotType,
          location: response.location,
          rules: response.rules,
          imageUrl: response.imageUrl
        });
        this.loadingSlots.set(false);
      },
      error: (err: any) => {
        console.error('Error loading slots', err);
        this.allSlots = [];
        this.categorizeSlots([]);
        this.loadingSlots.set(false);
      }
    });
  }

  categorizeSlots(slots: AvailableSlotResponseDto[]): void {
    this.morningSlots = [];
    this.afternoonSlots = [];
    this.eveningSlots = [];

    slots.forEach(slot => {
      const startHourStr = (slot.startTime ?? '00:00').split(':')[0];
      const startHour = parseInt(startHourStr, SLOT_HOURS.RADIX_DECIMAL);

      if (startHour >= SLOT_HOURS.MORNING_START && startHour < SLOT_HOURS.AFTERNOON_START) {
        this.morningSlots.push(slot);
      } else if (startHour >= SLOT_HOURS.AFTERNOON_START && startHour < SLOT_HOURS.EVENING_START) {
        this.afternoonSlots.push(slot);
      } else {
        this.eveningSlots.push(slot);
      }
    });
  }

  cancelSelection(): void {
    this.selectedSlot = null;
  }

  confirmBooking(): void {
    if (!this.selectedSlot?.slotId) return;
    this.showConfirmDialog.set(true);
  }

  executeBookingConfirm(): void {
    if (!this.selectedSlot?.slotId) return;
    this.isSubmittingConfirm.set(true);

    this.bookingService.postApiBooking({
      slotId: this.selectedSlot.slotId,
      peopleCount: BOOKING_PAGE_NUMBERS.DEFAULT_PEOPLE_COUNT
    }).subscribe({
      next: () => {
        this.showConfirmDialog.set(false);
        this.isSubmittingConfirm.set(false);
        this.snackBar.open(this.pageStrings.TOAST_CONFIRMED, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        this.router.navigate([AMENITY_ROUTES.BASE]);
      },
      error: (err: any) => {
        console.error('Error confirming booking', err);
        alert(this.pageStrings.BOOKING_FAILED);
        this.isSubmittingConfirm.set(false);
      }
    });
  }

  get bookingDetailsForDialog(): DialogBookingDetails {
    return {
      amenityName: this.amenityName(),
      slotDate: this.selectedDate,
      slotLabel: this.selectedSlot?.slotLabel ?? ''
    };
  }
}
