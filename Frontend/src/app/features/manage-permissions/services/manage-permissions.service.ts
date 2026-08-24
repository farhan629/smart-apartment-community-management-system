import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  UserSearchResultDto,
  PaginatedUserResponse,
  UserPermissionResponseDto,
  AssignPermissionsRequestDto,
} from '../../../core/models/manage-permissions.models';

@Injectable({ providedIn: 'root' })
export class ManagePermissionsService {
  private readonly http = inject(HttpClient);

  searchUsers(searchName: string): Observable<UserSearchResultDto[]> {
    return this.http
      .get<PaginatedUserResponse>(
        `${API_CONFIG.USERS}?name=${encodeURIComponent(searchName)}`,
      )
      .pipe(map((res) => res.items));
  }

  getUserPermissions(userId: string): Observable<UserPermissionResponseDto> {
    return this.http.get<UserPermissionResponseDto>(
      `${API_CONFIG.PERMISSIONS}/${userId}`,
    );
  }

  assignPermissions(request: AssignPermissionsRequestDto): Observable<void> {
    return this.http.put<void>(`${API_CONFIG.PERMISSIONS}/assign`, request);
  }
}