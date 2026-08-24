import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  VISIT_STATUS_LABELS,
  EDITABLE_VISIT_STATUSES,
  CANCELLABLE_VISIT_STATUSES,
  APPROVABLE_VISIT_STATUSES,
} from '../../../../core/constants/visit.constants';
import { Visit } from '../../../../core/models/visit.model';
import { RefTermOption } from '../../../../core/models/visitor.model';
import { VisitService } from '../../../../core/services/visit.service';
import { VisitorService } from '../../../../core/services/visitor.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { RejectVisitDialog } from '../../../../shared/components/reject-visit-dialog/reject-visit-dialog';
import { UpdateVisitorDialog } from '../../components/update-visitor-dialog/update-visitor-dialog';

@Component({
  selector: 'app-visit-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    ActionButton,
    StatusBadge,
  ],
  templateUrl: './visit-detail-page.html',
  styleUrl: './visit-detail-page.scss',
})
export class VisitDetailPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly visitService = inject(VisitService);
  private readonly visitorService = inject(VisitorService);
  private readonly dialog = inject(MatDialog);

  strings = APP_CONSTANTS.STRINGS;

  visit = signal<Visit | null>(null);
  purposeTypes = signal<RefTermOption[]>([]);

  isLoading = signal(false);
  isSaving = signal(false);
  isEditing = signal(false);
  errorMessage = signal('');

  editForm = this.fb.group({
    purposeTypeId: ['', Validators.required],
    startDate: [null as Date | null, Validators.required],
    endDate: [null as Date | null, Validators.required],
  });

  readonly canEdit = computed(
    () => !!this.visit() && (EDITABLE_VISIT_STATUSES as string[]).includes(this.visit()!.status),
  );

  readonly canCancel = computed(
    () => !!this.visit() && (CANCELLABLE_VISIT_STATUSES as string[]).includes(this.visit()!.status),
  );

  readonly canApprove = computed(
    () => !!this.visit() && (APPROVABLE_VISIT_STATUSES as string[]).includes(this.visit()!.status),
  );

  readonly qrSent = computed(() => !!this.visit()?.qrToken?.isActive);

  ngOnInit(): void {
    this.visitorService.getPurposeTypes().subscribe({
      next: (types) => this.purposeTypes.set(types),
    });

    const visitId =
      this.route.snapshot.paramMap.get('visitId') ?? this.route.snapshot.paramMap.get('id');
    if (visitId) {
      this.loadVisit(visitId);
    } else {
      this.errorMessage.set(this.strings.VISITS_LOAD_FAILED);
    }
  }

  loadVisit(id: string): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.visitService.getVisitById(id).subscribe({
      next: (visit) => {
        this.visit.set(visit);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set(this.strings.VISITS_LOAD_FAILED);
        this.isLoading.set(false);
      },
    });
  }

  getStatusLabel(status: string): string {
    return (VISIT_STATUS_LABELS as Record<string, string>)[status] ?? status;
  }

  onBack(): void {
    this.router.navigate(['/visitors']);
  }

  onUpdateVisitor(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(UpdateVisitorDialog, {
      width: '32rem',
      data: {
        visitorId: visit.visitorId,
        name: visit.visitorName,
        phoneNumber: visit.visitorPhoneNumber,
        email: visit.visitorEmail,
        visitorTypeId: visit.visitorType,
      },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated && this.visit()) {
        this.loadVisit(this.visit()!.id);
      }
    });
  }

  onApproveVisit(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.strings.APPROVE_VISIT_TITLE,
        message: this.strings.APPROVE_VISIT_MESSAGE,
        variant: 'primary',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.visitService.approveVisit(visit.id).subscribe({
          next: (updated) => this.visit.set(updated),
          error: () => this.errorMessage.set(this.strings.VISIT_APPROVE_FAILED),
        });
      }
    });
  }

  onRejectVisit(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(RejectVisitDialog, { width: '30rem' });

    dialogRef.afterClosed().subscribe((rejectionReason) => {
      if (rejectionReason) {
        this.visitService.rejectVisit(visit.id, { rejectionReason }).subscribe({
          next: (updated) => this.visit.set(updated),
          error: () => this.errorMessage.set(this.strings.VISIT_REJECT_FAILED),
        });
      }
    });
  }

  onStartEdit(): void {
    const visit = this.visit();
    if (!visit) return;

    this.editForm.setValue({
      purposeTypeId: visit.purposeTypeId,
      startDate: new Date(visit.startDate),
      endDate: new Date(visit.endDate),
    });
    this.isEditing.set(true);
  }

  onCancelEdit(): void {
    this.isEditing.set(false);
  }

  onSaveEdit(): void {
    const visit = this.visit();
    if (this.editForm.invalid || !visit) {
      this.editForm.markAllAsTouched();
      return;
    }

    const value = this.editForm.getRawValue();
    this.isSaving.set(true);

    this.visitService
      .updateVisit(visit.id, {
        purposeTypeId: value.purposeTypeId!,
        startDate: this.toDateOnly(value.startDate!),
        endDate: this.toDateOnly(value.endDate!),
      })
      .subscribe({
        next: (updated) => {
          this.visit.set(updated);
          this.isEditing.set(false);
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
          this.errorMessage.set(this.strings.VISIT_UPDATE_FAILED);
        },
      });
  }

  onCancelVisit(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.strings.CANCEL_VISIT_TITLE,
        message: this.strings.CANCEL_VISIT_MESSAGE,
        variant: 'danger',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.visitService.cancelVisit(visit.id).subscribe({
          next: () => this.router.navigate(['/visitors']),
          error: () => this.errorMessage.set(this.strings.VISIT_CANCEL_FAILED),
        });
      }
    });
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
