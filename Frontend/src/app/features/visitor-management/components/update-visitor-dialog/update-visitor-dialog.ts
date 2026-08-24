import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin, throwError } from 'rxjs';
import { catchError, finalize, timeout } from 'rxjs/operators';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  VISITOR_MANAGEMENT_STRINGS,
  VISITOR_MANAGEMENT_ICONS,
} from '../../../../core/constants/visitor-management-ui.constants';
import { VISITOR_FORM_RULES } from '../../../../core/constants/visitor.constants';
import { RefTermOption } from '../../../../core/models/visitor.model';
import { VisitorService } from '../../../../core/services/visitor.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';

export interface UpdateVisitorDialogData {
  visitorId: string;
}

const LOAD_TIMEOUT_MS = 10000;

@Component({
  selector: 'app-update-visitor-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PopupDialog,
    ActionButton,
  ],
  templateUrl: './update-visitor-dialog.html',
  styleUrl: './update-visitor-dialog.scss',
})
export class UpdateVisitorDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly visitorService = inject(VisitorService);

  strings = APP_CONSTANTS.STRINGS;
  vm = VISITOR_MANAGEMENT_STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;
  formRules = VISITOR_FORM_RULES;

  visitorTypes = signal<RefTermOption[]>([]);

  isLoading = signal(true);
  isSaving = signal(false);
  errorMessage = signal('');

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(this.formRules.NAME_MAX_LENGTH)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.formRules.PHONE_PATTERN)]],
    email: ['', [Validators.maxLength(this.formRules.EMAIL_MAX_LENGTH), Validators.email]],
    visitorTypeId: ['', Validators.required],
  });

  constructor(
    public dialogRef: MatDialogRef<UpdateVisitorDialog, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: UpdateVisitorDialogData,
  ) {
    this.form.disable();
  }

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    forkJoin({
      visitor: this.visitorService.getVisitorById(this.data.visitorId),
      types: this.visitorService.getVisitorTypes(),
    })
      .pipe(
        timeout(LOAD_TIMEOUT_MS),
        catchError((err) => {
          console.error('[UpdateVisitorDialog] load failed:', err);
          return throwError(() => err);
        }),
        finalize(() => this.isLoading.set(false)),
      )
      .subscribe({
        next: ({ visitor, types }) => {
          this.visitorTypes.set(types);
          this.form.setValue({
            name: visitor.name ?? '',
            phoneNumber: visitor.phoneNumber ?? '',
            email: visitor.email ?? '',
            visitorTypeId: visitor.visitorTypeId ?? '',
          });
          this.form.enable();
        },
        error: () => {
          this.errorMessage.set(this.vm.VISITOR_LOAD_FAILED);
        },
      });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onRetry(): void {
    this.loadData();
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.isSaving.set(true);
    this.errorMessage.set('');

    this.visitorService
      .updateVisitor(this.data.visitorId, {
        name: value.name.trim(),
        phoneNumber: value.phoneNumber.trim(),
        email: value.email?.trim() || undefined,
        visitorTypeId: value.visitorTypeId,
      })
      .pipe(finalize(() => this.isSaving.set(false)))
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: () => this.errorMessage.set(this.vm.VISITOR_UPDATE_FAILED),
      });
  }
}
