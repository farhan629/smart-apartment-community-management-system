import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, of, throwError } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import {
  EscalationDto,
  ReEscalateRequestDto,
  ReEscalateResponseDto,
  UpdateEscalationRequestDto,
} from '../models/escalation.model';

@Injectable({ providedIn: 'root' })
export class EscalationService {
  private readonly baseUrl = API_CONFIG.ESCALATIONS;

  constructor(private readonly http: HttpClient) {}

  // A 404 here just means the complaint hasn't been escalated yet, not a
  // real error — normalize it to `null` so the component can tell the two
  // cases apart.
  getEscalation(complaintId: string): Observable<EscalationDto | null> {
    return this.http.get<EscalationDto>(`${this.baseUrl}/${complaintId}`).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === 404) {
          return of(null);
        }
        return throwError(() => err);
      }),
    );
  }

  reEscalate(
    complaintId: string,
    payload: ReEscalateRequestDto,
  ): Observable<ReEscalateResponseDto> {
    return this.http.post<ReEscalateResponseDto>(`${this.baseUrl}/${complaintId}`, payload);
  }

  updateEscalation(
    complaintId: string,
    payload: UpdateEscalationRequestDto,
  ): Observable<EscalationDto> {
    return this.http.put<EscalationDto>(`${this.baseUrl}/${complaintId}`, payload);
  }
}
