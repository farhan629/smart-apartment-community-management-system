import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { APP_CONSTANTS } from '../constants/app.constants';
import { SKIP_LOADER } from '../interceptors/loader.interceptor';
import { NotificationDto, GetNotificationsResponse, MarkAllReadResponse, DeleteAllResponse } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/notification`;

  getNotifications(isRead?: boolean, page = APP_CONSTANTS.PAGINATION.DEFAULT_PAGE, limit = APP_CONSTANTS.PAGINATION.NOTIFICATION_PAGE_LIMIT, skipLoader = false): Observable<GetNotificationsResponse> {
    let params = `page=${page}&limit=${limit}`;
    if (isRead !== undefined) {
      params += `&isRead=${isRead}`;
    }
    const options = skipLoader ? { context: new HttpContext().set(SKIP_LOADER, true) } : {};
    return this.http.get<GetNotificationsResponse>(`${this.baseUrl}?${params}`, options);
  }

  markAsRead(id: string): Observable<NotificationDto> {
    return this.http.put<NotificationDto>(`${this.baseUrl}/${id}`, {});
  }

  markAllAsRead(): Observable<MarkAllReadResponse> {
    return this.http.put<MarkAllReadResponse>(`${this.baseUrl}/mark-all-read`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  deleteAll(): Observable<DeleteAllResponse> {
    return this.http.delete<DeleteAllResponse>(`${this.baseUrl}/delete-all`);
  }
}
