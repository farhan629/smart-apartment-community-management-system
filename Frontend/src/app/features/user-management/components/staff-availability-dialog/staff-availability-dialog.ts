import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { EmptyState } from '../../../../shared/components/empty-state/empty-state';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  STAFF_AVAILABILITY_FORMATS,
  STAFF_AVAILABILITY_STRINGS,
} from '../../../../core/constants/staff.constants';
import { StaffAvailabilityService } from '../../../../core/services/staff-availability.service';
import { AvailabilitySlotDto } from '../../../../core/models/staff-availability.model';

export interface StaffAvailabilityDialogData {
  staffId: string;
  categoryName: string;
}

@Component({
  selector: 'app-staff-availability-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, ActionButton, EmptyState],
  templateUrl: './staff-availability-dialog.html',
  styleUrl: './staff-availability-dialog.scss',
})
export class StaffAvailabilityDialog implements OnInit {
  private readonly staffAvailabilityService = inject(StaffAvailabilityService);
  private readonly dialogRef = inject(MatDialogRef<StaffAvailabilityDialog>);

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  availabilityStrings = STAFF_AVAILABILITY_STRINGS;
  dateFormat = STAFF_AVAILABILITY_FORMATS.DATE;

  readonly minDate = new Date().toISOString().split('T')[0];
  readonly dialogTitle: string;

  slots = signal<AvailabilitySlotDto[]>([]);
  loading = signal(true);
  loadError = signal(false);

  date = signal<string>('');
  startTime = signal<string>('');
  endTime = signal<string>('');

  isSaving = signal(false);
  saveError = signal<string | null>(null);

  deletingSlotId = signal<string | null>(null);
  deleteError = signal<string | null>(null);

  constructor(@Inject(MAT_DIALOG_DATA) public data: StaffAvailabilityDialogData) {
    this.dialogTitle = `${STAFF_AVAILABILITY_STRINGS.DIALOG_TITLE_PREFIX} ${data.categoryName}`;
  }

  ngOnInit(): void {
    this.loadSlots();
  }

  loadSlots(): void {
    this.loading.set(true);
    this.loadError.set(false);

    this.staffAvailabilityService.getSlots({ staffId: this.data.staffId }).subscribe({
      next: (slots) => {
        this.slots.set(slots);
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.loading.set(false);
      },
    });
  }

  addSlot(): void {
    const date = this.date();
    const startTime = this.startTime();
    const endTime = this.endTime();

    if (!date || !startTime || !endTime) {
      this.saveError.set(this.availabilityStrings.VALIDATION_ERROR);
      return;
    }

    if (startTime >= endTime) {
      this.saveError.set(this.availabilityStrings.TIME_RANGE_ERROR);
      return;
    }

    this.isSaving.set(true);
    this.saveError.set(null);

    this.staffAvailabilityService
      .createSlots(this.data.staffId, { slots: [{ date, startTime, endTime }] })
      .subscribe({
        next: (created) => {
          this.slots.set([...this.slots(), ...created]);
          this.date.set('');
          this.startTime.set('');
          this.endTime.set('');
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.saveError.set(this.availabilityStrings.SAVE_ERROR);
        },
      });
  }

  deleteSlot(slot: AvailabilitySlotDto): void {
    if (slot.isBooked) {
      return;
    }

    this.deletingSlotId.set(slot.slotId);
    this.deleteError.set(null);

    this.staffAvailabilityService.deleteSlot(this.data.staffId, slot.slotId).subscribe({
      next: () => {
        this.slots.set(this.slots().filter((s) => s.slotId !== slot.slotId));
        this.deletingSlotId.set(null);
      },
      error: () => {
        this.deletingSlotId.set(null);
        this.deleteError.set(this.availabilityStrings.DELETE_ERROR);
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
