import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { PagedResult } from '../models/paged-result.model';
import {
  ComplaintCancelRequestDto,
  ComplaintDetailDto,
  ComplaintFilterParams,
  ComplaintStatusUpdateRequestDto,
  ComplaintSummaryDto,
  CreateComplaintRequestDto,
} from '../models/complaint.model';

@Injectable({ providedIn: 'root' })
export class ComplaintService {
  private readonly baseUrl = API_CONFIG.COMPLAINTS;

  constructor(private readonly http: HttpClient) {}

  getComplaints(params: ComplaintFilterParams): Observable<PagedResult<ComplaintSummaryDto>> {
    return this.http.get<PagedResult<ComplaintSummaryDto>>(this.baseUrl, {
      params: this.buildHttpParams(params),
    });
  }

  getById(complaintId: string): Observable<ComplaintDetailDto> {
    return this.http.get<ComplaintDetailDto>(`${this.baseUrl}/${complaintId}`);
  }

  create(payload: CreateComplaintRequestDto): Observable<ComplaintDetailDto> {
    return this.http.post<ComplaintDetailDto>(this.baseUrl, payload);
  }

  updateStatus(
    complaintId: string,
    payload: ComplaintStatusUpdateRequestDto,
  ): Observable<ComplaintDetailDto> {
    return this.http.patch<ComplaintDetailDto>(`${this.baseUrl}/${complaintId}/status`, payload);
  }

  cancel(complaintId: string, payload: ComplaintCancelRequestDto): Observable<ComplaintDetailDto> {
    return this.http.patch<ComplaintDetailDto>(`${this.baseUrl}/${complaintId}/cancel`, payload);
  }

  private buildHttpParams(params: ComplaintFilterParams): HttpParams {
    let httpParams = new HttpParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    });
    return httpParams;
  }
}