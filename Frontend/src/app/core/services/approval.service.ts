import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api.config';
import {
  PaginatedApprovalResponseDto,
  UpdateApprovalRequestDto,
  UpdateApprovalResponseDto,
} from '../models/approval.models';

@Injectable({ providedIn: 'root' })
export class ApprovalService {
  private readonly http = inject(HttpClient);

  getApprovals(
    status?: string,
    page: number = 1,
    limit: number = 50
  ): Observable<PaginatedApprovalResponseDto> {
    let url = `${API_CONFIG.APPROVALS}?page=${page}&limit=${limit}`;
    if (status) {
      url += `&status=${encodeURIComponent(status)}`;
    }
    return this.http.get<PaginatedApprovalResponseDto>(url);
  }

  updateApproval(
    id: string,
    request: UpdateApprovalRequestDto
  ): Observable<UpdateApprovalResponseDto> {
    return this.http.put<UpdateApprovalResponseDto>(
      `${API_CONFIG.APPROVALS}/${id}`,
      request
    );
  }
}
