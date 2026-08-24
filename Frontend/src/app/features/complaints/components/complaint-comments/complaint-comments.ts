import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnChanges, OnInit, signal, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { forkJoin } from 'rxjs';

import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { EmptyState } from '../../../../shared/components/empty-state/empty-state';

import { APP_CONSTANTS, Role } from '../../../../core/constants/app.constants';
import {
  COMMENT_SECTION_STRINGS,
  COMMENT_TABS,
  COMMENT_VALIDATION,
  CommentTab,
  COMPLAINT_DATETIME_FORMAT,
} from '../../../../core/constants/complaint.constants';
import { PERMISSIONS } from '../../../../core/constants/permission.constants';
import { CommentDto, ProgressLogEntryDto } from '../../../../core/models/comment.model';
import { AuthService } from '../../../../core/services/auth-service';
import { CommentService } from '../../../../core/services/comment.service';
import { PermissionService } from '../../../../core/services/permission.service';

@Component({
  selector: 'app-complaint-comments',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, ActionButton, EmptyState],
  templateUrl: './complaint-comments.html',
  styleUrl: './complaint-comments.scss',
})
export class ComplaintComments implements OnInit, OnChanges {
  @Input({ required: true }) complaintId = '';

  private readonly commentService = inject(CommentService);
  private readonly permissionService = inject(PermissionService);
  private readonly authService = inject(AuthService);

  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  sectionStrings = COMMENT_SECTION_STRINGS;
  tabs = COMMENT_TABS;
  dateTimeFormat = COMPLAINT_DATETIME_FORMAT;
  ratingOptions: number[] = Array.from(
    { length: COMMENT_VALIDATION.RATING_MAX - COMMENT_VALIDATION.RATING_MIN + 1 },
    (_, i) => COMMENT_VALIDATION.RATING_MIN + i,
  );
  commentMaxLength = COMMENT_VALIDATION.TEXT_MAX_LENGTH;

  activeTab = signal<CommentTab>(COMMENT_TABS.COMMENTS);

  comments = signal<CommentDto[]>([]);
  progressLog = signal<ProgressLogEntryDto[]>([]);

  loading = signal(false);
  loadError = signal(false);

  commentText = signal('');
  rating = signal<number | null>(null);
  isPosting = signal(false);
  postError = signal<string | null>(null);

  private readonly currentRole =
    (this.authService.getUserRole() as Role) ?? APP_CONSTANTS.ROLES.RESIDENT;

  readonly canComment =
    this.permissionService.hasPermission(PERMISSIONS.COMPLAINT_COMMENT) &&
    this.currentRole !== APP_CONSTANTS.ROLES.ADMIN;

  ngOnInit(): void {
    this.loadThread();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['complaintId'] && !changes['complaintId'].firstChange) {
      this.loadThread();
    }
  }

  setTab(tab: CommentTab): void {
    this.activeTab.set(tab);
  }

  isAuthorSelf(commentedBy: string): boolean {
    const currentUserId = this.permissionService.userId();
    return !!currentUserId && commentedBy === currentUserId;
  }

  authorLabel(commentedBy: string): string {
    return this.isAuthorSelf(commentedBy)
      ? this.sectionStrings.YOU_LABEL
      : this.sectionStrings.SUPPORT_TEAM_LABEL;
  }

  selectRating(value: number): void {
    this.rating.set(this.rating() === value ? null : value);
  }

  submitComment(): void {
    const text = this.commentText().trim();

    if (!text) {
      this.postError.set(this.sectionStrings.COMMENT_REQUIRED_ERROR);
      return;
    }

    this.isPosting.set(true);
    this.postError.set(null);

    this.commentService
      .addComment(this.complaintId, {
        commentText: text,
        staffRating: this.rating(),
      })
      .subscribe({
        next: (comment) => {
          this.comments.set([...this.comments(), comment]);
          this.commentText.set('');
          this.rating.set(null);
          this.isPosting.set(false);
        },
        error: () => {
          this.isPosting.set(false);
          this.postError.set(this.sectionStrings.POST_COMMENT_ERROR);
        },
      });
  }

  private loadThread(): void {
    if (!this.complaintId) {
      return;
    }

    this.loading.set(true);
    this.loadError.set(false);

    forkJoin({
      comments: this.commentService.getComments(this.complaintId),
      progressLog: this.commentService.getProgressLog(this.complaintId),
    }).subscribe({
      next: ({ comments, progressLog }) => {
        this.comments.set(comments);
        this.progressLog.set(progressLog);
        this.loading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.loading.set(false);
      },
    });
  }
}
