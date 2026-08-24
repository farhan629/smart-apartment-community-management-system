import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import {
  BookingListResponseDto,
  BookingResponseDto,
  BookingService,
} from '../../../../core/services/aminety-service';

@Component({
  selector: 'app-booking-history',
  standalone: true,
  imports: [CommonModule, MatIconModule, StatusBadge],
  templateUrl: './booking-history.html',
  styleUrl: './booking-history.scss',
})
export class BookingHistory implements AfterViewInit {
  private readonly bookingService = inject(BookingService);
  private readonly cdr = inject(ChangeDetectorRef);

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;

  bookings: BookingResponseDto[] = [];
  loading = true;
  error = false;

  ngAfterViewInit(): void {
    this.bookingService.getApiBooking(undefined, undefined, undefined, 1, 5).subscribe({
      next: (res: BookingListResponseDto) => {
        this.bookings = res.data ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.error = true;
        this.cdr.detectChanges();
      },
    });
  }
}
