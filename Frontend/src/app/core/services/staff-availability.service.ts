import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import {
  AvailabilitySlotDto,
  CreateAvailabilityRequestDto,
  StaffAvailabilityFilterParams,
} from '../models/staff-availability.model';

@Injectable({ providedIn: 'root' })
export class StaffAvailabilityService {
  private readonly baseUrl = API_CONFIG.STAFF_AVAILABILITY;

  constructor(private readonly http: HttpClient) {}

  getSlots(params: StaffAvailabilityFilterParams): Observable<AvailabilitySlotDto[]> {
    return this.http.get<AvailabilitySlotDto[]>(this.baseUrl, {
      params: this.buildHttpParams(params),
    });
  }

  createSlots(
    staffId: string,
    payload: CreateAvailabilityRequestDto,
  ): Observable<AvailabilitySlotDto[]> {
    return this.http.post<AvailabilitySlotDto[]>(this.baseUrl, payload, {
      params: { staffId },
    });
  }

  deleteSlot(staffId: string, slotId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${slotId}`, {
      params: { staffId },
    });
  }

  private buildHttpParams(params: StaffAvailabilityFilterParams): HttpParams {
    let httpParams = new HttpParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    });
    return httpParams;
  }
}
