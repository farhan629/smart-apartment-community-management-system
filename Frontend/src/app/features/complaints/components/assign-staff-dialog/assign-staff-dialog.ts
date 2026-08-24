import { CommonModule } from '@angular/common';
import { Component, computed, Inject, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { ActionButton } from '../../../../shared/components/action-button/action-button';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  ASSIGN_STAFF_DIALOG_STRINGS,
  ASSIGNMENT_MODE,
} from '../../../../core/constants/complaint.constants';
import { StaffSummaryDto } from '../../../../core/models/staff.model';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { StaffService } from '../../../../core/services/staff.service';

export interface AssignStaffDialogData {
  complaintId: string;
  category: string;
  mode: typeof ASSIGNMENT_MODE.ASSIGN | typeof ASSIGNMENT_MODE.REASSIGN;
  assignmentId?: string;
}

const STAFF_PAGE_LIMIT = 100;

@Component({
  selector: 'app-assign-staff-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ActionButton],
  templateUrl: './assign-staff-dialog.html',
  styleUrl: './assign-staff-dialog.scss',
})
export class AssignStaffDialog implements OnInit {
  private readonly staffService = inject(StaffService);
  private readonly assignmentService = inject(AssignmentService);

  strings = { ...APP_CONSTANTS.STRINGS, ...ASSIGN_STAFF_DIALOG_STRINGS };
  assignmentMode = ASSIGNMENT_MODE;

  readonly mode: typeof ASSIGNMENT_MODE.ASSIGN | typeof ASSIGNMENT_MODE.REASSIGN;
  readonly category: string;

  readonly dialogTitle: string;
  private readonly confirmActionLabel: string;

  readonly minDate = new Date().toISOString().split('T')[0];

  staff = signal<StaffSummaryDto[]>([]);
  loading = signal(true);
  loadError = signal(false);

  showAllCategories = signal(false);
  selectedStaffId = signal<string>('');
  dueDate = signal<string>('');

  isSubmitting = signal(false);
  submitError = signal<string | null>(null);

  readonly visibleStaff = computed(() => {
    const all = this.staff();
    if (this.showAllCategories() || !this.category) {
      return all;
    }
    const inCategory = all.filter((s) => s.categoryName === this.category);
    return inCategory.length > 0 ? inCategory : all;
  });

  readonly confirmLabel = computed(() =>
    this.isSubmitting() ? this.strings.SAVING_LABEL : this.confirmActionLabel,
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) private readonly data: AssignStaffDialogData,
    private readonly dialogRef: MatDialogRef<AssignStaffDialog, boolean>,
  ) {
    this.mode = data.mode;
    this.category = data.category;

    this.dialogTitle =
      this.mode === ASSIGNMENT_MODE.ASSIGN
        ? this.strings.ASSIGN_TITLE
        : this.strings.REASSIGN_TITLE;

    this.confirmActionLabel =
      this.mode === ASSIGNMENT_MODE.ASSIGN
        ? this.strings.ASSIGN_ACTION_LABEL
        : this.strings.REASSIGN_ACTION_LABEL;
  }

  ngOnInit(): void {
    this.staffService.getStaffList(1, STAFF_PAGE_LIMIT).subscribe({
      next: (result) => {
        this.staff.set(result.items.filter((s) => s.isActive));
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.loading.set(false);
      },
    });
  }

  selectStaff(staffId: string): void {
    this.selectedStaffId.set(staffId);
  }

  confirm(): void {
    if (!this.selectedStaffId() || !this.dueDate()) {
      this.submitError.set(this.strings.VALIDATION_ERROR);
      return;
    }

    this.isSubmitting.set(true);
    this.submitError.set(null);

    const payload = { staffId: this.selectedStaffId(), dueDate: this.dueDate() };

    const request$ =
      this.mode === ASSIGNMENT_MODE.ASSIGN
        ? this.assignmentService.assign(this.data.complaintId, payload)
        : this.assignmentService.reassign(this.data.complaintId, this.data.assignmentId!, payload);

    request$.subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSubmitting.set(false);
        this.submitError.set(this.strings.SAVE_ERROR);
      },
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
