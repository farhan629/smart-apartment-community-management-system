import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, shareReplay, map } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import {
  MANAGEMENT_ROLE_NAMES,
  USER_LOOKUP_DEFAULTS,
  USER_QUERY_PARAM,
} from '../constants/user-lookup.constants';
import { IdentityPagedResult, RoleLookupDto, UserLookupDto } from '../models/user-lookup.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly usersUrl = API_CONFIG.USERS;
  private readonly roleManagementUrl = API_CONFIG.ROLE_MANAGEMENT;
  private managementRoles$?: Observable<RoleLookupDto[]>;

  constructor(private readonly http: HttpClient) {}

  getManagementRoles(): Observable<RoleLookupDto[]> {
    if (!this.managementRoles$) {
      this.managementRoles$ = this.http
        .get<RoleLookupDto[]>(this.roleManagementUrl)
        .pipe(shareReplay(1));
    }
    return this.managementRoles$;
  }

  getStaffRoleId(): Observable<string | undefined> {
    return this.getManagementRoles().pipe(
      map((roles) => roles.find((role) => role.termValue === MANAGEMENT_ROLE_NAMES.STAFF)?.id),
    );
  }

  getUsersByRole(
    roleId: string,
    limit: number = USER_LOOKUP_DEFAULTS.ROLE_USERS_LIMIT,
  ): Observable<UserLookupDto[]> {
    const params = new HttpParams()
      .set(USER_QUERY_PARAM.ROLE_ID, roleId)
      .set(USER_QUERY_PARAM.LIMIT, limit);

    return this.http
      .get<IdentityPagedResult<UserLookupDto>>(this.usersUrl, { params })
      .pipe(map((result) => result.items));
  }

  getUserById(userId: string): Observable<UserLookupDto> {
    return this.http.get<UserLookupDto>(`${this.usersUrl}/${userId}`);
  }

  getUsersCountByRole(roleId: string): Observable<number> {
    const params = new HttpParams()
      .set(USER_QUERY_PARAM.ROLE_ID, roleId)
      .set(USER_QUERY_PARAM.LIMIT, 1);

    return this.http
      .get<IdentityPagedResult<UserLookupDto>>(this.usersUrl, { params })
      .pipe(map((result) => result.total));
  }
}
