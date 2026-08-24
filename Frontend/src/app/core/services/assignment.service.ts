import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { PagedResult } from '../models/paged-result.model';
import { ASSIGNMENT_QUERY_PARAM } from '../constants/complaint.constants';
import {
  AssignComplaintRequestDto,
  AssignmentResponseDto,
  DenyAssignmentRequestDto,
  ResidentFlatResponseDto,
} from '../models/assignment.model';

@Injectable({ providedIn: 'root' })
export class AssignmentService {
  private readonly complaintsUrl = API_CONFIG.COMPLAINTS;

  constructor(private readonly http: HttpClient) {}

  getMyAssignments(page: number, limit: number): Observable<PagedResult<AssignmentResponseDto>> {
    return this.http.get<PagedResult<AssignmentResponseDto>>(API_CONFIG.STAFF_ASSIGNMENTS, {
      params: { page, limit },
    });
  }

  getHistory(complaintId: string): Observable<AssignmentResponseDto[]> {
    return this.http.get<AssignmentResponseDto[]>(
      `${this.complaintsUrl}/${complaintId}/assignments`,
    );
  }

  getResidentFlat(complaintId: string, assignmentId?: string): Observable<ResidentFlatResponseDto> {
    let params = new HttpParams();
    if (assignmentId) {
      params = params.set(ASSIGNMENT_QUERY_PARAM.ASSIGNMENT_ID, assignmentId);
    }
    return this.http.get<ResidentFlatResponseDto>(
      `${this.complaintsUrl}/${complaintId}/assignments/resident-flat`,
      { params },
    );
  }

  assign(
    complaintId: string,
    payload: AssignComplaintRequestDto,
  ): Observable<AssignmentResponseDto> {
    return this.http.post<AssignmentResponseDto>(
      `${this.complaintsUrl}/${complaintId}/assign`,
      payload,
    );
  }

  accept(complaintId: string, assignmentId: string): Observable<AssignmentResponseDto> {
    return this.http.patch<AssignmentResponseDto>(
      `${this.complaintsUrl}/${complaintId}/assignments/${assignmentId}/accept`,
      {},
    );
  }

  deny(
    complaintId: string,
    assignmentId: string,
    payload: DenyAssignmentRequestDto,
  ): Observable<AssignmentResponseDto> {
    return this.http.patch<AssignmentResponseDto>(
      `${this.complaintsUrl}/${complaintId}/assignments/${assignmentId}/deny`,
      payload,
    );
  }

  reassign(
    complaintId: string,
    assignmentId: string,
    payload: AssignComplaintRequestDto,
  ): Observable<AssignmentResponseDto> {
    return this.http.patch<AssignmentResponseDto>(
      `${this.complaintsUrl}/${complaintId}/assignments/${assignmentId}/reassign`,
      payload,
    );
  }
}
