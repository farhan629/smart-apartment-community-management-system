import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, computed, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

import { APP_CONSTANTS } from '../../../core/constants/app.constants';
import {
  ASSIGNMENT_STATUS,
  COMPLAINT_LIST_DEFAULTS,
} from '../../../core/constants/complaint.constants';
import {
  OVERVIEW_DASHBOARD_FETCH_LIMITS,
  OVERVIEW_DASHBOARD_STRINGS,
  OVERVIEW_WEEK_DAYS,
  RELATIVE_TIME_STRINGS,
} from '../../../core/constants/overview-dashboard.constants';
import {
  DASHBOARD_ASSIGNMENTS_FETCH_LIMIT,
  DASHBOARD_ASSIGNMENTS_PREVIEW_LIMIT,
  RECENT_ACTIVITY_LIMIT,
  STAFF_DASHBOARD_ROLES,
  STAFF_DASHBOARD_STRINGS,
  STAFF_STAT_CARD_DEFINITIONS,
} from '../../../core/constants/staff-dashboard.constants';
import { VISIT_STATUS } from '../../../core/constants/visit.constants';
import { AssignmentResponseDto } from '../../../core/models/assignment.model';
import { FlatItemDto } from '../../../core/models/auth.models';
import { BookingSummaryDto } from '../../../core/models/booking.model';
import { CommentDto } from '../../../core/models/comment.model';
import { ComplaintSummaryDto } from '../../../core/models/complaint.model';
import { UserLookupDto } from '../../../core/models/user-lookup.model';
import { Visit } from '../../../core/models/visit.model';
import { AssignmentService } from '../../../core/services/assignment.service';
import { AuthService } from '../../../core/services/auth-service';
import { BookingService } from '../../../core/services/booking.service';
import { CommentService } from '../../../core/services/comment.service';
import { ComplaintService } from '../../../core/services/complaint.service';
import { FlatService } from '../../../core/services/flat-service';
import { PermissionService } from '../../../core/services/permission.service';
import { RoleService } from '../../../core/services/role-service';
import { UserService } from '../../../core/services/user.service';
import { VisitService } from '../../../core/services/visit.service';
import { StatusBadge } from '../../../shared/components/status-badge/status-badge';

import { AssignedComplaintsList } from '../components/assigned-complaints-list/assigned-complaints-list';
import { RecentActivity } from '../components/recent-activity/recent-activity';
import { StaffStatCard, StaffStatCards } from '../components/staff-stat-cards/staff-stat-cards';
import { BookingHistory } from '../components/booking-history/booking-history';
import { BookingReport } from '../components/booking-report/booking-report';
import { BookingSummary } from '../components/booking-summary/booking-summary';
import {
  BookingResponseDto,
  ReportResponseDto,
  ReportSummaryDto,
} from '../../../core/services/aminety-service';

type AccentKey = 'success' | 'info' | 'danger' | 'primary';

interface StatCard {
  label: string;
  value: string;
  icon: string;
  accent: AccentKey;
}

interface BookingRow {
  amenity: string;
  icon: string;
  resident: string;
  unit: string;
  timeSlot: string;
  status: string;
}

interface TrendPoint {
  day: string;
  value: number;
}

interface ComplaintTimelineItem {
  complaintId: string;
  title: string;
  category: string;
  status: string;
  updatedLabel: string;
}

interface OverviewFetchResult {
  residentsTotal: number | null;
  activeVisitors: number;
  openComplaints: number;
  todaysBookings: number;
  trendVisits: Visit[];
  complaintsForResolution: ComplaintSummaryDto[];
  latestBookingsRaw: BookingSummaryDto[];
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    RouterLink,
    StatusBadge,
    StaffStatCards,
    AssignedComplaintsList,
    RecentActivity,
    BookingHistory,
    BookingReport,
    BookingSummary,
  ],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPageComponent implements OnInit {
  strings = APP_CONSTANTS.STRINGS;
  icons = APP_CONSTANTS.ICONS;
  routes = APP_CONSTANTS.ROUTES;

  staffStrings = STAFF_DASHBOARD_STRINGS;
  staffLoading = signal(true);
  staffError = signal(false);
  staffAssignmentsPreview = signal<AssignmentResponseDto[]>([]);
  staffComments = signal<CommentDto[]>([]);
  staffStatCards = signal<StaffStatCard[]>([]);

  overviewStrings = OVERVIEW_DASHBOARD_STRINGS;
  overviewLoading = signal(true);
  overviewError = signal(false);
  statCards = signal<StatCard[]>([]);
  visitorTrend = signal<TrendPoint[]>([]);
  resolvedCount = signal(0);
  pendingCount = signal(0);
  latestBookings = signal<BookingRow[]>([]);
  residentComplaintsTimeline = signal<ComplaintTimelineItem[]>([]);
  hoveredDonutSegment = signal<'resolved' | 'pending' | null>(null);
  hoveredTrendPoint = signal<TrendPoint | null>(null);
  private readonly displayUserName = signal('');

  // Admin booking properties from set-2
  bookingLoading = signal(false);
  bookingError = signal(false);
  adminBookings: BookingResponseDto[] = [];
  adminSummary: ReportSummaryDto | null = null;

  // Static stats from set-2 (used as fallback)
  private defaultStatCards: StatCard[] = [
    {
      label: this.strings.TOTAL_RESIDENTS_LABEL,
      value: '1,284',
      icon: this.icons.RESIDENTS,
      accent: 'success',
    },
    {
      label: this.strings.ACTIVE_VISITORS_LABEL,
      value: '42',
      icon: this.icons.VISITORS,
      accent: 'info',
    },
    {
      label: this.strings.OPEN_COMPLAINTS_LABEL,
      value: '18',
      icon: this.icons.COMPLAINTS,
      accent: 'danger',
    },
    {
      label: this.strings.TODAYS_BOOKINGS_LABEL,
      value: '24',
      icon: this.icons.BOOKINGS,
      accent: 'primary',
    },
  ];

  maxTrendValue = computed(() => Math.max(1, ...this.visitorTrend().map((point) => point.value)));

  resolvedPercent = computed(() => {
    const total = this.resolvedCount() + this.pendingCount();
    return total === 0 ? 0 : Math.round((this.resolvedCount() / total) * 100);
  });

  constructor(
    private readonly authService: AuthService,
    private readonly assignmentService: AssignmentService,
    private readonly commentService: CommentService,
    private readonly permissionService: PermissionService,
    private readonly complaintService: ComplaintService,
    private readonly visitService: VisitService,
    private readonly bookingService: BookingService,
    private readonly userService: UserService,
    private readonly roleService: RoleService,
    private readonly flatService: FlatService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  get userName(): string {
    return this.displayUserName() || this.permissionService.userName();
  }

  get isStaffRole(): boolean {
    const role = this.authService.getUserRole();
    return !!role && (STAFF_DASHBOARD_ROLES as string[]).includes(role);
  }

  get isAdminRole(): boolean {
    return this.authService.getUserRole() === APP_CONSTANTS.ROLES.ADMIN;
  }

  ngOnInit(): void {
    this.loadDisplayUserName();
    
    // Check if permission service is loaded before determining view
    if (this.permissionService.loaded()) {
      this.determineView();
    }
  }

  private determineView(): void {
    if (this.isStaffRole) {
      this.loadStaffDashboard();
    } else if (this.isAdminRole) {
      this.loadAdminBookings();
      this.loadOverviewDashboard(); // Load overview for admin as well
    } else {
      this.loadOverviewDashboard();
    }
  }

  private loadDisplayUserName(): void {
    const userId = this.authService.getUserId();
    if (!userId) {
      return;
    }
    this.userService
      .getUserById(userId)
      .pipe(catchError(() => of(null)))
      .subscribe((user) => {
        if (user?.userName) {
          this.displayUserName.set(user.userName);
        }
      });
  }

  statusColorKey(status: string): string {
    return APP_CONSTANTS.STATUS_COLOR_KEY[status] ?? 'neutral';
  }

  getDonutDashArray(circumference: number): string {
    const resolvedLength = (this.resolvedPercent() / 100) * circumference;
    return `${resolvedLength} ${circumference}`;
  }

  getPendingDashArray(circumference: number): string {
    const pendingLength = ((100 - this.resolvedPercent()) / 100) * circumference;
    return `${pendingLength} ${circumference}`;
  }

  getPendingDashOffset(circumference: number): number {
    const resolvedLength = (this.resolvedPercent() / 100) * circumference;
    return -resolvedLength;
  }

  private loadStaffDashboard(): void {
    this.staffLoading.set(true);
    this.staffError.set(false);

    const staffId = this.authService.getUserId();

    forkJoin({
      assignments: this.assignmentService
        .getMyAssignments(COMPLAINT_LIST_DEFAULTS.PAGE_NUMBER, DASHBOARD_ASSIGNMENTS_FETCH_LIMIT)
        .pipe(catchError(() => of(null))),
      comments: staffId
        ? this.commentService
            .getStaffComments(staffId)
            .pipe(catchError(() => of([] as CommentDto[])))
        : of([] as CommentDto[]),
    }).subscribe({
      next: ({ assignments, comments }) => {
        this.staffLoading.set(false);

        if (!assignments) {
          this.staffError.set(true);
          return;
        }

        this.staffAssignmentsPreview.set(this.buildStaffPreview(assignments.items));
        this.staffStatCards.set(this.buildStaffStatCards(assignments.items));

        const sortedComments = [...comments].sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
        );
        this.staffComments.set(sortedComments.slice(0, RECENT_ACTIVITY_LIMIT));
        this.cdr.detectChanges();
      },
      error: () => {
        this.staffLoading.set(false);
        this.staffError.set(true);
        this.cdr.detectChanges();
      },
    });
  }

  private buildStaffPreview(assignments: AssignmentResponseDto[]): AssignmentResponseDto[] {
    const activeStatuses: string[] = [ASSIGNMENT_STATUS.PENDING, ASSIGNMENT_STATUS.ACCEPTED];
    return [...assignments]
      .filter((assignment) => activeStatuses.includes(assignment.status))
      .sort((a, b) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime())
      .slice(0, DASHBOARD_ASSIGNMENTS_PREVIEW_LIMIT);
  }

  private buildStaffStatCards(assignments: AssignmentResponseDto[]): StaffStatCard[] {
    return STAFF_STAT_CARD_DEFINITIONS.map((definition) => ({
      key: definition.key,
      label: definition.label,
      icon: definition.icon,
      accent: definition.accent,
      value: assignments.filter((assignment) =>
        (definition.statuses as readonly string[]).includes(assignment.status),
      ).length,
    }));
  }

  private loadAdminBookings(): void {
    this.bookingLoading.set(true);
    this.bookingError.set(false);

    this.bookingService.getBookingReport({ pageNumber: 1, pageSize: 5 })
      .pipe(catchError((e) => { 
        console.error('Admin booking err', e); 
        return of(null); 
      }))
      .subscribe((res) => {
        this.bookingLoading.set(false);
        if (res) {
          this.adminBookings = res.bookings ?? [];
          this.adminSummary = res.summary ?? null;
        } else {
          this.bookingError.set(true);
        }
        this.cdr.detectChanges();
      });
  }

  private loadOverviewDashboard(): void {
    this.overviewLoading.set(true);
    this.overviewError.set(false);

    const isAdmin = this.isAdminRole;
    const userId = this.authService.getUserId() ?? undefined;
    const today = this.formatDate(new Date());
    const { start, end } = this.getWeekRange();
    const hostUserId = isAdmin ? undefined : userId;

    forkJoin({
      residentsTotal: isAdmin ? this.loadResidentsTotal() : of(null),
      activeVisitors: this.visitService
        .getVisits({
          status: VISIT_STATUS.CHECKED_IN,
          hostUserId,
          limit: OVERVIEW_DASHBOARD_FETCH_LIMITS.TOTALS_ONLY,
        })
        .pipe(
          map((res) => res.pagination.totalCount),
          catchError(() => of(0)),
        ),
      openComplaints: this.complaintService
        .getComplaints({
          status: APP_CONSTANTS.COMPLAINT_STATUS.OPEN,
          page: 1,
          limit: OVERVIEW_DASHBOARD_FETCH_LIMITS.TOTALS_ONLY,
        })
        .pipe(
          map((res) => res.totalCount),
          catchError(() => of(0)),
        ),
      todaysBookings: (isAdmin
        ? this.bookingService
            .getBookingReport({ fromDate: today, toDate: today })
            .pipe(map((res) => res.summary?.totalBookings ?? 0))
        : this.bookingService
            .getMyBookings({ fromDate: today, toDate: today })
            .pipe(map((res) => res.pagination?.totalCount ?? 0))
      ).pipe(catchError(() => of(0))),
      trendVisits: this.visitService
        .getVisits({
          startDate: start,
          endDate: end,
          hostUserId,
          limit: OVERVIEW_DASHBOARD_FETCH_LIMITS.VISITOR_TREND,
        })
        .pipe(
          map((res) => res.items),
          catchError(() => of([] as Visit[])),
        ),
      complaintsForResolution: this.complaintService
        .getComplaints({ page: 1, limit: OVERVIEW_DASHBOARD_FETCH_LIMITS.RESOLUTION_COMPLAINTS })
        .pipe(
          map((res) => res.items),
          catchError(() => of([] as ComplaintSummaryDto[])),
        ),
      latestBookingsRaw: (isAdmin
        ? this.bookingService
            .getBookingReport({
              pageNumber: 1,
              pageSize: OVERVIEW_DASHBOARD_FETCH_LIMITS.LATEST_BOOKINGS,
            })
            .pipe(map((res) => res.bookings ?? []))
        : this.bookingService
            .getMyBookings({
              pageNumber: 1,
              pageSize: OVERVIEW_DASHBOARD_FETCH_LIMITS.LATEST_BOOKINGS,
            })
            .pipe(map((res) => res.data ?? []))
      ).pipe(catchError(() => of([] as BookingSummaryDto[]))),
    }).subscribe({
      next: (result: OverviewFetchResult) => {
        // Use dynamic stats if available, otherwise fallback to default
        const dynamicStats = this.buildOverviewStatCards(isAdmin, result);
        this.statCards.set(dynamicStats.length > 0 ? dynamicStats : this.defaultStatCards);
        this.visitorTrend.set(this.buildVisitorTrend(result.trendVisits));
        this.applyResolutionCounts(result.complaintsForResolution);
        this.resolveLatestBookings(isAdmin, result.latestBookingsRaw);
        if (!isAdmin) {
          this.residentComplaintsTimeline.set(
            this.buildComplaintsTimeline(result.complaintsForResolution),
          );
        }
        this.overviewLoading.set(false);
        this.cdr.detectChanges();
      },
      error: () => {
        this.overviewLoading.set(false);
        this.overviewError.set(true);
        // Use default stats on error
        this.statCards.set(this.defaultStatCards);
        this.cdr.detectChanges();
      },
    });
  }

  private loadResidentsTotal(): Observable<number> {
    return this.roleService.getOccupantRoles().pipe(
      switchMap((roles) => {
        const residentRole = roles.find((role) => role.termValue === APP_CONSTANTS.ROLES.RESIDENT);
        return residentRole ? this.userService.getUsersCountByRole(residentRole.id) : of(0);
      }),
      catchError(() => of(0)),
    );
  }

  private buildOverviewStatCards(isAdmin: boolean, result: OverviewFetchResult): StatCard[] {
    const { ICONS, STRINGS } = APP_CONSTANTS;
    const cards: StatCard[] = [];

    if (isAdmin) {
      cards.push({
        label: STRINGS.TOTAL_RESIDENTS_LABEL,
        value: String(result.residentsTotal ?? 0),
        icon: ICONS.RESIDENTS,
        accent: 'success',
      });
    }

    cards.push(
      {
        label: STRINGS.ACTIVE_VISITORS_LABEL,
        value: String(result.activeVisitors),
        icon: ICONS.VISITORS,
        accent: 'info',
      },
      {
        label: STRINGS.OPEN_COMPLAINTS_LABEL,
        value: String(result.openComplaints),
        icon: ICONS.COMPLAINTS,
        accent: 'danger',
      },
      {
        label: STRINGS.TODAYS_BOOKINGS_LABEL,
        value: String(result.todaysBookings),
        icon: ICONS.BOOKINGS,
        accent: 'primary',
      },
    );

    return cards;
  }

  private buildVisitorTrend(visits: Visit[]): TrendPoint[] {
    const counts = new Map<number, number>();
    for (const visit of visits) {
      const dayIndex = new Date(visit.startDate).getDay();
      counts.set(dayIndex, (counts.get(dayIndex) ?? 0) + 1);
    }
    return OVERVIEW_WEEK_DAYS.map((entry) => ({
      day: entry.key,
      value: counts.get(entry.dayIndex) ?? 0,
    }));
  }

  private applyResolutionCounts(items: ComplaintSummaryDto[]): void {
    const { COMPLAINT_STATUS } = APP_CONSTANTS;
    const resolved = items.filter(
      (item) =>
        item.status === COMPLAINT_STATUS.RESOLVED || item.status === COMPLAINT_STATUS.CLOSED,
    ).length;
    this.resolvedCount.set(resolved);
    this.pendingCount.set(items.length - resolved);
  }

  private buildComplaintsTimeline(items: ComplaintSummaryDto[]): ComplaintTimelineItem[] {
    return [...items]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, OVERVIEW_DASHBOARD_FETCH_LIMITS.COMPLAINTS_TIMELINE_PREVIEW)
      .map((item) => ({
        complaintId: item.complaintId,
        title: item.description,
        category: item.category,
        status: item.status,
        updatedLabel: this.formatRelativeTime(item.createdAt),
      }));
  }

  private formatRelativeTime(dateStr: string): string {
    const diffMs = Date.now() - new Date(dateStr).getTime();
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMinutes / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMinutes < 1) {
      return RELATIVE_TIME_STRINGS.JUST_NOW;
    }
    if (diffMinutes < 60) {
      return `${diffMinutes} ${RELATIVE_TIME_STRINGS.MINUTES_AGO_SUFFIX}`;
    }
    if (diffHours < 24) {
      return `${diffHours} ${RELATIVE_TIME_STRINGS.HOURS_AGO_SUFFIX}`;
    }
    if (diffDays === 1) {
      return RELATIVE_TIME_STRINGS.YESTERDAY;
    }
    return `${diffDays} ${RELATIVE_TIME_STRINGS.DAYS_AGO_SUFFIX}`;
  }

  private resolveLatestBookings(isAdmin: boolean, bookings: BookingSummaryDto[]): void {
    if (!isAdmin) {
      this.resolveResidentLatestBookings(bookings);
      return;
    }

    const userIds = Array.from(new Set(bookings.map((booking) => booking.userId).filter(Boolean)));
    if (userIds.length === 0) {
      this.latestBookings.set([]);
      return;
    }

    forkJoin(userIds.map((id) => this.userService.getUserById(id).pipe(catchError(() => of(null)))))
      .pipe(
        switchMap((users) => {
          const flatIds = Array.from(
            new Set(
              users
                .filter((user): user is UserLookupDto => !!user && !!user.flatId)
                .map((user) => user.flatId as string),
            ),
          );

          if (flatIds.length === 0) {
            return of({ users, flats: [] as (FlatItemDto | null)[] });
          }

          return forkJoin(
            flatIds.map((id) => this.flatService.getFlatById(id).pipe(catchError(() => of(null)))),
          ).pipe(map((flats) => ({ users, flats })));
        }),
      )
      .subscribe(({ users, flats }) => {
        const userMap = new Map(userIds.map((id, index) => [id, users[index]]));
        const flatMap = new Map(
          flats.filter((flat): flat is FlatItemDto => !!flat).map((flat) => [flat.id, flat]),
        );

        const rows = bookings.map((booking) => {
          const user = userMap.get(booking.userId);
          const flat = user?.flatId ? flatMap.get(user.flatId) : undefined;
          const residentName = user?.userName || '-';
          const unit = flat ? `${flat.block}-${flat.number}` : '-';
          return this.toBookingRow(booking, residentName, unit);
        });

        this.latestBookings.set(rows);
      });
  }

  private resolveResidentLatestBookings(bookings: BookingSummaryDto[]): void {
    const userId = this.permissionService.userId() || this.authService.getUserId();

    if (!userId) {
      this.latestBookings.set(bookings.map((booking) => this.toBookingRow(booking, '-', '-')));
      return;
    }

    this.userService
      .getUserById(userId)
      .pipe(
        switchMap((user) => {
          const resolvedResidentName = user?.userName || '-';
          return user?.flatId
            ? this.flatService.getFlatById(user.flatId).pipe(
                map((flat) => ({
                  residentName: resolvedResidentName,
                  unit: `${flat.block}-${flat.number}`,
                })),
                catchError(() => of({ residentName: resolvedResidentName, unit: '-' })),
              )
            : of({ residentName: resolvedResidentName, unit: '-' });
        }),
        catchError(() => of({ residentName: '-', unit: '-' })),
      )
      .subscribe(({ residentName, unit }) => {
        this.latestBookings.set(
          bookings.map((booking) => this.toBookingRow(booking, residentName, unit)),
        );
      });
  }

  private toBookingRow(booking: BookingSummaryDto, resident: string, unit: string): BookingRow {
    return {
      amenity: booking.amenityName ?? '-',
      icon: APP_CONSTANTS.ICONS.AMENITIES,
      resident,
      unit,
      timeSlot: `${this.formatTime(booking.startTime)} - ${this.formatTime(booking.endTime)}`,
      status: booking.status ?? '-',
    };
  }

  private formatTime(time: string): string {
    return time && time.length > 5 ? time.slice(0, 5) : time;
  }

  private formatDate(date: Date): string {
    return date.toISOString().slice(0, 10);
  }

  private getWeekRange(): { start: string; end: string } {
    const now = new Date();
    const day = now.getDay();
    const diffToMonday = day === 0 ? -6 : 1 - day;
    const monday = new Date(now);
    monday.setDate(now.getDate() + diffToMonday);
    const sunday = new Date(monday);
    sunday.setDate(monday.getDate() + 6);
    return { start: this.formatDate(monday), end: this.formatDate(sunday) };
  }
}