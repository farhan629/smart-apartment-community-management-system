import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { CategoryLookupDto, RefTermLookupDto } from '../models/lookup.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private readonly baseUrl = API_CONFIG.LOOKUPS;

  private complaintTypes$?: Observable<RefTermLookupDto[]>;
  private complaintPriorities$?: Observable<RefTermLookupDto[]>;
  private complaintStatuses$?: Observable<RefTermLookupDto[]>;
  private categories$?: Observable<CategoryLookupDto[]>;

  constructor(private readonly http: HttpClient) {}

  getComplaintTypes(): Observable<RefTermLookupDto[]> {
    if (!this.complaintTypes$) {
      this.complaintTypes$ = this.http
        .get<RefTermLookupDto[]>(`${this.baseUrl}/complaint-types`)
        .pipe(shareReplay(1));
    }
    return this.complaintTypes$;
  }

  getComplaintPriorities(): Observable<RefTermLookupDto[]> {
    if (!this.complaintPriorities$) {
      this.complaintPriorities$ = this.http
        .get<RefTermLookupDto[]>(`${this.baseUrl}/complaint-priorities`)
        .pipe(shareReplay(1));
    }
    return this.complaintPriorities$;
  }

  getComplaintStatuses(): Observable<RefTermLookupDto[]> {
    if (!this.complaintStatuses$) {
      this.complaintStatuses$ = this.http
        .get<RefTermLookupDto[]>(`${this.baseUrl}/complaint-statuses`)
        .pipe(shareReplay(1));
    }
    return this.complaintStatuses$;
  }

  getCategories(): Observable<CategoryLookupDto[]> {
    if (!this.categories$) {
      this.categories$ = this.http
        .get<CategoryLookupDto[]>(`${this.baseUrl}/categories`)
        .pipe(shareReplay(1));
    }
    return this.categories$;
  }
}
