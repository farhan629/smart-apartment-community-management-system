import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { UserPermissionsDto } from '../models/permission.model';
import { Permission } from '../constants/permission.constants';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  private readonly http = inject(HttpClient);

  private readonly _permissions = signal<string[]>([]);
  private readonly _roleName = signal<string>('');
  private readonly _userName = signal<string>('');
  private readonly _userId = signal<string>('');
  private readonly _loaded = signal<boolean>(false);

  readonly roleName = this._roleName.asReadonly();
  readonly userName = this._userName.asReadonly();
  readonly userId = this._userId.asReadonly();
  readonly loaded = this._loaded.asReadonly();

  hasPermission(permission: Permission): boolean {
    return this._permissions().includes(permission);
  }

  hasAnyPermission(permissions: Permission[]): boolean {
    return permissions.some((p) => this.hasPermission(p));
  }

  load(): Observable<UserPermissionsDto | null> {
    return this.http.get<UserPermissionsDto>(API_CONFIG.PERMISSIONS_ME).pipe(
      tap((res) => {
        this._permissions.set(res.permissions ?? []);
        this._roleName.set(res.roleName ?? '');
        this._userName.set(res.userName ?? '');
        this._userId.set(res.userId ?? '');
        this._loaded.set(true);
      }),
      catchError(() => {
        this.clear();
        return of(null);
      }),
    );
  }

  clear(): void {
    this._permissions.set([]);
    this._roleName.set('');
    this._userName.set('');
    this._userId.set('');
    this._loaded.set(false);
  }
}
