export const VISIT_STATUS = {
  PENDING: 'PENDING',
  APPROVED: 'APPROVED',
  REJECTED: 'REJECTED',
  CHECKED_IN: 'CHECKED_IN',
  CHECKED_OUT: 'CHECKED_OUT',
  EXPIRED: 'EXPIRED',
  CANCELLED: 'CANCELLED',
} as const;

export type VisitStatus = (typeof VISIT_STATUS)[keyof typeof VISIT_STATUS];

export const VISIT_STATUS_LABELS: Record<VisitStatus, string> = {
  [VISIT_STATUS.PENDING]: 'Pending',
  [VISIT_STATUS.APPROVED]: 'Approved',
  [VISIT_STATUS.REJECTED]: 'Rejected',
  [VISIT_STATUS.CHECKED_IN]: 'Checked In',
  [VISIT_STATUS.CHECKED_OUT]: 'Checked Out',
  [VISIT_STATUS.EXPIRED]: 'Expired',
  [VISIT_STATUS.CANCELLED]: 'Cancelled',
};

export const EDITABLE_VISIT_STATUSES: VisitStatus[] = [VISIT_STATUS.PENDING];

export const CANCELLABLE_VISIT_STATUSES: VisitStatus[] = [
  VISIT_STATUS.APPROVED,
];

export const UPCOMING_VISIT_STATUSES: VisitStatus[] = [
  VISIT_STATUS.PENDING,
  VISIT_STATUS.APPROVED,
];

export const HISTORY_VISIT_STATUSES: VisitStatus[] = [
  VISIT_STATUS.CHECKED_IN,
  VISIT_STATUS.CHECKED_OUT,
  VISIT_STATUS.REJECTED,
  VISIT_STATUS.EXPIRED,
  VISIT_STATUS.CANCELLED,
];

export const VISIT_TABS = {
  UPCOMING: 'upcoming',
  HISTORY: 'history',
} as const;

export type VisitTab = (typeof VISIT_TABS)[keyof typeof VISIT_TABS];

export const VISIT_PAGINATION_DEFAULTS = {
  PAGE: 1,
  LIMIT: 10,
  SORT_BY: 'startDate',
  SORT_ORDER: 'desc',
};
export const APPROVABLE_VISIT_STATUSES: VisitStatus[] = [VISIT_STATUS.PENDING];
export const SCAN_DIRECTION = {
  CHECK_IN: 'check-in',
  CHECK_OUT: 'check-out',
} as const;


export type ScanDirection = (typeof SCAN_DIRECTION)[keyof typeof SCAN_DIRECTION];