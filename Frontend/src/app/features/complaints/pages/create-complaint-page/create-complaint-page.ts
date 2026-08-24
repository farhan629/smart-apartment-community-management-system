import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  CREATE_COMPLAINT_STRINGS,
  FORM_VALIDATORS,
  RefDataOption,
} from '../../../../core/constants/complaint.constants';
import { CreateComplaintRequestDto } from '../../../../core/models/complaint.model';
import { ComplaintService } from '../../../../core/services/complaint.service';
import { LookupService } from '../../../../core/services/lookup.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-create-complaint-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ActionButton],
  templateUrl: './create-complaint-page.html',
  styleUrl: './create-complaint-page.scss',
})
export class CreateComplaintPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly complaintService = inject(ComplaintService);
  private readonly lookupService = inject(LookupService);
  private readonly router = inject(Router);
  private readonly dialogRef = inject(MatDialogRef<CreateComplaintPage>);
  protected readonly validators = FORM_VALIDATORS;

  strings = {
    ...APP_CONSTANTS.STRINGS,
    ...CREATE_COMPLAINT_STRINGS,
  };

  icons = APP_CONSTANTS.ICONS;

  readonly descriptionMaxLength = CREATE_COMPLAINT_STRINGS.DESCRIPTION_MAX_LENGTH;

  categoryOptions: RefDataOption[] = [];
  typeOptions: RefDataOption[] = [];
  priorityOptions: RefDataOption[] = [];
  loadingLookups = true;

  isSubmitting = false;
  errorMessage: string | null = null;

  readonly minDate = new Date().toISOString().split('T')[0];

  form = this.fb.nonNullable.group({
    complaintTypeRefId: ['', Validators.required],
    categoryId: ['', Validators.required],
    priorityRefId: ['', Validators.required],
    description: [
      '',
      [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(CREATE_COMPLAINT_STRINGS.DESCRIPTION_MAX_LENGTH),
      ],
    ],
    preferredDate: ['', Validators.required],
    preferredTime: [''],
  });

  ngOnInit(): void {
    this.loadingLookups = true;

    forkJoin({
      types: this.lookupService.getComplaintTypes(),
      priorities: this.lookupService.getComplaintPriorities(),
      categories: this.lookupService.getCategories(),
    }).subscribe({
      next: ({ types, priorities, categories }) => {
        this.typeOptions = types.map((t) => ({ id: t.id, label: t.displayName }));
        this.priorityOptions = priorities.map((p) => ({ id: p.id, label: p.displayName }));
        this.categoryOptions = categories.map((c) => ({ id: c.id, label: c.name }));
        this.loadingLookups = false;
      },
      error: () => {
        this.loadingLookups = false;
        this.errorMessage = this.strings.SUBMIT_ERROR;
      },
    });
  }

  get descriptionLength(): number {
    return this.form.controls.description.value.length;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage = null;
    this.isSubmitting = true;

    const raw = this.form.getRawValue();

    const payload: CreateComplaintRequestDto = {
      complaintTypeRefId: raw.complaintTypeRefId,
      categoryId: raw.categoryId,
      priorityRefId: raw.priorityRefId,
      description: raw.description,
      preferredDate: raw.preferredDate,
      preferredTime: raw.preferredTime || null,
    };

    this.complaintService.create(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSubmitting = false;
        this.errorMessage = this.strings.SUBMIT_ERROR;
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
