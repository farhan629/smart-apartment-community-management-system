import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { PagedResult } from '../models/paged-result.model';
import { StaffResponseDto, StaffSummaryDto } from '../models/staff.model';

@Injectable({ providedIn: 'root' })
export class StaffService {
  private readonly baseUrl = API_CONFIG.STAFF;

  constructor(private readonly http: HttpClient) {}

  getStaffList(page: number, limit: number): Observable<PagedResult<StaffSummaryDto>> {
    return this.http.get<PagedResult<StaffSummaryDto>>(this.baseUrl, {
      params: { page, limit },
    });
  }

  getStaffById(staffId: string): Observable<StaffResponseDto> {
    return this.http.get<StaffResponseDto>(`${this.baseUrl}/${staffId}`);
  }
}
