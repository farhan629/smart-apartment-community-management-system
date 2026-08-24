import { Component, computed } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { PermissionService } from '../../../../core/services/permission.service';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { AmenityBookingHistory } from '../../components/amenity-booking-history/amenity-booking-history';
import { AmenityCards } from '../../components/amenity-cards/amenity-cards';
import { AMENITY_DASHBOARD_STRINGS, DASHBOARD_SCROLL, DASHBOARD_NUMBERS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-dashboard-user',
  standalone: true,
  imports: [AmenityBookingHistory, AmenityCards, MatButtonModule],
  templateUrl: './amenity-dashboard-user.html',
  styleUrl: './amenity-dashboard-user.scss',
})
export class AmenityDashboardUser {
  dashboardStrings = AMENITY_DASHBOARD_STRINGS;
  historyLimit = DASHBOARD_NUMBERS.HISTORY_LIMIT;

  isAdmin = computed(() => this.permissionService.hasPermission(PERMISSIONS.AMENITY_MANAGE));

  constructor(
    private permissionService: PermissionService,
    private router: Router,
  ) {}

  navigateToAddAmenity(): void {
    this.router.navigate(['/amenities/new']);
  }

  scrollToAmenities(): void {
    const element = document.getElementById(DASHBOARD_SCROLL.ELEMENT_ID);
    if (element) {
      element.scrollIntoView({
        behavior: DASHBOARD_SCROLL.BEHAVIOR,
        block: DASHBOARD_SCROLL.BLOCK
      });
    }
  }
}
