import { APP_CONSTANTS } from './app.constants';
import { ASSIGNMENT_STATUS } from './complaint.constants';

const { ROLES } = APP_CONSTANTS;

export const STAFF_DASHBOARD_ROLES: string[] = [ROLES.STAFF, ROLES.ELECTRICIAN, ROLES.SECURITY];

export const STAFF_DASHBOARD_STRINGS = {
  WELCOME_BACK: 'Welcome back',
  SUBTITLE: "Here's what's on your plate today.",
  STATS_TITLE: 'My Assignments Overview',
  ASSIGNED_LIST_TITLE: 'Assigned Complaints',
  ASSIGNED_LIST_EMPTY: "You don't have any assigned complaints right now.",
  COMPLAINT_COLUMN: 'Complaint',
  STATUS_COLUMN: 'Status',
  ASSIGNED_ON_COLUMN: 'Assigned On',
  DUE_DATE_COLUMN: 'Due Date',
  RECENT_ACTIVITY_TITLE: 'Recent Activity',
  RECENT_ACTIVITY_EMPTY: 'No recent activity to show.',
  VIEW_COMPLAINT: 'View',
  LOADING: 'Loading your dashboard...',
  ERROR_MESSAGE: 'Could not load your dashboard. Please try again.',
} as const;

export const STAFF_STAT_CARD_DEFINITIONS = [
  {
    key: 'pending',
    label: 'Pending',
    icon: 'hourglass_top',
    accent: 'warning' as const,
    statuses: [ASSIGNMENT_STATUS.PENDING],
  },
  {
    key: 'accepted',
    label: 'In Progress',
    icon: 'engineering',
    accent: 'info' as const,
    statuses: [ASSIGNMENT_STATUS.ACCEPTED],
  },
  {
    key: 'completed',
    label: 'Completed',
    icon: 'check_circle',
    accent: 'success' as const,
    statuses: [ASSIGNMENT_STATUS.COMPLETED],
  },
  {
    key: 'escalated',
    label: 'Escalated',
    icon: 'report_problem',
    accent: 'danger' as const,
    statuses: [ASSIGNMENT_STATUS.ESCALATED],
  },
] as const;

export const RECENT_ACTIVITY_LIMIT = 5;
export const DASHBOARD_ASSIGNMENTS_FETCH_LIMIT = 100;
export const DASHBOARD_ASSIGNMENTS_PREVIEW_LIMIT = 5;
