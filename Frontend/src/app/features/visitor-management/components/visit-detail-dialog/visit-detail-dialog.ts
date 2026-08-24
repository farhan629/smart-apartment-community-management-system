import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit, computed, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  VISIT_STATUS_LABELS,
  CANCELLABLE_VISIT_STATUSES,
  APPROVABLE_VISIT_STATUSES,
} from '../../../../core/constants/visit.constants';
import {
  VISITOR_MANAGEMENT_STRINGS,
  VISITOR_MANAGEMENT_ICONS,
} from '../../../../core/constants/visitor-management-ui.constants';
import { Visit } from '../../../../core/models/visit.model';
import { VisitService } from '../../../../core/services/visit.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { RejectVisitDialog } from '../../../../shared/components/reject-visit-dialog/reject-visit-dialog';
import { UpdateVisitorDialog } from '../update-visitor-dialog/update-visitor-dialog';
import { finalize, timeout, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export interface VisitDetailDialogData {
  visitId: string;
}

@Component({
  selector: 'app-visit-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    ActionButton,
    StatusBadge,
  ],
  templateUrl: './visit-detail-dialog.html',
  styleUrl: './visit-detail-dialog.scss',
})
export class VisitDetailDialog implements OnInit {
  private readonly visitService = inject(VisitService);
  private readonly dialog = inject(MatDialog);

  strings = APP_CONSTANTS.STRINGS;
  vm = VISITOR_MANAGEMENT_STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;

  visit = signal<Visit | null>(null);
  isLoading = signal(false);
  isActing = signal(false);
  errorMessage = signal('');

  private changed = false;

  readonly canCancel = computed(
    () => !!this.visit() && (CANCELLABLE_VISIT_STATUSES as string[]).includes(this.visit()!.status),
  );

  readonly canApprove = computed(
    () => !!this.visit() && (APPROVABLE_VISIT_STATUSES as string[]).includes(this.visit()!.status),
  );

  readonly qrSent = computed(() => !!this.visit()?.qrToken?.isActive);

  constructor(
    public dialogRef: MatDialogRef<VisitDetailDialog, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: VisitDetailDialogData,
  ) {}

  ngOnInit(): void {
    this.loadVisit();
  }

  loadVisit(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.visitService
      .getVisitById(this.data.visitId)
      .pipe(
        timeout(10000),
        catchError((err) => {
          console.error('[VisitDetailDialog] getVisitById failed:', err);
          return throwError(() => err);
        }),
        finalize(() => this.isLoading.set(false)),
      )
      .subscribe({
        next: (visit) => {
          this.visit.set(visit);
        },
        error: () => this.errorMessage.set(this.strings.VISITS_LOAD_FAILED),
      });
  }

  getStatusLabel(status: string): string {
    return (VISIT_STATUS_LABELS as Record<string, string>)[status] ?? status;
  }

  onUpdateVisitor(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(UpdateVisitorDialog, {
      width: '32rem',
      data: { visitorId: visit.visitorId },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.changed = true;
        this.loadVisit();
      }
    });
  }

  onApproveVisit(): void {
    const visit = this.visit();
    if (!visit) return;

    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.vm.APPROVE_VISIT_TITLE,
        message: this.vm.APPROVE_VISIT_MESSAGE,
        variant: 'primary',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.isActing.set(true);
        this.visitService.approveVisit(visit.id).subscribe({
          next: (updated) => {
            this.visit.set(updated);
            this.changed = true;
            this.isActing.set(false);
          },
          error: () => {
            this.errorMessage.set(this.strings.VISIT_APPROVE_FAILED);
            this.isActing.set(false);
          },
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
        this.isActing.set(true);
        this.visitService.rejectVisit(visit.id, { rejectionReason }).subscribe({
          next: (updated) => {
            this.visit.set(updated);
            this.changed = true;
            this.isActing.set(false);
          },
          error: () => {
            this.errorMessage.set(this.strings.VISIT_REJECT_FAILED);
            this.isActing.set(false);
          },
        });
      }
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
        this.isActing.set(true);
        this.visitService.cancelVisit(visit.id).subscribe({
          next: () => {
            this.changed = true;
            this.close();
          },
          error: () => {
            this.errorMessage.set(this.strings.VISIT_CANCEL_FAILED);
            this.isActing.set(false);
          },
        });
      }
    });
  }

  close(): void {
    this.dialogRef.close(this.changed);
  }
}
