import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AUTH_ENDPOINTS } from '../constants/auth.constants';
import { OccupantRoleDto } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  getOccupantRoles(): Observable<OccupantRoleDto[]> {
    return this.http.get<OccupantRoleDto[]>(
      `${this.baseUrl}${AUTH_ENDPOINTS.ROLE_OCCUPANT}`
    );
  }
}
