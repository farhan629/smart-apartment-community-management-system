import { Component, EventEmitter, Input, OnInit, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { provideNativeDateAdapter } from '@angular/material/core';

import { FormatTimePipe } from '../../pipes/format-time.pipe';
import { AMENITY_SLOT_STRINGS } from '../../../../core/constants/amenity.constants';
import {
  SlotService,
  AmenityResponseDto,
  SlotResponseDto,
  CreateSlotRequestDto,
  UpdateSlotRequestDto
} from '../../../../core/services/aminety-service';

@Component({
  selector: 'app-amenity-slot',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule,
    MatSnackBarModule,
    FormatTimePipe
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './amenity-slot.html',
  styleUrl: './amenity-slot.scss',
})
export class AmenitySlot implements OnInit {
  @Input() amenity!: AmenityResponseDto;
  @Output() close = new EventEmitter<void>();

  slotStrings = AMENITY_SLOT_STRINGS;

  loading = signal<boolean>(false);
  submitting = signal<boolean>(false);
  activeTab = signal<'create' | 'manage'>('create');

  // Create slot form models
  newSlot = {
    slotLabel: '',
    slotDate: null as Date | null,
    startTime: '',
    endTime: '',
    maxCapacity: 1
  };

  // Queue of slots to be created
  queue = signal<CreateSlotRequestDto[]>([]);

  // Manage slots variables
  slots = signal<SlotResponseDto[]>([]);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(5);
  totalPages = signal<number>(1);
  totalSlots = signal<number>(0);

  // Edit slot states
  editingSlotId = signal<string | null>(null);
  editForm = {
    slotLabel: '',
    startTime: '',
    endTime: '',
    maxCapacity: 1
  };

  constructor(
    private slotService: SlotService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    if (this.amenity?.id) {
      this.loadSlots();
    }
  }

  setTab(tab: 'create' | 'manage'): void {
    this.activeTab.set(tab);
    if (tab === 'manage') {
      this.loadSlots();
    }
  }

  addSlotToQueue(): void {
    if (!this.newSlot.slotDate || !this.newSlot.startTime || !this.newSlot.endTime || !this.newSlot.maxCapacity) {
      this.snackBar.open('Please fill in all required fields.', 'Close', { duration: 3000 });
      return;
    }

    if (this.newSlot.startTime >= this.newSlot.endTime) {
      this.snackBar.open('Start time must be before end time.', 'Close', { duration: 3000 });
      return;
    }

    // Format local date to YYYY-MM-DD
    const d = this.newSlot.slotDate;
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const dateString = `${year}-${month}-${day}`;

    const slot: CreateSlotRequestDto = {
      slotLabel: this.newSlot.slotLabel || null,
      slotDate: dateString,
      startTime: this.newSlot.startTime + ':00', // API expects HH:mm:ss format
      endTime: this.newSlot.endTime + ':00',
      maxCapacity: this.newSlot.maxCapacity
    };

    this.queue.update(q => [...q, slot]);

    // Reset inputs, leaving date for convenience
    this.newSlot.slotLabel = '';
    this.newSlot.startTime = '';
    this.newSlot.endTime = '';
    this.newSlot.maxCapacity = 1;
  }

  removeFromQueue(index: number): void {
    this.queue.update(q => q.filter((_, i) => i !== index));
  }

  submitQueue(): void {
    if (this.queue().length === 0) return;

    this.submitting.set(true);
    this.slotService.postApiAmenitiesSlotsBulk(this.amenity.id!, { slots: this.queue() }).subscribe({
      next: () => {
        this.snackBar.open('Slots created successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.queue.set([]);
        this.submitting.set(false);
        this.setTab('manage');
      },
      error: (err) => {
        console.error('Error creating slots', err);
        this.snackBar.open('Error creating slots. Please try again.', 'Close', { duration: 3000 });
        this.submitting.set(false);
      }
    });
  }

  loadSlots(): void {
    if (!this.amenity?.id) return;

    this.loading.set(true);
    this.slotService.getApiAmenitiesSlots(this.amenity.id, this.pageNumber(), this.pageSize()).subscribe({
      next: (res) => {
        this.slots.set(res.data ?? []);
        if (res.pagination) {
          this.totalPages.set(res.pagination.totalPages ?? 1);
          this.totalSlots.set(res.pagination.totalCount ?? 0);
        } else {
          this.totalPages.set(1);
          this.totalSlots.set(res.data?.length ?? 0);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading slots', err);
        this.loading.set(false);
      }
    });
  }

  prevPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update(p => p - 1);
      this.loadSlots();
    }
  }

  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update(p => p + 1);
      this.loadSlots();
    }
  }

  startEdit(slot: SlotResponseDto): void {
    this.editingSlotId.set(slot.id || null);
    this.editForm = {
      slotLabel: slot.slotLabel || '',
      startTime: this.formatTimeForInput(slot.startTime),
      endTime: this.formatTimeForInput(slot.endTime),
      maxCapacity: slot.maxCapacity || 1
    };
  }

  cancelEdit(): void {
    this.editingSlotId.set(null);
  }

  saveEdit(slotId: string): void {
    if (!this.editForm.startTime || !this.editForm.endTime || !this.editForm.maxCapacity) {
      this.snackBar.open('Please fill in all required fields.', 'Close', { duration: 3000 });
      return;
    }

    if (this.editForm.startTime >= this.editForm.endTime) {
      this.snackBar.open('Start time must be before end time.', 'Close', { duration: 3000 });
      return;
    }

    const request: UpdateSlotRequestDto = {
      slotLabel: this.editForm.slotLabel || null,
      startTime: this.editForm.startTime.includes(':') && this.editForm.startTime.split(':').length === 2 
        ? this.editForm.startTime + ':00' 
        : this.editForm.startTime,
      endTime: this.editForm.endTime.includes(':') && this.editForm.endTime.split(':').length === 2 
        ? this.editForm.endTime + ':00' 
        : this.editForm.endTime,
      maxCapacity: this.editForm.maxCapacity
    };

    this.submitting.set(true);
    this.slotService.putApiSlots(slotId, request).subscribe({
      next: () => {
        this.snackBar.open('Slot updated successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.editingSlotId.set(null);
        this.submitting.set(false);
        this.loadSlots();
      },
      error: (err) => {
        console.error('Error updating slot', err);
        this.snackBar.open('Error updating slot. Please try again.', 'Close', { duration: 3000 });
        this.submitting.set(false);
      }
    });
  }

  deleteSlot(slotId: string): void {
    if (confirm('Are you sure you want to delete this slot?')) {
      this.submitting.set(true);
      this.slotService.deleteApiSlots(slotId).subscribe({
        next: () => {
          this.snackBar.open('Slot deleted successfully!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.submitting.set(false);
          if (this.slots().length === 1 && this.pageNumber() > 1) {
            this.pageNumber.update(p => p - 1);
          }
          this.loadSlots();
        },
        error: (err) => {
          console.error('Error deleting slot', err);
          this.snackBar.open('Error deleting slot. Please try again.', 'Close', { duration: 3000 });
          this.submitting.set(false);
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }

  // Private helper to format slot time (HH:mm:ss -> HH:mm) for HTML5 time input fields
  private formatTimeForInput(time: string | undefined | null): string {
    if (!time) return '';
    const parts = time.split(':');
    if (parts.length >= 2) {
      return `${parts[0]}:${parts[1]}`;
    }
    return time;
  }
}
