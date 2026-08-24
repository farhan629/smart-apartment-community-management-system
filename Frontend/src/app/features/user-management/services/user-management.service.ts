import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api.config';
import {
  UserListItem,
  UserDetailDto,
  UpdateUserRequestDto,
  ManagementRoleDto,
  CategoryDto,
  UploadResponseDto,
  RoleOptionDto,
} from '../../../core/models/user-management.models';
import {
  UserPermissionResponseDto,
  AssignPermissionsRequestDto,
  PaginatedUserResponse,
} from '../../../core/models/manage-permissions.models';
import { RegisterManagementRequestDto, OccupantRoleDto } from '../../../core/models/auth.models';

@Injectable({ providedIn: 'root' })
export class UserManagementService {
  private readonly http = inject(HttpClient);

  getUsers(page: number = 1, limit: number = 10, searchTerm?: string, roleId?: string): Observable<PaginatedUserResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    if (searchTerm) {
      params = params.set('name', searchTerm);
    }
    if (roleId) {
      params = params.set('roleId', roleId);
    }

    return this.http.get<PaginatedUserResponse>(`${API_CONFIG.USERS}`, { params });
  }

  getUserById(id: string): Observable<UserDetailDto> {
    return this.http.get<UserDetailDto>(`${API_CONFIG.USERS}/${id}`);
  }

  updateUser(id: string, request: UpdateUserRequestDto): Observable<void> {
    return this.http.put<void>(`${API_CONFIG.USERS}/${id}`, request);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${API_CONFIG.USERS}/${id}`);
  }

  getManagementRoles(): Observable<ManagementRoleDto[]> {
    return this.http.get<ManagementRoleDto[]>(`${API_CONFIG.ROLE_MANAGEMENT}`);
  }

  getOccupantRoles(): Observable<OccupantRoleDto[]> {
    return this.http.get<OccupantRoleDto[]>(`${API_CONFIG.ROLE_OCCUPANT}`);
  }

  getAllRoles(): Observable<RoleOptionDto[]> {
    const occupant$ = this.http.get<OccupantRoleDto[]>(`${API_CONFIG.ROLE_OCCUPANT}`);
    const management$ = this.http.get<ManagementRoleDto[]>(`${API_CONFIG.ROLE_MANAGEMENT}`);
    return new Observable<RoleOptionDto[]>((subscriber) => {
      occupant$.subscribe({
        next: (occupant) => {
          management$.subscribe({
            next: (management) => {
              subscriber.next([...occupant, ...management]);
              subscriber.complete();
            },
            error: () => {
              subscriber.next([...occupant]);
              subscriber.complete();
            },
          });
        },
        error: () => {
          management$.subscribe({
            next: (management) => {
              subscriber.next([...management]);
              subscriber.complete();
            },
            error: () => {
              subscriber.next([]);
              subscriber.complete();
            },
          });
        },
      });
    });
  }

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${API_CONFIG.LOOKUP_CATEGORIES}`);
  }

  uploadImage(file: File): Observable<UploadResponseDto> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UploadResponseDto>(`${API_CONFIG.AMENITY_UPLOAD}`, formData);
  }

  registerUser(request: RegisterManagementRequestDto): Observable<void> {
    const formData = new FormData();
    formData.append('UserName', request.userName);
    formData.append('Email', request.email);
    formData.append('Password', request.password);
    formData.append('Phone', request.phone);
    formData.append('Role_id', request.role_id);
    if (request.category_id) {
      formData.append('Category_id', request.category_id);
    }
    if (request.photo) {
      formData.append('Photo', request.photo);
    }
    return this.http.post<void>(`${API_CONFIG.AUTH.REGISTER}`, formData);
  }

  resolvePhotoUrl(path: string): string {
    if (path && path.startsWith('/uploads/')) {
      return `${API_CONFIG.GATEWAY}/gateway${path}`;
    }
    return path;
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
