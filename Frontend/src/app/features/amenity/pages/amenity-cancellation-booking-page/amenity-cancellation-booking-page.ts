import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { BookingService, BookingResponseDto, BookingListResponseDto, ReportResponseDto } from '../../../../core/services/aminety-service';
import { AmenityActionDialog, DialogBookingDetails } from '../../components/amenity-action-dialog/amenity-action-dialog';
import { CANCELLATION_STRINGS, AMENITY_ROUTES } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-cancellation-booking-page',
  standalone: true,
  imports: [CommonModule, AmenityActionDialog, MatSnackBarModule],
  templateUrl: './amenity-cancellation-booking-page.html',
  styleUrl: './amenity-cancellation-booking-page.scss',
})
export class AmenityCancellationBookingPage implements OnInit {
  bookingId!: string;
  booking = signal<BookingResponseDto | null>(null);
  loading = signal<boolean>(true);
  error = signal<boolean>(false);
  isSubmitting = signal<boolean>(false);
  pageStrings = CANCELLATION_STRINGS;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: BookingService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any) => {
      const id = params.get('bookingId');
      if (id) {
        this.bookingId = id;
        this.loadBookingDetails();
      } else {
        this.goBack();
      }
    });
  }

  loadBookingDetails(): void {
    this.loading.set(true);
    this.error.set(false);

    this.bookingService.getApiBooking().subscribe({
      next: (res: BookingListResponseDto) => {
        const found = res.data?.find(b => b.bookingId === this.bookingId);
        if (found) {
          this.booking.set(found);
          this.loading.set(false);
        } else {
          this.bookingService.getApiBookingReport().subscribe({
            next: (adminRes: ReportResponseDto) => {
              const adminFound = adminRes.bookings?.find(b => b.bookingId === this.bookingId);
              if (adminFound) {
                this.booking.set(adminFound);
              } else {
                this.error.set(true);
              }
              this.loading.set(false);
            },
            error: () => {
              this.error.set(true);
              this.loading.set(false);
            }
          });
        }
      },
      error: () => {
        this.bookingService.getApiBookingReport().subscribe({
          next: (adminRes: ReportResponseDto) => {
            const adminFound = adminRes.bookings?.find(b => b.bookingId === this.bookingId);
            if (adminFound) {
              this.booking.set(adminFound);
            } else {
              this.error.set(true);
            }
            this.loading.set(false);
          },
          error: () => {
            this.error.set(true);
            this.loading.set(false);
          }
        });
      }
    });
  }

  onConfirmCancel(reason: string): void {
    if (!confirm(this.pageStrings.CONFIRM_ALERT)) {
      return;
    }
    this.isSubmitting.set(true);

    this.bookingService.deleteApiBooking(this.bookingId, reason).subscribe({
      next: () => {
        this.snackBar.open(this.pageStrings.TOAST_SUCCESS, 'Close', {
          duration: 3000,
          horizontalPosition: 'right',
          verticalPosition: 'bottom',
          panelClass: ['success-snackbar']
        });
        this.goBack();
      },
      error: (err: any) => {
        console.error('Error confirming cancellation', err);
        alert(this.pageStrings.ERROR_SUBMIT);
        this.isSubmitting.set(false);
      }
    });
  }

  get bookingDetailsForDialog(): DialogBookingDetails {
    const item = this.booking()!;
    return {
      amenityName: item.amenityName ?? '',
      slotDate: item.slotDate ?? '',
      slotLabel: item.slotLabel ?? '',
      status: item.status ?? ''
    };
  }

  goBack(): void {
    this.router.navigate([AMENITY_ROUTES.BASE, AMENITY_ROUTES.BOOKINGS_SUFX]);
  }
}
