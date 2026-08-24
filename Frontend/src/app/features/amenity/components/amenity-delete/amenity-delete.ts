import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { AmenityResponseDto } from '../../../../core/services/aminety-service';
import { DELETE_AMENITY_STRINGS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-amenity-delete',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './amenity-delete.html',
  styleUrl: './amenity-delete.scss',
})
export class AmenityDelete {
  @Input() amenity!: AmenityResponseDto;
  @Input() isSubmitting = false;

  @Output() confirm = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  strings = DELETE_AMENITY_STRINGS;

  onCancel(): void {
    this.close.emit();
  }

  onDelete(): void {
    this.confirm.emit();
  }
}
