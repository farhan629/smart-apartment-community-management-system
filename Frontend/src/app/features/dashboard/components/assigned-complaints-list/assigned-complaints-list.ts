import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { STAFF_DASHBOARD_STRINGS } from '../../../../core/constants/staff-dashboard.constants';
import { AssignmentResponseDto } from '../../../../core/models/assignment.model';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';

@Component({
  selector: 'app-assigned-complaints-list',
  standalone: true,
  imports: [CommonModule, StatusBadge],
  templateUrl: './assigned-complaints-list.html',
  styleUrl: './assigned-complaints-list.scss',
})
export class AssignedComplaintsList {
  @Input({ required: true }) assignments: AssignmentResponseDto[] = [];

  readonly strings = STAFF_DASHBOARD_STRINGS;

  constructor(private readonly router: Router) {}

  viewComplaint(complaintId: string): void {
    this.router.navigate([APP_CONSTANTS.ROUTES.COMPLAINTS, complaintId]);
  }
}
