import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  HostBinding,
  Inject,
  NgZone,
  OnInit,
  Optional,
  Renderer2,
  computed,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';

import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { CapitalizeFirstPipe } from '../../../../shared/pipes/capitalize-first.pipe';
import {
  AssignStaffDialog,
  AssignStaffDialogData,
} from '../../components/assign-staff-dialog/assign-staff-dialog';
import { ComplaintComments } from '../../components/complaint-comments/complaint-comments';
import { ComplaintEscalation } from '../../components/complaint-escalation/complaint-escalation';

import { APP_CONSTANTS, Role } from '../../../../core/constants/app.constants';
import {
  ASSIGNMENT_MODE,
  ASSIGNMENT_STATUS,
  ASSIGN_STAFF_DIALOG_CONFIG,
  COMPLAINT_DETAIL_STRINGS,
  COMPLAINT_ROUTE_PARAM,
  COMPLAINT_STATUS,
  KEYBOARD_KEYS,
} from '../../../../core/constants/complaint.constants';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import {
  AssignmentResponseDto,
  ResidentFlatResponseDto,
} from '../../../../core/models/assignment.model';
import { ComplaintDetailDto } from '../../../../core/models/complaint.model';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { AuthService } from '../../../../core/services/auth-service';
import { ComplaintService } from '../../../../core/services/complaint.service';
import { PermissionService } from '../../../../core/services/permission.service';

export interface ComplaintDetailDialogData {
  complaintId: string;
}

@Component({
  selector: 'app-complaint-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    ActionButton,
    StatusBadge,
    ComplaintComments,
    ComplaintEscalation,
    CapitalizeFirstPipe,
  ],
  templateUrl: './complaint-detail-page.html',
  styleUrl: './complaint-detail-page.scss',
})
export class ComplaintDetailPage implements OnInit {
  isPanelMode = false;

  @HostBinding('class.detail--panel') get panelHostClass(): boolean {
    return this.isPanelMode;
  }

  private visible = false;

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  detailStrings = COMPLAINT_DETAIL_STRINGS;
  complaintStatus = COMPLAINT_STATUS;
  assignmentStatus = ASSIGNMENT_STATUS;
  assignmentMode = ASSIGNMENT_MODE;

  complaint = signal<ComplaintDetailDto | null>(null);
  loading = signal(true);
  error = signal(false);

  showCancelForm = signal(false);
  cancelReason = signal('');
  isCancelling = signal(false);
  cancelError = signal<string | null>(null);

  assignments = signal<AssignmentResponseDto[]>([]);
  isUpdatingStatus = signal(false);
  statusError = signal<string | null>(null);

  residentFlat = signal<ResidentFlatResponseDto | null>(null);

  private changed = false;

  private readonly cancellableStatuses = new Set<string>([
    COMPLAINT_STATUS.OPEN,
    COMPLAINT_STATUS.ASSIGNED,
  ]);
  private readonly terminalStatuses = new Set<string>([
    COMPLAINT_STATUS.RESOLVED,
    COMPLAINT_STATUS.CLOSED,
    COMPLAINT_STATUS.CANCELLED,
  ]);

  private readonly assignmentService = inject(AssignmentService);
  private readonly permissionService = inject(PermissionService);
  private readonly authService = inject(AuthService);
  private readonly dialog = inject(MatDialog);
  private readonly elementRef = inject(ElementRef<HTMLElement>);
  private readonly renderer = inject(Renderer2);
  private readonly ngZone = inject(NgZone);

  private readonly currentRole =
    (this.authService.getUserRole() as Role) ?? APP_CONSTANTS.ROLES.RESIDENT;

  readonly canCancel = computed(() => {
    const complaint = this.complaint();
    return (
      !!complaint &&
      this.cancellableStatuses.has(complaint.status) &&
      this.currentRole !== APP_CONSTANTS.ROLES.ADMIN
    );
  });

  readonly latestAssignment = computed(() => {
    const list = this.assignments();
    return list.length ? list[list.length - 1] : null;
  });

  readonly canAssign = computed(() => {
    const complaint = this.complaint();
    return (
      !!complaint &&
      complaint.status === COMPLAINT_STATUS.OPEN &&
      this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_ASSIGN)
    );
  });

  readonly canReassign = computed(() => {
    const complaint = this.complaint();
    return (
      !!complaint &&
      !!this.latestAssignment() &&
      complaint.status !== COMPLAINT_STATUS.OPEN &&
      !this.terminalStatuses.has(complaint.status) &&
      this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_ASSIGN)
    );
  });

  readonly nextStatusAction = computed(
    (): {
      label: string;
      value: typeof COMPLAINT_STATUS.IN_PROGRESS | typeof COMPLAINT_STATUS.RESOLVED;
    } | null => {
      const complaint = this.complaint();
      if (!complaint || !this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_MANAGE)) {
        return null;
      }
      if (complaint.status === COMPLAINT_STATUS.ASSIGNED) {
        return {
          label: this.detailStrings.START_PROGRESS_LABEL,
          value: COMPLAINT_STATUS.IN_PROGRESS,
        };
      }
      if (complaint.status === COMPLAINT_STATUS.IN_PROGRESS) {
        return {
          label: this.detailStrings.MARK_RESOLVED_LABEL,
          value: COMPLAINT_STATUS.RESOLVED,
        };
      }
      return null;
    },
  );

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly complaintService: ComplaintService,
    @Optional()
    @Inject(MAT_DIALOG_DATA)
    private readonly dialogData: ComplaintDetailDialogData | null,
    @Optional() private readonly dialogRef: MatDialogRef<ComplaintDetailPage, boolean> | null,
  ) {
    this.isPanelMode = !!this.dialogData;
  }

  ngOnInit(): void {
    const complaintId = this.isPanelMode
      ? this.dialogData!.complaintId
      : this.route.snapshot.paramMap.get(COMPLAINT_ROUTE_PARAM);

    if (this.isPanelMode) {
      this.dialogRef!.afterOpened().subscribe(() => {
        this.ngZone.runOutsideAngular(() => {
          requestAnimationFrame(() => {
            requestAnimationFrame(() => {
              this.visible = true;
              this.renderer.addClass(this.elementRef.nativeElement, 'detail--visible');
            });
          });
        });
      });

      this.dialogRef!.backdropClick().subscribe(() => {
        this.close();
      });
      this.dialogRef!.keydownEvents().subscribe((event) => {
        if (event.key === KEYBOARD_KEYS.ESCAPE) {
          this.close();
        }
      });
    }

    if (!complaintId) {
      this.error.set(true);
      this.loading.set(false);
      return;
    }

    this.fetchComplaint(complaintId);
  }

  private fetchComplaint(complaintId: string): void {
    this.loading.set(true);
    this.error.set(false);

    this.complaintService.getById(complaintId).subscribe({
      next: (result) => {
        this.complaint.set(result);
        this.loading.set(false);
        this.loadAssignments(complaintId);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }

  private loadAssignments(complaintId: string): void {
    this.assignmentService.getHistory(complaintId).subscribe({
      next: (history) => {
        this.assignments.set(history);
        this.loadResidentFlat(complaintId, history);
      },
      error: () => this.assignments.set([]),
    });
  }

  private loadResidentFlat(complaintId: string, history: AssignmentResponseDto[]): void {
    const isAdmin = this.authService.getUserRole() === APP_CONSTANTS.ROLES.ADMIN;
    const assignmentId = history.length ? history[history.length - 1].assignmentId : undefined;

    if (!isAdmin && !assignmentId) {
      return;
    }

    this.assignmentService.getResidentFlat(complaintId, assignmentId).subscribe({
      next: (flat) => this.residentFlat.set(flat),
      error: () => this.residentFlat.set(null),
    });
  }

  openAssignDialog(mode: typeof ASSIGNMENT_MODE.ASSIGN | typeof ASSIGNMENT_MODE.REASSIGN): void {
    const complaint = this.complaint();
    if (!complaint) {
      return;
    }

    const data: AssignStaffDialogData = {
      complaintId: complaint.complaintId,
      category: complaint.category,
      mode,
      assignmentId:
        mode === ASSIGNMENT_MODE.REASSIGN
          ? (this.latestAssignment()?.assignmentId ?? undefined)
          : undefined,
    };

    const dialogRef = this.dialog.open(AssignStaffDialog, {
      width: ASSIGN_STAFF_DIALOG_CONFIG.WIDTH,
      maxWidth: ASSIGN_STAFF_DIALOG_CONFIG.MAX_WIDTH,
      autoFocus: false,
      data,
    });

    dialogRef.afterClosed().subscribe((saved) => {
      if (saved) {
        this.changed = true;
        this.fetchComplaint(complaint.complaintId);
      }
    });
  }

  updateStatus(
    status: typeof COMPLAINT_STATUS.IN_PROGRESS | typeof COMPLAINT_STATUS.RESOLVED,
  ): void {
    const complaint = this.complaint();
    if (!complaint) {
      return;
    }

    this.isUpdatingStatus.set(true);
    this.statusError.set(null);

    this.complaintService.updateStatus(complaint.complaintId, { status }).subscribe({
      next: (updated) => {
        this.complaint.set(updated);
        this.isUpdatingStatus.set(false);
        this.changed = true;
      },
      error: () => {
        this.isUpdatingStatus.set(false);
        this.statusError.set(this.detailStrings.STATUS_UPDATE_ERROR);
      },
    });
  }

  openCancelForm(): void {
    this.cancelError.set(null);
    this.cancelReason.set('');
    this.showCancelForm.set(true);
  }

  closeCancelForm(): void {
    this.showCancelForm.set(false);
  }

  confirmCancel(): void {
    const complaint = this.complaint();
    const reason = this.cancelReason().trim();

    if (!complaint || reason.length === 0) {
      this.cancelError.set(this.detailStrings.CANCEL_REASON_REQUIRED_ERROR);
      return;
    }

    this.isCancelling.set(true);
    this.cancelError.set(null);

    this.complaintService
      .cancel(complaint.complaintId, {
        cancellationReason: reason,
      })
      .subscribe({
        next: (updated) => {
          this.complaint.set(updated);
          this.isCancelling.set(false);
          this.showCancelForm.set(false);
          this.changed = true;
        },
        error: () => {
          this.isCancelling.set(false);
          this.cancelError.set(this.detailStrings.CANCEL_ERROR);
        },
      });
  }

  goBack(): void {
    if (this.isPanelMode) {
      this.close();
      return;
    }

    this.router.navigate([APP_CONSTANTS.ROUTES.COMPLAINTS]);
  }

  close(): void {
    this.visible = false;
    this.renderer.removeClass(this.elementRef.nativeElement, 'detail--visible');
    setTimeout(() => {
      this.dialogRef!.close(this.changed);
    }, 600);
  }
}
