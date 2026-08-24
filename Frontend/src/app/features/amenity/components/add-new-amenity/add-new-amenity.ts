import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { AmenityService, CreateAmenityRequestDto } from '../../../../core/services/aminety-service';
import { ADD_NEW_AMENITY_STRINGS, SLOT_TYPE_IDS, AMENITY_STATUS_IDS } from '../../../../core/constants/amenity.constants';

@Component({
  selector: 'app-add-new-amenity',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule],
  templateUrl: './add-new-amenity.html',
  styleUrl: './add-new-amenity.scss',
})
export class AddNewAmenity {
  @Input() isSubmitting = false;

  @Output() submitAmenity = new EventEmitter<CreateAmenityRequestDto>();
  @Output() back = new EventEmitter<void>();

  strings = ADD_NEW_AMENITY_STRINGS;
  
  name = signal<string>('');
  location = signal<string>('');
  slotTypeId = signal<string>(SLOT_TYPE_IDS.TIME_COUNT);
  statusId = signal<string>(AMENITY_STATUS_IDS.AVAILABLE);
  rules = signal<string>('');
  imageUrl = signal<string>('');

  isUploading = signal<boolean>(false);
  uploadError = signal<string | null>(null);
  validationNameError = signal<string | null>(null);
  validationLocationError = signal<string | null>(null);

  slotTypesList = [
    { label: this.strings.LABEL_SLOT_TIME_COUNT, value: SLOT_TYPE_IDS.TIME_COUNT },
    { label: this.strings.LABEL_SLOT_TIME_ONLY, value: SLOT_TYPE_IDS.TIME }
  ];

  statusList = [
    { label: this.strings.LABEL_STATUS_AVAILABLE, value: AMENITY_STATUS_IDS.AVAILABLE },
    { label: this.strings.LABEL_STATUS_MAINTENANCE, value: AMENITY_STATUS_IDS.MAINTENANCE }
  ];

  constructor(private amenityService: AmenityService) {}

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    const files = target.files;
    if (!files || files.length === 0) return;

    const file = files[0];
    this.isUploading.set(true);
    this.uploadError.set(null);

    this.amenityService.postApiAmenityUpload({ file }).subscribe({
      next: (res) => {
        if (res.imageUrl) {
          this.imageUrl.set(res.imageUrl);
        } else {
          this.uploadError.set(this.strings.ERROR_UPLOAD);
        }
        this.isUploading.set(false);
      },
      error: (err) => {
        console.error('Error uploading image', err);
        this.uploadError.set(this.strings.ERROR_UPLOAD);
        this.isUploading.set(false);
      }
    });
  }

  onSubmit(): void {
    let hasError = false;

    if (!this.name().trim()) {
      this.validationNameError.set(this.strings.VALIDATION_NAME);
      hasError = true;
    } else {
      this.validationNameError.set(null);
    }

    if (!this.location().trim()) {
      this.validationLocationError.set(this.strings.VALIDATION_LOCATION);
      hasError = true;
    } else {
      this.validationLocationError.set(null);
    }

    if (hasError) return;

    const request: CreateAmenityRequestDto = {
      name: this.name().trim(),
      location: this.location().trim(),
      slotTypeId: this.slotTypeId(),
      statusId: this.statusId(),
      rules: this.rules().trim() || null,
      imageUrl: this.imageUrl() || null
    };

    this.submitAmenity.emit(request);
  }

  onBack(): void {
    this.back.emit();
  }
}
