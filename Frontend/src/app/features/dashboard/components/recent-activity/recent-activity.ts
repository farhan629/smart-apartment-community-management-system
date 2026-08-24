import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { STAFF_DASHBOARD_STRINGS } from '../../../../core/constants/staff-dashboard.constants';
import { CommentDto } from '../../../../core/models/comment.model';

@Component({
  selector: 'app-recent-activity',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './recent-activity.html',
  styleUrl: './recent-activity.scss',
})
export class RecentActivity {
  @Input({ required: true }) comments: CommentDto[] = [];

  readonly strings = STAFF_DASHBOARD_STRINGS;
}
