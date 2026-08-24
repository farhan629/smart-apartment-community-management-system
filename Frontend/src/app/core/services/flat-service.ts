import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AUTH_ENDPOINTS } from '../constants/auth.constants';
import { FlatItemDto, FlatResponseDto } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class FlatService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  getFlats(pageNumber = 1, pageSize = 100): Observable<FlatResponseDto> {
    return this.http.get<FlatResponseDto>(
      `${this.baseUrl}${AUTH_ENDPOINTS.FLATS}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  getFlatById(id: string): Observable<FlatItemDto> {
    return this.http.get<FlatItemDto>(`${this.baseUrl}${AUTH_ENDPOINTS.FLATS}/${id}`);
  }
}
