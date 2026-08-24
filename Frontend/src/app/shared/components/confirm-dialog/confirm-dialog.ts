import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';
import { ActionButton } from '../action-button/action-button';
import { PopupDialog } from '../popup-dialog/popup-dialog';

export interface ConfirmDialogData {
  title?: string;
  message?: string;
  confirmLabel?: string;
  cancelLabel?: string;
  variant?: 'primary' | 'danger';
}
@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, PopupDialog, ActionButton],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss',
})
export class ConfirmDialog {
  strings = APP_CONSTANTS.STRINGS;

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialog, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData,
  ) {
    this.data = {
      title: data.title ?? this.strings.ARE_YOU_SURE,
      message: data.message ?? this.strings.ARE_YOU_SURE_MESSAGE,
      confirmLabel: data.confirmLabel ?? this.strings.CONFIRM,
      cancelLabel: data.cancelLabel ?? this.strings.CANCEL,
      variant: data.variant ?? 'primary',
    };
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
