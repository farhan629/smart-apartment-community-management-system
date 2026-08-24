import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { ComplaintService } from './complaint.service';
import { APP_CONSTANTS } from '../constants/app.constants';
import { ComplaintSummaryDto } from '../models/complaint.model';
import { ComplaintRow, DashboardData, StatCard } from '../models/dashboard.model';

const RECENT_LIMIT = 100;
const TABLE_ROW_LIMIT = 5;

type DashboardView = (typeof APP_CONSTANTS.DASHBOARD_VIEW)[keyof typeof APP_CONSTANTS.DASHBOARD_VIEW];

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly complaintService = inject(ComplaintService);

  getAdminDashboard(): Observable<DashboardData> {
    return this.complaintService
      .getComplaints({ page: 1, limit: RECENT_LIMIT })
      .pipe(map((result) => this.buildDashboardData(result.items, APP_CONSTANTS.DASHBOARD_VIEW.ADMIN)));
  }

  getStaffDashboard(): Observable<DashboardData> {
    return this.complaintService
      .getComplaints({ page: 1, limit: RECENT_LIMIT })
      .pipe(map((result) => this.buildDashboardData(result.items, APP_CONSTANTS.DASHBOARD_VIEW.STAFF)));
  }

  getResidentDashboard(): Observable<DashboardData> {
    return this.complaintService
      .getComplaints({ page: 1, limit: RECENT_LIMIT })
      .pipe(map((result) => this.buildDashboardData(result.items, APP_CONSTANTS.DASHBOARD_VIEW.RESIDENT)));
  }

  private buildDashboardData(
    items: ComplaintSummaryDto[],
    view: DashboardView,
  ): DashboardData {
    const { COMPLAINT_STATUS } = APP_CONSTANTS;

    const openCount = items.filter((c) => c.status === COMPLAINT_STATUS.OPEN).length;
    const inProgressCount = items.filter((c) => c.status === COMPLAINT_STATUS.IN_PROGRESS).length;
    const resolvedCount = items.filter(
      (c) => c.status === COMPLAINT_STATUS.RESOLVED || c.status === COMPLAINT_STATUS.CLOSED,
    ).length;
    const escalatedCount = items.filter((c) => c.status === COMPLAINT_STATUS.ESCALATED).length;
    const pendingCount = items.length - resolvedCount;

    const cards: StatCard[] = this.buildCards(view, {
      total: items.length,
      openCount,
      inProgressCount,
      resolvedCount,
      escalatedCount,
    });

    const trendMap = new Map<string, number>();
    for (const item of items) {
      trendMap.set(item.category, (trendMap.get(item.category) ?? 0) + 1);
    }
    const trend = Array.from(trendMap.entries()).map(([category, value]) => ({ category, value }));

    const complaints: ComplaintRow[] = items.slice(0, TABLE_ROW_LIMIT).map((c) => ({
      complaintId: c.complaintId,
      category: c.category,
      description: c.description,
      status: c.status,
      priority: c.priority,
      createdAt: c.createdAt,
    }));

    return { cards, trend, resolvedCount, pendingCount, complaints };
  }

  private buildCards(
    view: DashboardView,
    counts: {
      total: number;
      openCount: number;
      inProgressCount: number;
      resolvedCount: number;
      escalatedCount: number;
    },
  ): StatCard[] {
    const { ICONS, STRINGS, STATUS, ACCENT, DASHBOARD_VIEW } = APP_CONSTANTS;

    if (view === DASHBOARD_VIEW.ADMIN) {
      return [
        {
          label: STRINGS.TOTAL_COMPLAINTS,
          value: String(counts.total),
          icon: ICONS.COMPLAINTS,
          accent: ACCENT.PRIMARY,
        },
        {
          label: STATUS.OPEN,
          value: String(counts.openCount),
          icon: ICONS.WARNING,
          accent: ACCENT.DANGER,
        },
        {
          label: STATUS.IN_PROGRESS,
          value: String(counts.inProgressCount),
          icon: ICONS.MAINTENANCE,
          accent: ACCENT.INFO,
        },
        {
          label: STATUS.RESOLVED,
          value: String(counts.resolvedCount),
          icon: ICONS.CHECK,
          accent: ACCENT.SUCCESS,
        },
      ];
    }

    if (view === DASHBOARD_VIEW.STAFF) {
      return [
        {
          label: STRINGS.ASSIGNED_TO_TEAM,
          value: String(counts.total),
          icon: ICONS.COMPLAINTS,
          accent: ACCENT.PRIMARY,
        },
        {
          label: STATUS.IN_PROGRESS,
          value: String(counts.inProgressCount),
          icon: ICONS.MAINTENANCE,
          accent: ACCENT.INFO,
        },
        {
          label: STATUS.RESOLVED,
          value: String(counts.resolvedCount),
          icon: ICONS.CHECK,
          accent: ACCENT.SUCCESS,
        },
        {
          label: STRINGS.ESCALATED,
          value: String(counts.escalatedCount),
          icon: ICONS.ESCALATION,
          accent: ACCENT.DANGER,
        },
      ];
    }

    return [
      {
        label: STRINGS.MY_COMPLAINTS_TITLE,
        value: String(counts.total),
        icon: ICONS.COMPLAINTS,
        accent: ACCENT.PRIMARY,
      },
      {
        label: STATUS.OPEN,
        value: String(counts.openCount),
        icon: ICONS.WARNING,
        accent: ACCENT.DANGER,
      },
      {
        label: STATUS.IN_PROGRESS,
        value: String(counts.inProgressCount),
        icon: ICONS.MAINTENANCE,
        accent: ACCENT.INFO,
      },
      {
        label: STATUS.RESOLVED,
        value: String(counts.resolvedCount),
        icon: ICONS.CHECK,
        accent: ACCENT.SUCCESS,
      },
    ];
  }
}
