import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { BookingResponseDto } from '../../../../core/services/aminety-service';

@Component({
  selector: 'app-booking-report',
  standalone: true,
  imports: [CommonModule, MatIconModule, StatusBadge],
  templateUrl: './booking-report.html',
  styleUrl: './booking-report.scss',
})
export class BookingReport {
  @Input() bookings: BookingResponseDto[] = [];
  @Input() loading = false;
  @Input() error = false;

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
}
