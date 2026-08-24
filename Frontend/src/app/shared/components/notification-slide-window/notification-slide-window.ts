import { Component, inject, output, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Subject, takeUntil } from 'rxjs';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';
import { NotificationService } from '../../../core/services/notification.service';
import { NotificationHubService } from '../../../core/services/notification-hub.service';
import { NotificationDto } from '../../../core/models/notification.models';

@Component({
  selector: 'app-notification-slide-window',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './notification-slide-window.html',
  styleUrl: './notification-slide-window.scss',
})
export class NotificationSlideWindow implements OnInit, OnDestroy {
  private readonly notificationService = inject(NotificationService);
  private readonly hubService = inject(NotificationHubService);
  private readonly destroy$ = new Subject<void>();

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly pagination = APP_CONSTANTS.PAGINATION;
  readonly closed = output<void>();

  readonly notifications = signal<NotificationDto[]>([]);
  readonly loading = signal(false);

  ngOnInit(): void {
    this.loadNotifications();

    this.hubService.onNotification$.pipe(takeUntil(this.destroy$)).subscribe((notif) => {
      this.notifications.update((list) => [notif, ...list]);
    });
  }

  private loadNotifications(): void {
    this.loading.set(true);
    this.notificationService.getNotifications().subscribe({
      next: (res) => {
        this.notifications.set(res.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  onClose(): void {
    this.closed.emit();
  }

  onMarkAsRead(notification: NotificationDto): void {
    if (notification.isRead) {
      return;
    }
    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => {
        this.notifications.update((list) =>
          list.map((n) => (n.id === notification.id ? { ...n, isRead: true } : n))
        );
      },
    });
  }

  onMarkAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.update((list) => list.map((n) => ({ ...n, isRead: true })));
      },
    });
  }

  onDelete(notification: NotificationDto): void {
    this.notificationService.delete(notification.id).subscribe({
      next: () => {
        this.notifications.update((list) => list.filter((n) => n.id !== notification.id));
      },
    });
  }

  onDeleteAll(): void {
    this.notificationService.deleteAll().subscribe({
      next: () => {
        this.notifications.set([]);
      },
    });
  }

  get unreadCount(): number {
    return this.notifications().filter((n) => !n.isRead).length;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
