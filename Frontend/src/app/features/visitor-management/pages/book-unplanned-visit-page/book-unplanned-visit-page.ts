import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { VISITOR_FORM_RULES } from '../../../../core/constants/visitor.constants';
import { VisitService } from '../../../../core/services/visit.service';
import { VisitorService } from '../../../../core/services/visitor.service';
import { FlatService } from '../../../../core/services/flat-service';
import { RefTermOption } from '../../../../core/models/visitor.model';
import { FlatItemDto } from '../../../../core/models/auth.models';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-book-unplanned-visit-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ActionButton],
  templateUrl: './book-unplanned-visit-page.html',
  styleUrl: './book-unplanned-visit-page.scss',
})
export class BookUnplannedVisitPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly visitService = inject(VisitService);
  private readonly visitorService = inject(VisitorService);
  private readonly flatService = inject(FlatService);
  private readonly router = inject(Router);

  strings = APP_CONSTANTS.STRINGS;
  formRules = VISITOR_FORM_RULES;

  visitorTypes = signal<RefTermOption[]>([]);
  purposeTypes = signal<RefTermOption[]>([]);
  allFlats = signal<FlatItemDto[]>([]);

  isLoadingVisitorTypes = signal(false);
  isLoadingPurposeTypes = signal(false);
  isLoadingFlats = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  readonly minDate = new Date().toISOString().split('T')[0];

  form = this.fb.nonNullable.group({
    blockNumber: ['', Validators.required],
    flatNumber: ['', Validators.required],
    name: ['', [Validators.required, Validators.maxLength(this.formRules.NAME_MAX_LENGTH)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.formRules.PHONE_PATTERN)]],
    email: ['', [Validators.maxLength(this.formRules.EMAIL_MAX_LENGTH), Validators.email]],
    visitorTypeId: ['', Validators.required],
    purposeTypeId: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
  });

  private readonly selectedBlock = toSignal(this.form.controls.blockNumber.valueChanges, {
    initialValue: '',
  });

  readonly blocks = computed(() => {
    const names = new Set(this.allFlats().map((flat) => flat.block));
    return [...names].sort((a, b) => a.localeCompare(b));
  });

  readonly flatsInSelectedBlock = computed(() =>
    this.allFlats()
      .filter((flat) => flat.block === this.selectedBlock())
      .sort((a, b) => a.number.localeCompare(b.number, undefined, { numeric: true })),
  );

  ngOnInit(): void {
    this.isLoadingVisitorTypes.set(true);
    this.isLoadingPurposeTypes.set(true);
    this.isLoadingFlats.set(true);

    this.form.controls.blockNumber.valueChanges.subscribe(() => {
      this.form.controls.flatNumber.setValue('');
    });

    this.flatService.getFlats().subscribe({
      next: (response) => {
        this.allFlats.set(response.items);
        this.isLoadingFlats.set(false);
      },
      error: () => {
        this.errorMessage.set(this.strings.FLATS_LOAD_FAILED);
        this.isLoadingFlats.set(false);
      },
    });

    this.visitorService.getVisitorTypes().subscribe({
      next: (types) => {
        this.visitorTypes.set(types);
        this.isLoadingVisitorTypes.set(false);
      },
      error: () => {
        this.errorMessage.set(this.strings.VISITOR_TYPES_LOAD_FAILED);
        this.isLoadingVisitorTypes.set(false);
      },
    });

    this.visitorService.getPurposeTypes().subscribe({
      next: (types) => {
        this.purposeTypes.set(types);
        this.isLoadingPurposeTypes.set(false);
      },
      error: () => {
        this.errorMessage.set(this.strings.PURPOSE_TYPES_LOAD_FAILED);
        this.isLoadingPurposeTypes.set(false);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.isSubmitting.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.visitService
      .createVisit({
        visitor: {
          name: value.name.trim(),
          phoneNumber: value.phoneNumber.trim(),
          email: value.email?.trim() || undefined,
          visitorTypeId: value.visitorTypeId,
        },
        purposeTypeId: value.purposeTypeId,
        startDate: value.startDate,
        endDate: value.endDate,
        blockNumber: value.blockNumber.trim(),
        flatNumber: value.flatNumber.trim(),
      })
      .subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.successMessage.set(this.strings.UNPLANNED_VISIT_SUBMITTED);
          this.form.reset();
        },
        error: () => {
          this.isSubmitting.set(false);
          this.errorMessage.set(this.strings.VISIT_BOOK_FAILED);
        },
      });
  }

  onCancel(): void {
    this.router.navigate([APP_CONSTANTS.ROUTES.DASHBOARD]);
  }
}
