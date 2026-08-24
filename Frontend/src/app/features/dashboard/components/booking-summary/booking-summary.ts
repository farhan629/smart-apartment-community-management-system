import { Component, Input } from '@angular/core';

import { ReportSummaryDto } from '../../../../core/services/aminety-service';

interface Segment {
  label: string;
  color: string;
  dash: number;
  offset: number;
}

@Component({
  selector: 'app-booking-summary',
  standalone: true,
  imports: [],
  templateUrl: './booking-summary.html',
  styleUrl: './booking-summary.scss',
})
export class BookingSummary {
  @Input() summary: ReportSummaryDto | null = null;

  readonly circumference = 2 * Math.PI * 40;

  get segments(): Segment[] {
    const s = this.summary;
    if (!s) return [];

    const total = s.totalBookings ?? 1;
    const active = ((s.activeBookings ?? 0) / total) * this.circumference;
    const cancelled = ((s.cancelledBookings ?? 0) / total) * this.circumference;
    const completed = ((s.completedBookings ?? 0) / total) * this.circumference;

    return [
      { label: 'Active', color: '#4caf50', dash: active, offset: 0 },
      { label: 'Cancelled', color: '#f44336', dash: cancelled, offset: -active },
      { label: 'Completed', color: '#2196f3', dash: completed, offset: -(active + cancelled) },
    ];
  }
}
