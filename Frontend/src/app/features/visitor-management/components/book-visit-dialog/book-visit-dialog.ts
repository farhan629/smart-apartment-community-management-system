import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  VISITOR_FORM_RULES,
  VISITOR_SEARCH_CONFIG,
} from '../../../../core/constants/visitor.constants';
import {
  VISITOR_MANAGEMENT_STRINGS,
  VISITOR_MANAGEMENT_ICONS,
} from '../../../../core/constants/visitor-management-ui.constants';
import { VisitService } from '../../../../core/services/visit.service';
import { VisitorService } from '../../../../core/services/visitor.service';
import { RefTermOption, Visitor } from '../../../../core/models/visitor.model';
import { PopupDialog } from '../../../../shared/components/popup-dialog/popup-dialog';
import { ActionButton } from '../../../../shared/components/action-button/action-button';

@Component({
  selector: 'app-book-visit-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatIconModule, PopupDialog, ActionButton],
  templateUrl: './book-visit-dialog.html',
  styleUrl: './book-visit-dialog.scss',
})
export class BookVisitDialog implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly visitService = inject(VisitService);
  private readonly visitorService = inject(VisitorService);
  public dialogRef = inject(MatDialogRef<BookVisitDialog, boolean>);
  public data = inject(MAT_DIALOG_DATA);

  @ViewChild('nameInput') private readonly nameInput?: ElementRef<HTMLInputElement>;

  strings = APP_CONSTANTS.STRINGS;
  vm = VISITOR_MANAGEMENT_STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;
  formRules = VISITOR_FORM_RULES;
  searchConfig = VISITOR_SEARCH_CONFIG;

  visitorTypes = signal<RefTermOption[]>([]);
  purposeTypes = signal<RefTermOption[]>([]);

  isLoadingVisitorTypes = signal(false);
  isLoadingPurposeTypes = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal('');

  visitorSuggestions = signal<Visitor[]>([]);
  isSearchingVisitors = signal(false);
  isNameFocused = signal(false);
  selectedVisitor = signal<Visitor | null>(null);

  readonly minDateStr = this.toDateOnly(new Date());

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(this.formRules.NAME_MAX_LENGTH)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.formRules.PHONE_PATTERN)]],
    email: ['', [Validators.maxLength(this.formRules.EMAIL_MAX_LENGTH), Validators.email]],
    visitorTypeId: ['', Validators.required],
    purposeTypeId: ['', Validators.required],
    startDate: [null as string | null, Validators.required],
    endDate: [null as string | null, Validators.required],
  });

  ngOnInit(): void {
    this.isLoadingVisitorTypes.set(true);
    this.isLoadingPurposeTypes.set(true);

    this.form.controls.name.valueChanges
      .pipe(
        debounceTime(this.searchConfig.DEBOUNCE_MS),
        distinctUntilChanged(),
        switchMap((term) => {
          this.selectedVisitor.set(null);

          const trimmed = term.trim();
          if (trimmed.length < this.searchConfig.MIN_CHARS) {
            this.visitorSuggestions.set([]);
            this.isSearchingVisitors.set(false);
            return of(null);
          }

          this.isSearchingVisitors.set(true);
          return this.visitorService.getVisitors({
            search: trimmed,
            limit: this.searchConfig.RESULT_LIMIT,
            skipLoader: true,
          });
        }),
      )
      .subscribe({
        next: (response) => {
          if (response) {
            this.visitorSuggestions.set(response.items);
          }
          this.isSearchingVisitors.set(false);
        },
        error: () => {
          this.visitorSuggestions.set([]);
          this.isSearchingVisitors.set(false);
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

  onClose(): void {
    this.dialogRef.close(false);
  }

  onSelectVisitor(visitor: Visitor): void {
    this.selectedVisitor.set(visitor);
    this.form.patchValue(
      {
        name: visitor.name,
        phoneNumber: visitor.phoneNumber,
        email: visitor.email ?? '',
        visitorTypeId: visitor.visitorTypeId,
      },
      { emitEvent: false },
    );
    this.visitorSuggestions.set([]);
    this.isNameFocused.set(false);
  }

  onClearSelectedVisitor(): void {
    this.selectedVisitor.set(null);
    this.form.patchValue(
      { name: '', phoneNumber: '', email: '', visitorTypeId: '' },
      { emitEvent: false },
    );
    this.visitorSuggestions.set([]);
    this.nameInput?.nativeElement.focus();
  }

  onNameFocus(): void {
    this.isNameFocused.set(true);
  }

  onNameBlur(): void {
    setTimeout(() => this.isNameFocused.set(false), 150);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const selectedVisitor = this.selectedVisitor();

    this.visitService
      .createVisit({
        ...(selectedVisitor
          ? { visitorId: selectedVisitor.id }
          : {
              visitor: {
                name: value.name.trim(),
                phoneNumber: value.phoneNumber.trim(),
                email: value.email?.trim() || undefined,
                visitorTypeId: value.visitorTypeId,
              },
            }),
        purposeTypeId: value.purposeTypeId,
        startDate: value.startDate!,
        endDate: value.endDate!,
      })
      .subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.dialogRef.close(true);
        },
        error: () => {
          this.isSubmitting.set(false);
          this.errorMessage.set(this.strings.VISIT_BOOK_FAILED);
        },
      });
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
