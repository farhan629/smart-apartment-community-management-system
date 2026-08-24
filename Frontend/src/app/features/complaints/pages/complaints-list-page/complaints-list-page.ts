import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';

import { APP_CONSTANTS, Role } from '../../../../core/constants/app.constants';
import {
  ASSIGNMENT_STATUS,
  COMPLAINT_DATE_FORMAT,
  COMPLAINT_DETAIL_STRINGS,
  COMPLAINT_DIALOG_CONFIG,
  COMPLAINT_FILTER_KEYS,
  COMPLAINT_LIST_DEFAULTS,
  COMPLAINT_LIST_STRINGS,
  ESCALATION_CHECK_STRINGS,
  VIEW_MODE,
  ViewMode,
} from '../../../../core/constants/complaint.constants';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { AssignmentResponseDto } from '../../../../core/models/assignment.model';
import {
  ComplaintFilterParams,
  ComplaintSummaryDto,
} from '../../../../core/models/complaint.model';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { AuthService } from '../../../../core/services/auth-service';
import { ComplaintService } from '../../../../core/services/complaint.service';
import { JobsService } from '../../../../core/services/jobs.service';
import { LookupService } from '../../../../core/services/lookup.service';
import { PermissionService } from '../../../../core/services/permission.service';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { EmptyState } from '../../../../shared/components/empty-state/empty-state';
import {
  FilterBar,
  FilterDropdownConfig,
  FilterValues,
} from '../../../../shared/components/filter-bar/filter-bar';
import { SearchBar } from '../../../../shared/components/search-bar/search-bar';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { CapitalizeFirstPipe } from '../../../../shared/pipes/capitalize-first.pipe';
import { ComplaintDetailPage } from '../complaint-detail-page/complaint-detail-page';
import { CreateComplaintPage } from '../create-complaint-page/create-complaint-page';

type ComplaintRow = ComplaintSummaryDto | AssignmentResponseDto;

@Component({
  selector: 'app-complaints-list-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    FilterBar,
    SearchBar,
    StatusBadge,
    EmptyState,
    ActionButton,
    CapitalizeFirstPipe,
  ],
  templateUrl: './complaints-list-page.html',
  styleUrl: './complaints-list-page.scss',
})
export class ComplaintsListPage implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly assignmentService = inject(AssignmentService);
  private readonly permissionService = inject(PermissionService);
  private readonly lookupService = inject(LookupService);
  private readonly jobsService = inject(JobsService);

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  viewModeOptions = VIEW_MODE;
  listStrings = COMPLAINT_LIST_STRINGS;
  dateFormat = COMPLAINT_DATE_FORMAT;
  detailStrings = COMPLAINT_DETAIL_STRINGS;
  escalationCheckStrings = ESCALATION_CHECK_STRINGS;

  readonly canRaiseComplaint = computed(
    () =>
      this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_SUBMIT) &&
      this.currentRole == APP_CONSTANTS.ROLES.RESIDENT,
  );

  readonly canRunEscalationCheck = computed(() =>
    this.permissionService.hasPermission(PERMISSIONS.JOB_TRIGGER),
  );

  isRunningEscalationCheck = signal(false);
  escalationCheckMessage = signal<string | null>(null);
  escalationCheckError = signal(false);
  lastEscalatedComplaintIds = signal<Set<string>>(new Set());

  complaints = signal<ComplaintRow[]>([]);
  loading = signal(false);
  error = signal(false);

  pageNumber = signal<number>(COMPLAINT_LIST_DEFAULTS.PAGE_NUMBER);
  pageSize = signal<number>(COMPLAINT_LIST_DEFAULTS.PAGE_SIZE);

  totalCount = signal(0);
  totalPages = signal(0);

  private readonly currentRole =
    (this.authService.getUserRole() as Role) ?? APP_CONSTANTS.ROLES.RESIDENT;
  readonly isStaff = this.currentRole === APP_CONSTANTS.ROLES.STAFF;
  viewMode = signal<ViewMode>(this.isStaff ? VIEW_MODE.MINE : VIEW_MODE.ALL);
  private activeFilters = signal<FilterValues>({});
  private searchTerm = signal('');

  dropdowns = signal<FilterDropdownConfig[]>([
    {
      key: COMPLAINT_FILTER_KEYS.STATUS,
      placeholder: COMPLAINT_LIST_STRINGS.STATUS_FILTER_PLACEHOLDER,
      options: [],
    },
    {
      key: COMPLAINT_FILTER_KEYS.CATEGORY,
      placeholder: COMPLAINT_LIST_STRINGS.CATEGORY_FILTER_PLACEHOLDER,
      options: [],
    },
  ]);

  private loadFilterOptions(): void {
    forkJoin({
      statuses: this.lookupService.getComplaintStatuses(),
      categories: this.lookupService.getCategories(),
    }).subscribe(({ statuses, categories }) => {
      this.dropdowns.set([
        {
          key: COMPLAINT_FILTER_KEYS.STATUS,
          placeholder: COMPLAINT_LIST_STRINGS.STATUS_FILTER_PLACEHOLDER,
          options: statuses.map((status) => ({
            label: status.displayName,
            value: status.code,
          })),
        },
        {
          key: COMPLAINT_FILTER_KEYS.CATEGORY,
          placeholder: COMPLAINT_LIST_STRINGS.CATEGORY_FILTER_PLACEHOLDER,
          options: categories.map((category) => ({
            label: category.name,
            value: category.id,
          })),
        },
      ]);
    });
  }

  readonly visibleComplaints = computed(() => {
    const complaints = this.complaints();
    const term = this.searchTerm().trim().toLowerCase();

    if (!term || this.isAssignmentRow(complaints[0])) {
      return complaints;
    }

    return (complaints as ComplaintSummaryDto[]).filter(
      (complaint) =>
        complaint.description.toLowerCase().includes(term) ||
        complaint.category.toLowerCase().includes(term),
    );
  });

  constructor(
    private readonly complaintService: ComplaintService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    this.loadComplaints();
    this.loadFilterOptions();
  }

  loadComplaints(): void {
    this.loading.set(true);
    this.error.set(false);

    if (this.isStaff && this.viewMode() === VIEW_MODE.MINE) {
      this.loadMyAssignments();
      return;
    }

    const params: ComplaintFilterParams = {
      ...this.activeFilters(),
      page: this.pageNumber(),
      limit: this.pageSize(),
    };

    this.complaintService.getComplaints(params).subscribe({
      next: (result) => {
        this.complaints.set(result.items);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }

  private loadMyAssignments(): void {
    this.assignmentService.getMyAssignments(this.pageNumber(), this.pageSize()).subscribe({
      next: (result) => {
        this.complaints.set(result.items);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }

  setViewMode(mode: ViewMode): void {
    if (this.viewMode() === mode) {
      return;
    }
    this.viewMode.set(mode);
    this.pageNumber.set(COMPLAINT_LIST_DEFAULTS.PAGE_NUMBER);
    this.loadComplaints();
  }

  isAssignmentRow(row: ComplaintRow | undefined): row is AssignmentResponseDto {
    return !!row && 'assignmentId' in row;
  }

  isPendingAssignment(row: ComplaintRow): boolean {
    return this.isAssignmentRow(row) && row.status === ASSIGNMENT_STATUS.PENDING;
  }

  acceptAssignment(row: AssignmentResponseDto): void {
    this.assignmentService.accept(row.complaintId, row.assignmentId).subscribe({
      next: () => this.loadComplaints(),
      error: () => this.error.set(true),
    });
  }

  denyAssignment(row: AssignmentResponseDto): void {
    const denialReason = window.prompt(COMPLAINT_LIST_STRINGS.DENY_REASON_PROMPT);
    if (!denialReason) {
      return;
    }
    this.assignmentService.deny(row.complaintId, row.assignmentId, { denialReason }).subscribe({
      next: () => this.loadComplaints(),
      error: () => this.error.set(true),
    });
  }

  onFilterChange(filters: FilterValues): void {
    this.activeFilters.set(filters);
    this.pageNumber.set(COMPLAINT_LIST_DEFAULTS.PAGE_NUMBER);
    this.loadComplaints();
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.pageNumber()) {
      return;
    }

    this.pageNumber.set(page);
    this.loadComplaints();
  }

  viewComplaint(complaintId: string): void {
    const dialogRef = this.dialog.open(ComplaintDetailPage, {
      data: { complaintId },
      panelClass: COMPLAINT_DIALOG_CONFIG.DETAIL_PANEL_CLASS,
      backdropClass: COMPLAINT_DIALOG_CONFIG.DETAIL_BACKDROP_CLASS,
      width: COMPLAINT_DIALOG_CONFIG.DETAIL_WIDTH,
      height: COMPLAINT_DIALOG_CONFIG.DETAIL_HEIGHT,
      position: { top: '0', right: '0' },
      enterAnimationDuration: '0ms',
      exitAnimationDuration: '0ms',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (changed) {
        this.loadComplaints();
      }
    });
  }

  runEscalationCheck(): void {
    if (this.isRunningEscalationCheck()) {
      return;
    }

    this.isRunningEscalationCheck.set(true);
    this.escalationCheckMessage.set(null);
    this.escalationCheckError.set(false);
    this.lastEscalatedComplaintIds.set(new Set());

    this.jobsService.runEscalationCheck().subscribe({
      next: (result) => {
        this.isRunningEscalationCheck.set(false);
        this.escalationCheckMessage.set(
          result.escalatedCount > 0
            ? this.escalationCheckStrings.SUCCESS_MESSAGE(result.escalatedCount)
            : this.escalationCheckStrings.NO_ESCALATIONS_MESSAGE,
        );
        if (result.escalatedCount > 0) {
          this.lastEscalatedComplaintIds.set(new Set(result.escalatedComplaintIds));
          this.loadComplaints();
        }
      },
      error: () => {
        this.isRunningEscalationCheck.set(false);
        this.escalationCheckError.set(true);
        this.escalationCheckMessage.set(this.escalationCheckStrings.ERROR_MESSAGE);
      },
    });
  }

  wasJustEscalated(complaintId: string): boolean {
    return this.lastEscalatedComplaintIds().has(complaintId);
  }

  raiseComplaint(): void {
    const dialogRef = this.dialog.open(CreateComplaintPage, {
      width: COMPLAINT_DIALOG_CONFIG.CREATE_WIDTH,
      maxWidth: COMPLAINT_DIALOG_CONFIG.CREATE_MAX_WIDTH,
      panelClass: COMPLAINT_DIALOG_CONFIG.CREATE_PANEL_CLASS,
      autoFocus: false,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) {
        this.loadComplaints();
      }
    });
  }
}
