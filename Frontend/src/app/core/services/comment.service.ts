import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { COMPLAINT_QUERY_PARAM, COMPLAINT_SUB_RESOURCE } from '../constants/complaint.constants';
import { STAFF_QUERY_PARAM, STAFF_SUB_RESOURCE } from '../constants/staff.constants';
import { CommentDto, CreateCommentRequestDto, ProgressLogEntryDto } from '../models/comment.model';

@Injectable({ providedIn: 'root' })
export class CommentService {
  private readonly complaintsUrl = API_CONFIG.COMPLAINTS;
  private readonly staffUrl = API_CONFIG.STAFF;

  constructor(private readonly http: HttpClient) {}

  getComments(complaintId: string): Observable<CommentDto[]> {
    return this.http.get<CommentDto[]>(
      `${this.complaintsUrl}/${complaintId}/${COMPLAINT_SUB_RESOURCE.COMMENTS}`,
      { params: this.complaintIdParam(complaintId) },
    );
  }

  addComment(complaintId: string, payload: CreateCommentRequestDto): Observable<CommentDto> {
    return this.http.post<CommentDto>(
      `${this.complaintsUrl}/${complaintId}/${COMPLAINT_SUB_RESOURCE.COMMENTS}`,
      payload,
      { params: this.complaintIdParam(complaintId) },
    );
  }

  getStaffComments(staffId: string): Observable<CommentDto[]> {
    return this.http.get<CommentDto[]>(
      `${this.staffUrl}/${staffId}/${STAFF_SUB_RESOURCE.COMMENTS}`,
      { params: new HttpParams().set(STAFF_QUERY_PARAM.STAFF_ID, staffId) },
    );
  }

  getProgressLog(complaintId: string): Observable<ProgressLogEntryDto[]> {
    return this.http.get<ProgressLogEntryDto[]>(
      `${this.complaintsUrl}/${complaintId}/${COMPLAINT_SUB_RESOURCE.PROGRESS_LOG}`,
      { params: this.complaintIdParam(complaintId) },
    );
  }

  private complaintIdParam(complaintId: string): HttpParams {
    return new HttpParams().set(COMPLAINT_QUERY_PARAM.COMPLAINT_ID, complaintId);
  }
}
