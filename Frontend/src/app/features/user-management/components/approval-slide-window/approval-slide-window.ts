import { Component, inject, output, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, takeUntil } from 'rxjs';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { ApprovalService } from '../../../../core/services/approval.service';
import { ApprovalDetailDto } from '../../../../core/models/approval.models';

@Component({
  selector: 'app-approval-slide-window',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './approval-slide-window.html',
  styleUrl: './approval-slide-window.scss',
})
export class ApprovalSlideWindow implements OnInit, OnDestroy {
  private readonly approvalService = inject(ApprovalService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly destroy$ = new Subject<void>();

  readonly strings = APP_CONSTANTS.STRINGS;
  readonly icons = APP_CONSTANTS.ICONS;
  readonly closed = output<void>();

  readonly approvals = signal<ApprovalDetailDto[]>([]);
  readonly loading = signal(false);

  ngOnInit(): void {
    this.loadApprovals();
  }

  private loadApprovals(): void {
    this.loading.set(true);
    this.approvalService.getApprovals('pending').subscribe({
      next: (res) => {
        this.approvals.set(res.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  onClose(): void {
    this.closed.emit();
  }

  onApprove(approval: ApprovalDetailDto): void {
    this.approvalService.updateApproval(approval.id, { isApproved: true }).subscribe({
      next: () => {
        this.approvals.update((list) => list.filter((a) => a.id !== approval.id));
        this.snackBar.open(this.strings.USER_APPROVED_SUCCESS, this.strings.CLOSE, {
          duration: 3000,
          panelClass: 'snackbar-success',
        });
      },
      error: () => {
        this.snackBar.open(this.strings.FAILED_TO_APPROVE_USER, this.strings.CLOSE, {
          duration: 5000,
          panelClass: 'snackbar-error',
        });
      },
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
