import { CommonModule } from '@angular/common';
import { Component, effect, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { APP_CONSTANTS, NavItem } from '../../../core/constants/app.constants';
import { PermissionService } from '../../../core/services/permission.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class SidebarComponent {
  private readonly permissionService = inject(PermissionService);

  appName = APP_CONSTANTS.STRINGS.APP_NAME;
  navItems: NavItem[] = [];

  constructor() {
    this.refreshNavItems();

    effect(() => {
      this.permissionService.loaded();
      this.refreshNavItems();
    });
  }

  private refreshNavItems(): void {
    this.navItems = APP_CONSTANTS.NAV_ITEMS.filter((item) => this.canSee(item));
  }

  private canSee(item: NavItem): boolean {
    if (item.permissions?.length) {
      return this.permissionService.hasAnyPermission(item.permissions);
    }

    return true;
  }
}
