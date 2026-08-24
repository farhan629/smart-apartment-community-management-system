import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AvailableSlotResponseDto } from '../../../../core/services/aminety-service';
import { AMENITY_BOOKING_STRINGS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-booking',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './amenity-booking.html',
  styleUrl: './amenity-booking.scss',
})
export class AmenityBooking {
  @Input() morningSlots: AvailableSlotResponseDto[] = [];
  @Input() afternoonSlots: AvailableSlotResponseDto[] = [];
  @Input() eveningSlots: AvailableSlotResponseDto[] = [];
  @Input() selectedSlot: AvailableSlotResponseDto | null = null;
  @Input() selectedDate: Date = new Date();
  @Input() loadingSlots = false;

  @Output() slotSelected = new EventEmitter<AvailableSlotResponseDto>();

  bookingStrings = AMENITY_BOOKING_STRINGS;

  selectSlot(slot: AvailableSlotResponseDto): void {
    if (slot.availableSpots !== undefined && slot.availableSpots <= 0) return;
    this.slotSelected.emit(slot);
  }
}
