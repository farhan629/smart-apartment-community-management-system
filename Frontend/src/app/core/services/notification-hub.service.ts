import { Injectable, inject, signal, OnDestroy } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth-service';
import { NotificationDto } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationHubService implements OnDestroy {
  private readonly authService = inject(AuthService);
  private hubConnection: signalR.HubConnection | null = null;
  private readonly notificationSubject = new Subject<NotificationDto>();

  readonly connected = signal(false);
  readonly onNotification$: Observable<NotificationDto> = this.notificationSubject.asObservable();

  connect(): void {
    const token = this.authService.getAccessToken();
    if (!token) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiBaseUrl}/notification-hub`, {
        accessTokenFactory: () => token,
        withCredentials: true,
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
      this.notificationSubject.next(notification);
    });

    this.hubConnection.onreconnecting(() => {
      this.connected.set(false);
    });

    this.hubConnection.onreconnected(() => {
      this.connected.set(true);
    });

    this.hubConnection.onclose(() => {
      this.connected.set(false);
    });

    this.hubConnection.start().then(() => {
      this.connected.set(true);
    }).catch((err) => {
      console.error('SignalR connection failed:', err);
      this.connected.set(false);
    });
  }

  disconnect(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }
    this.connected.set(false);
  }

  ngOnDestroy(): void {
    this.notificationSubject.complete();
    this.disconnect();
  }
}
