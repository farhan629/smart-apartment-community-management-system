import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AMENITY_DOWNBAR_STRINGS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-downbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './amenity-downbar.html',
  styleUrl: './amenity-downbar.scss',
})
export class AmenityDownbar {
  @Input() selectedDate: Date | null = null;
  @Input() selectedSlotLabel: string | null = null;
  @Input() isValid = false;

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  downbarStrings = AMENITY_DOWNBAR_STRINGS;

  onConfirm(): void {
    if (this.isValid) {
      this.confirm.emit();
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
