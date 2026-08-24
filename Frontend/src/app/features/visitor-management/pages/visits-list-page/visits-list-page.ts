import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTabsModule } from '@angular/material/tabs';
import { forkJoin } from 'rxjs';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  APPROVABLE_VISIT_STATUSES,
  CANCELLABLE_VISIT_STATUSES,
  EDITABLE_VISIT_STATUSES,
  HISTORY_VISIT_STATUSES,
  UPCOMING_VISIT_STATUSES,
  VISIT_STATUS_LABELS,
  VISIT_TABS,
  VisitTab,
} from '../../../../core/constants/visit.constants';
import {
  VISITOR_MANAGEMENT_ICONS,
  VISITOR_MANAGEMENT_STRINGS,
  VISIT_LIST_DEFAULT_PAGE_SIZE,
  VISIT_LIST_MERGE_FETCH_LIMIT,
  VISIT_LIST_PAGE_SIZE_OPTIONS,
  VISIT_STATUS_FILTER_OPTIONS,
} from '../../../../core/constants/visitor-management-ui.constants';
import { GetVisitsFilters, Visit } from '../../../../core/models/visit.model';
import { VisitService } from '../../../../core/services/visit.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { RejectVisitDialog } from '../../../../shared/components/reject-visit-dialog/reject-visit-dialog';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { BookVisitDialog } from '../../components/book-visit-dialog/book-visit-dialog';
import { UpdateVisitorDialog } from '../../components/update-visitor-dialog/update-visitor-dialog';
import { VisitDetailDialog } from '../../components/visit-detail-dialog/visit-detail-dialog';

@Component({
  selector: 'app-visits-list-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatIconModule,
    MatPaginatorModule,
    ActionButton,
    StatusBadge,
  ],
  templateUrl: './visits-list-page.html',
  styleUrl: './visits-list-page.scss',
})
export class VisitsListPage implements OnInit {
  private readonly visitService = inject(VisitService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);

  strings = APP_CONSTANTS.STRINGS;
  vm = VISITOR_MANAGEMENT_STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;
  tabs = VISIT_TABS;
  cancellableStatuses = CANCELLABLE_VISIT_STATUSES;
  approvableStatuses = APPROVABLE_VISIT_STATUSES;
  editableStatuses = EDITABLE_VISIT_STATUSES;
  pageSizeOptions = VISIT_LIST_PAGE_SIZE_OPTIONS;

  activeTab = signal<VisitTab>(VISIT_TABS.UPCOMING);

  filterForm = this.fb.nonNullable.group({
    status: [''],
    fromDate: [null as string | null],
    toDate: [null as string | null],
  });

  get statusOptions() {
    return this.activeTab() === this.tabs.UPCOMING
      ? VISIT_STATUS_FILTER_OPTIONS.UPCOMING
      : VISIT_STATUS_FILTER_OPTIONS.HISTORY;
  }

  private mergedVisits: Visit[] = [];
  displayedVisits = signal<Visit[]>([]);
  totalCount = signal(0);

  pageIndex = signal(0);
  pageSize = signal(VISIT_LIST_DEFAULT_PAGE_SIZE);

  isLoading = signal(false);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadVisits();
  }

  getStatusLabel(status: string): string {
    return (VISIT_STATUS_LABELS as Record<string, string>)[status] ?? status;
  }

  onTabChange(index: number): void {
    this.activeTab.set(index === 0 ? this.tabs.UPCOMING : this.tabs.HISTORY);
    this.filterForm.reset({ status: '', fromDate: null, toDate: null });
    this.pageIndex.set(0);
    this.loadVisits();
  }

  onApplyFilters(): void {
    this.pageIndex.set(0);
    this.loadVisits();
  }

  onClearFilters(): void {
    this.filterForm.reset({ status: '', fromDate: null, toDate: null });
    this.pageIndex.set(0);
    this.loadVisits();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
    this.applyPagination();
  }

  loadVisits(): void {
    const { status, fromDate, toDate } = this.filterForm.getRawValue();
    const statuses = status ? [status] : this.tabStatuses;
    const sortOrder = this.activeTab() === this.tabs.UPCOMING ? 'asc' : 'desc';

    const baseFilters: Omit<GetVisitsFilters, 'status'> = {
      startDate: fromDate || undefined,
      endDate: toDate || undefined,
      sortBy: 'startDate',
      sortOrder,
      page: 1,
      limit: VISIT_LIST_MERGE_FETCH_LIMIT,
    };

    this.isLoading.set(true);
    this.errorMessage.set('');

    forkJoin(
      statuses.map((s) => this.visitService.getVisits({ ...baseFilters, status: s })),
    ).subscribe({
      next: (responses) => {
        const items = responses.flatMap((r) => r?.items ?? []);
        items.sort((a, b) => {
          const diff = new Date(a.startDate).getTime() - new Date(b.startDate).getTime();
          return sortOrder === 'asc' ? diff : -diff;
        });
        this.mergedVisits = items;
        this.isLoading.set(false);
        this.applyPagination();
      },
      error: () => {
        this.mergedVisits = [];
        this.errorMessage.set(this.strings.VISITS_LOAD_FAILED);
        this.isLoading.set(false);
        this.applyPagination();
      },
    });
  }

  private applyPagination(): void {
    this.totalCount.set(this.mergedVisits.length);
    const start = this.pageIndex() * this.pageSize();
    this.displayedVisits.set(this.mergedVisits.slice(start, start + this.pageSize()));
  }

  private get tabStatuses(): string[] {
    return this.activeTab() === this.tabs.UPCOMING
      ? UPCOMING_VISIT_STATUSES
      : HISTORY_VISIT_STATUSES;
  }

  canCancel(visit: Visit): boolean {
    return (this.cancellableStatuses as string[]).includes(visit.status);
  }

  canApprove(visit: Visit): boolean {
    return (this.approvableStatuses as string[]).includes(visit.status);
  }

  canUpdate(visit: Visit): boolean {
    return (this.editableStatuses as string[]).includes(visit.status);
  }

  onViewDetail(visit: Visit): void {
    const dialogRef = this.dialog.open(VisitDetailDialog, {
      width: '32rem',
      data: { visitId: visit.id },
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (changed) this.loadVisits();
    });
  }

  onBookVisitor(): void {
    const dialogRef = this.dialog.open(BookVisitDialog, { width: '34rem' });

    dialogRef.afterClosed().subscribe((booked) => {
      if (booked) this.loadVisits();
    });
  }

  onCancelVisit(visit: Visit): void {
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
          next: () => this.loadVisits(),
          error: () => this.errorMessage.set(this.strings.VISIT_CANCEL_FAILED),
        });
      }
    });
  }

  onApproveVisit(visit: Visit): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.vm.APPROVE_VISIT_TITLE,
        message: this.vm.APPROVE_VISIT_MESSAGE,
        variant: 'primary',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.visitService.approveVisit(visit.id).subscribe({
          next: () => this.loadVisits(),
          error: () => this.errorMessage.set(this.strings.VISIT_APPROVE_FAILED),
        });
      }
    });
  }

  onUpdateVisitor(visit: Visit): void {
    const dialogRef = this.dialog.open(UpdateVisitorDialog, {
      width: '32rem',
      data: { visitorId: visit.visitorId },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) this.loadVisits();
    });
  }

  onRejectVisit(visit: Visit): void {
    const dialogRef = this.dialog.open(RejectVisitDialog, { width: '30rem' });

    dialogRef.afterClosed().subscribe((rejectionReason) => {
      if (rejectionReason) {
        this.visitService.rejectVisit(visit.id, { rejectionReason }).subscribe({
          next: () => this.loadVisits(),
          error: () => this.errorMessage.set(this.strings.VISIT_REJECT_FAILED),
        });
      }
    });
  }
}
