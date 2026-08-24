import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  inject,
  Input,
  OnChanges,
  OnInit,
  signal,
  SimpleChanges,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { ActionButton } from '../../../../shared/components/action-button/action-button';

import { APP_CONSTANTS, Role } from '../../../../core/constants/app.constants';
import {
  COMPLAINT_DATE_FORMAT,
  COMPLAINT_DATETIME_FORMAT,
  COMPLAINT_STATUS,
  ESCALATION_STRINGS,
  ESCALATION_VALIDATION,
} from '../../../../core/constants/complaint.constants';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { EscalationDto } from '../../../../core/models/escalation.model';
import { AuthService } from '../../../../core/services/auth-service';
import { EscalationService } from '../../../../core/services/escalation.service';
import { PermissionService } from '../../../../core/services/permission.service';

@Component({
  selector: 'app-complaint-escalation',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, ActionButton],
  templateUrl: './complaint-escalation.html',
  styleUrl: './complaint-escalation.scss',
})
export class ComplaintEscalation implements OnInit, OnChanges {
  @Input({ required: true }) complaintId = '';
  @Input() complaintStatus = '';

  private readonly escalationService = inject(EscalationService);
  private readonly permissionService = inject(PermissionService);
  private readonly authService = inject(AuthService);

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  escalationStrings = ESCALATION_STRINGS;
  reasonMaxLength = ESCALATION_VALIDATION.REASON_MAX_LENGTH;
  dateFormat = COMPLAINT_DATE_FORMAT;
  dateTimeFormat = COMPLAINT_DATETIME_FORMAT;

  readonly today = new Date().toISOString().split('T')[0];

  private readonly currentRole =
    (this.authService.getUserRole() as Role) ?? APP_CONSTANTS.ROLES.RESIDENT;

  readonly canEscalate =
    this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_ESCALATE) &&
    this.currentRole !== APP_CONSTANTS.ROLES.ADMIN;
  readonly canManage = this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_MANAGE);

  loading = signal(false);
  loadError = signal(false);

  escalation = signal<EscalationDto | null>(null);

  reason = signal('');
  isEscalating = signal(false);
  escalateError = signal<string | null>(null);

  resolutionDate = signal<string>('');
  isUpdating = signal(false);
  updateError = signal<string | null>(null);

  readonly isComplaintClosed = computed(
    () =>
      this.complaintStatus === COMPLAINT_STATUS.RESOLVED ||
      this.complaintStatus === COMPLAINT_STATUS.CANCELLED,
  );

  readonly canShowEscalateForm = computed(() => this.canEscalate && !this.isComplaintClosed());

  readonly hasContent = computed(() => !!this.escalation() || this.canShowEscalateForm());

  readonly escalateSubmitLabel = computed(() =>
    this.escalation()
      ? this.escalationStrings.RE_ESCALATE_SUBMIT_LABEL
      : this.escalationStrings.ESCALATE_SUBMIT_LABEL,
  );

  ngOnInit(): void {
    this.loadEscalation();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['complaintId'] && !changes['complaintId'].firstChange) {
      this.loadEscalation();
    }
  }

  submitEscalation(): void {
    const reason = this.reason().trim();

    if (!reason) {
      this.escalateError.set(this.escalationStrings.REASON_REQUIRED_ERROR);
      return;
    }

    this.isEscalating.set(true);
    this.escalateError.set(null);

    this.escalationService.reEscalate(this.complaintId, { escalationReason: reason }).subscribe({
      next: () => {
        this.reason.set('');
        this.isEscalating.set(false);
        this.loadEscalation();
      },
      error: () => {
        this.isEscalating.set(false);
        this.escalateError.set(this.escalationStrings.ESCALATE_ERROR);
      },
    });
  }

  submitResolution(): void {
    const resolutionDate = this.resolutionDate();

    if (!resolutionDate) {
      this.updateError.set(this.escalationStrings.RESOLUTION_DATE_REQUIRED_ERROR);
      return;
    }

    this.isUpdating.set(true);
    this.updateError.set(null);

    this.escalationService
      .updateEscalation(this.complaintId, {
        resolvedAfterEscalation: true,
        resolutionDate,
      })
      .subscribe({
        next: (updated) => {
          this.escalation.set(updated);
          this.isUpdating.set(false);
        },
        error: () => {
          this.isUpdating.set(false);
          this.updateError.set(this.escalationStrings.UPDATE_ERROR);
        },
      });
  }

  private loadEscalation(): void {
    if (!this.complaintId) {
      return;
    }

    this.loading.set(true);
    this.loadError.set(false);

    this.escalationService.getEscalation(this.complaintId).subscribe({
      next: (escalation) => {
        this.escalation.set(escalation);
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.loading.set(false);
      },
    });
  }
}
