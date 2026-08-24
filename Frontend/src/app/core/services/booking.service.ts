import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import {
  BookingListResponseDto,
  GetBookingReportFilters,
  GetBookingsFilters,
  ReportResponseDto,
} from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly http = inject(HttpClient);

  getMyBookings(filters?: GetBookingsFilters): Observable<BookingListResponseDto> {
    return this.http.get<BookingListResponseDto>(API_CONFIG.BOOKING, {
      params: this.buildParams(filters),
    });
  }

  getBookingReport(filters?: GetBookingReportFilters): Observable<ReportResponseDto> {
    return this.http.get<ReportResponseDto>(API_CONFIG.BOOKING_REPORT, {
      params: this.buildParams(filters),
    });
  }

  private buildParams<T extends object>(filters?: T): HttpParams {
    let params = new HttpParams();
    if (!filters) {
      return params;
    }
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return params;
  }
}
