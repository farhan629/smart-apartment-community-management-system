import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Subject, interval, takeUntil } from 'rxjs';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';
import { API_CONFIG } from '../../../core/config/api.config';
import { AuthService } from '../../../core/services/auth-service';
import { UserService } from '../../../core/services/user.service';
import { PermissionService } from '../../../core/services/permission.service';
import { NotificationService } from '../../../core/services/notification.service';
import { NotificationHubService } from '../../../core/services/notification-hub.service';
import { NotificationSlideWindow } from '../notification-slide-window/notification-slide-window';
import { ActionButton } from '../action-button/action-button';
import { ConfirmDialog } from '../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, MatIconModule, NotificationSlideWindow, ActionButton],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class HeaderComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly userService = inject(UserService);
  private readonly permissionService = inject(PermissionService);
  private readonly notificationService = inject(NotificationService);
  protected readonly hubService = inject(NotificationHubService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly destroy$ = new Subject<void>();

  icons = APP_CONSTANTS.ICONS;
  strings = APP_CONSTANTS.STRINGS;
  private readonly ROUTES = APP_CONSTANTS.ROUTES;

  readonly userName = signal('');
  readonly profilePhoto = signal('');
  userRole = this.permissionService.roleName;

  readonly showNotifications = signal(false);
  readonly unreadCount = signal(0);

  private latestNotificationId: string | null = null;

  ngOnInit(): void {
    this.loadUserProfile();
    this.hubService.connect();
    this.seedLatestId();
    this.listenHub();
    this.startPolling();
  }

  private loadUserProfile(): void {
    const userId = this.authService.getUserId();
    if (!userId) {
      return;
    }

    this.userService.getUserById(userId).subscribe({
      next: (user) => {
        if (user.userName) {
          this.userName.set(user.userName);
        }
        if (user.photoUrl) {
          this.profilePhoto.set(
            user.photoUrl.startsWith('/uploads/')
              ? `${API_CONFIG.GATEWAY}/gateway${user.photoUrl}`
              : user.photoUrl,
          );
        }
      },
    });
  }

  private seedLatestId(): void {
    this.notificationService.getNotifications(undefined, 1, 1, true).subscribe((res) => {
      if (res.items.length > 0) {
        this.latestNotificationId = res.items[0].id;
        this.unreadCount.set(res.items.filter((n) => !n.isRead).length);
      }
    });
  }

  private listenHub(): void {
    this.hubService.onNotification$.pipe(takeUntil(this.destroy$)).subscribe((notif) => {
      this.latestNotificationId = notif.id;
      this.unreadCount.update((c) => c + 1);
      this.showSnackbar(notif.title);
    });
  }

  private startPolling(): void {
    interval(5000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.notificationService.getNotifications(undefined, 1, 1, true).subscribe((res) => {
          if (res.items.length === 0) {
            return;
          }
          const latest = res.items[0];
          if (latest.id !== this.latestNotificationId) {
            this.latestNotificationId = latest.id;
            const unread = res.items.filter((n) => !n.isRead).length;
            if (unread !== this.unreadCount()) {
              this.unreadCount.set(unread);
            }
            if (!latest.isRead) {
              this.showSnackbar(latest.title);
            }
          }
        });
      });
  }

  private showSnackbar(title: string): void {
    const ref = this.snackBar.open(title, this.strings.NOTIFICATION_VIEW, {
      duration: 5000,
      panelClass: 'snackbar-success',
    });
    ref.onAction().subscribe(() => {
      this.showNotifications.set(true);
    });
  }

  toggleNotifications(): void {
    this.showNotifications.update((v) => !v);
  }

  closeNotifications(): void {
    this.showNotifications.set(false);
  }

  goToSettings(): void {
    this.router.navigate([this.ROUTES.SETTINGS]);
  }

  logout(): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.strings.LOGOUT,
        message: 'Are you sure you want to logout?',
        confirmLabel: this.strings.LOGOUT,
        variant: 'danger',
      },
    });
    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) { return; }
      this.authService.httpLogout().subscribe({
        next: () => {
          this.authService.clearSession();
          this.router.navigate([this.ROUTES.LOGIN]);
        },
        error: () => {
          this.authService.clearSession();
          this.router.navigate([this.ROUTES.LOGIN]);
        },
      });
    });
  }

  ngOnDestroy(): void {
    this.hubService.disconnect();
    this.destroy$.next();
    this.destroy$.complete();
  }
}
