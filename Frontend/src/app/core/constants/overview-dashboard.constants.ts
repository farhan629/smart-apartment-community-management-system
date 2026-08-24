export const OVERVIEW_DASHBOARD_STRINGS = {
  LOADING: 'Loading your dashboard...',
  ERROR_MESSAGE: 'Could not load your dashboard. Please try again.',
} as const;

export const OVERVIEW_DASHBOARD_FETCH_LIMITS = {
  VISITOR_TREND: 100,
  RESOLUTION_COMPLAINTS: 100,
  LATEST_BOOKINGS: 5,
  TOTALS_ONLY: 1,
  COMPLAINTS_TIMELINE_PREVIEW: 4,
} as const;

export const RELATIVE_TIME_STRINGS = {
  JUST_NOW: 'Just now',
  MINUTES_AGO_SUFFIX: 'min ago',
  HOURS_AGO_SUFFIX: 'hr ago',
  YESTERDAY: 'Yesterday',
  DAYS_AGO_SUFFIX: 'days ago',
} as const;

export const OVERVIEW_WEEK_DAYS = [
  { key: 'Mon', dayIndex: 1 },
  { key: 'Tue', dayIndex: 2 },
  { key: 'Wed', dayIndex: 3 },
  { key: 'Thu', dayIndex: 4 },
  { key: 'Fri', dayIndex: 5 },
  { key: 'Sat', dayIndex: 6 },
  { key: 'Sun', dayIndex: 0 },
] as const;
