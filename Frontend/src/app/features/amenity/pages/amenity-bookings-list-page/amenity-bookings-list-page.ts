import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AmenityBookingHistory } from '../../components/amenity-booking-history/amenity-booking-history';
import { BOOKING_HISTORY_PAGE_STRINGS, AMENITY_ROUTES } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-bookings-list-page',
  standalone: true,
  imports: [CommonModule, AmenityBookingHistory],
  templateUrl: './amenity-bookings-list-page.html',
  styleUrl: './amenity-bookings-list-page.scss',
})
export class AmenityBookingsListPage {
  pageStrings = BOOKING_HISTORY_PAGE_STRINGS;

  constructor(private router: Router) {}

  goBack(): void {
    this.router.navigate([AMENITY_ROUTES.BASE]);
  }
}
