import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_CONFIG } from '../config/api.config';
import {
  Visit,
  CreateVisitRequest,
  UpdateVisitRequest,
  VisitCreateResponse,
  GetVisitsResponse,
  GetVisitsFilters,
  RejectVisitRequestDto,
} from '../models/visit.model';

@Injectable({ providedIn: 'root' })
export class VisitService {
  private readonly http = inject(HttpClient);

  getVisits(filters?: GetVisitsFilters): Observable<GetVisitsResponse> {
    let httpParams = new HttpParams();
    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          httpParams = httpParams.set(key, value as string);
        }
      });
    }

    return this.http.get<GetVisitsResponse>(API_CONFIG.VISITS, {
      params: httpParams,
    });
  }

 getVisitById(id: string): Observable<Visit> {
  const httpParams = new HttpParams().set('id', id);
  return this.http.get<GetVisitsResponse | Visit>(API_CONFIG.VISITS, { params: httpParams }).pipe(
    map((response: any) => {
      const visit = response?.items ? response.items[0] : response;
      if (!visit || !visit.id) {
        throw new Error('Visit not found');
      }
      return visit as Visit;
    }),
  );
}
  createVisit(request: CreateVisitRequest): Observable<VisitCreateResponse> {
    return this.http.post<VisitCreateResponse>(API_CONFIG.VISITS, request);
  }

  updateVisit(id: string, request: UpdateVisitRequest): Observable<Visit> {
    const httpParams = new HttpParams().set('id', id);
    return this.http.put<Visit>(API_CONFIG.VISITS, request, {
      params: httpParams,
    });
  }

  cancelVisit(id: string): Observable<void> {
    const httpParams = new HttpParams().set('id', id);
    return this.http.delete<void>(API_CONFIG.VISITS, { params: httpParams });
  }

  checkInByToken(token: string): Observable<void> {
    const formData = new FormData();
    formData.append('token', token);
    return this.http.post<void>(API_CONFIG.VISIT_CHECKIN, formData);
  }

  approveVisit(id: string): Observable<Visit> {
    return this.http.patch<Visit>(`${API_CONFIG.VISITS}/${id}/approve`, {});
  }

  rejectVisit(id: string, request: RejectVisitRequestDto): Observable<Visit> {
    return this.http.patch<Visit>(`${API_CONFIG.VISITS}/${id}/reject`, request);
  }

  checkOutByToken(token: string): Observable<void> {
    const formData = new FormData();
    formData.append('token', token);
    return this.http.post<void>(API_CONFIG.VISIT_CHECKOUT, formData);
  }
}