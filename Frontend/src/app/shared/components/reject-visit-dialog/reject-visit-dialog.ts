import { CommonModule } from '@angular/common';
import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

import { APP_CONSTANTS } from '../../../core/constants/app.constants';
import { VISITOR_MANAGEMENT_ICONS } from '../../../core/constants/visitor-management-ui.constants';
import { ActionButton } from '../action-button/action-button';
import { PopupDialog } from '../popup-dialog/popup-dialog';

export interface RejectVisitDialogData {
  title?: string;
}

const REJECTION_REASON_MAX_LENGTH = 500;

@Component({
  selector: 'app-reject-visit-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatIconModule, PopupDialog, ActionButton],
  templateUrl: './reject-visit-dialog.html',
  styleUrl: './reject-visit-dialog.scss',
})
export class RejectVisitDialog {
  private readonly fb = inject(FormBuilder);

  strings = APP_CONSTANTS.STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;
  maxLength = REJECTION_REASON_MAX_LENGTH;

  form = this.fb.nonNullable.group({
    rejectionReason: ['', [Validators.required, Validators.maxLength(this.maxLength)]],
  });

  constructor(
    public dialogRef: MatDialogRef<RejectVisitDialog, string>,
    @Inject(MAT_DIALOG_DATA) public data: RejectVisitDialogData,
  ) {
    this.data = { title: data?.title ?? this.strings.REJECT_VISIT_TITLE };
  }

  get charCount(): number {
    return this.form.controls.rejectionReason.value.length;
  }

  onCancel(): void {
    this.dialogRef.close(undefined);
  }

  onConfirm(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.form.getRawValue().rejectionReason.trim());
  }
}