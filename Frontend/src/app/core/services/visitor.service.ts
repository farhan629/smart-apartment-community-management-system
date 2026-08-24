import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_CONFIG } from '../config/api.config';
import { SKIP_LOADER } from '../interceptors/loader.interceptor';
import {
  CreateVisitorRequest,
  GetVisitorsResponse,
  RefTermOption,
  UpdateVisitorRequest,
  Visitor,
} from '../models/visitor.model';

@Injectable({ providedIn: 'root' })
export class VisitorService {
  private readonly http = inject(HttpClient);

  getVisitors(params?: {
    page?: number;
    limit?: number;
    search?: string;
    skipLoader?: boolean;
  }): Observable<GetVisitorsResponse> {
    let httpParams = new HttpParams();
    if (params?.page) httpParams = httpParams.set('page', params.page);
    if (params?.limit) httpParams = httpParams.set('limit', params.limit);
    if (params?.search) httpParams = httpParams.set('search', params.search);

    return this.http.get<GetVisitorsResponse>(API_CONFIG.VISITORS, {
      params: httpParams,
      context: new HttpContext().set(SKIP_LOADER, !!params?.skipLoader),
    });
  }

  getVisitorById(id: string): Observable<Visitor> {
    const httpParams = new HttpParams().set('id', id);
    return this.http
      .get<GetVisitorsResponse | Visitor>(API_CONFIG.VISITORS, { params: httpParams })
      .pipe(
        map((response: any) => {
          const visitor = response?.items ? response.items[0] : response;
          if (!visitor || !visitor.id) {
            throw new Error('Visitor not found');
          }
          return visitor as Visitor;
        }),
      );
  }

  createVisitor(request: CreateVisitorRequest): Observable<Visitor> {
    return this.http.post<Visitor>(API_CONFIG.VISITORS, request);
  }

  updateVisitor(id: string, request: UpdateVisitorRequest): Observable<any> {
    const httpParams = new HttpParams().set('id', id);
    return this.http.put(API_CONFIG.VISITORS, request, { params: httpParams });
  }

  uploadVisitorPhoto(id: string, file: File): Observable<Visitor> {
    const formData = new FormData();
    formData.append('photo', file);
    return this.http.post<Visitor>(`${API_CONFIG.VISITORS}/${id}/photo`, formData);
  }

  getVisitorTypes(): Observable<RefTermOption[]> {
    return this.http.get<RefTermOption[]>(API_CONFIG.VISITOR_TYPES);
  }

  getPurposeTypes(): Observable<RefTermOption[]> {
    return this.http.get<RefTermOption[]>(API_CONFIG.PURPOSE_TYPES);
  }
}
