import { Component, Input, Output, EventEmitter, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { CANCELLATION_STRINGS, AMENITY_BOOKING_PAGE_STRINGS, CALENDER_NUMBERS } from '../../../../core/constants/amenity.constants';

export interface DialogBookingDetails {
  amenityName: string;
  slotDate: string | Date;
  slotLabel: string;
  status?: string;
}

@Component({
  selector: 'app-amenity-action-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule],
  templateUrl: './amenity-action-dialog.html',
  styleUrl: './amenity-action-dialog.scss',
})
export class AmenityActionDialog {
  @Input() mode: 'confirm' | 'cancel' = 'cancel';
  @Input() bookingDetails!: DialogBookingDetails;
  @Input() isSubmitting = false;
  @Input() isInline = false;

  @Output() confirm = new EventEmitter<string>();
  @Output() close = new EventEmitter<void>();

  reason = signal<string>('');
  validationError = signal<string | null>(null);

  readonly title = computed(() => 
    this.mode === 'confirm' ? AMENITY_BOOKING_PAGE_STRINGS.DIALOG_TITLE : CANCELLATION_STRINGS.PAGE_TITLE
  );

  readonly subtitle = computed(() => 
    this.mode === 'confirm' ? AMENITY_BOOKING_PAGE_STRINGS.DIALOG_SUBTITLE : CANCELLATION_STRINGS.SUBTITLE
  );

  readonly btnConfirmLabel = computed(() => 
    this.mode === 'confirm' ? AMENITY_BOOKING_PAGE_STRINGS.BTN_CONFIRM : CANCELLATION_STRINGS.BTN_CONFIRM
  );

  readonly btnSubmittingLabel = computed(() => 
    this.mode === 'confirm' ? AMENITY_BOOKING_PAGE_STRINGS.BTN_BOOKING : CANCELLATION_STRINGS.BTN_CANCELLING
  );

  readonly btnBackLabel = computed(() => 
    this.mode === 'confirm' ? AMENITY_BOOKING_PAGE_STRINGS.BTN_CANCEL : CANCELLATION_STRINGS.BTN_BACK
  );

  readonly placeholderReason = CANCELLATION_STRINGS.PLACEHOLDER_REASON;
  readonly labelReason = CANCELLATION_STRINGS.LABEL_REASON;
  readonly labelAmenity = CANCELLATION_STRINGS.LABEL_AMENITY;
  readonly labelDate = CANCELLATION_STRINGS.LABEL_DATE;
  readonly labelSlot = CANCELLATION_STRINGS.LABEL_SLOT;
  readonly labelStatus = CANCELLATION_STRINGS.LABEL_STATUS;

  onSubmit(): void {
    if (this.mode === 'cancel') {
      const val = this.reason().trim();
      if (!val) {
        this.validationError.set(CANCELLATION_STRINGS.VALIDATION_REASON);
        return;
      }
      this.validationError.set(null);
      this.confirm.emit(val);
    } else {
      this.confirm.emit('');
    }
  }

  onBack(): void {
    this.close.emit();
  }
}
