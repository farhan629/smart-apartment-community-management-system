export const STAFF_CATEGORIES = {
  ELECTRICIAN: 'Electrician',
  PLUMBER: 'Plumber',
  CLEANER: 'Cleaner',
  SECURITY: 'Security',
  GENERAL_MAINTENANCE: 'General Maintenance',
} as const;

export type StaffCategory = (typeof STAFF_CATEGORIES)[keyof typeof STAFF_CATEGORIES];

export const STAFF_CATEGORY_OPTIONS: StaffCategory[] = Object.values(STAFF_CATEGORIES);

export const STAFF_SUB_RESOURCE = {
  AVAILABILITY: 'availability',
  COMMENTS: 'comments',
} as const;

export const STAFF_QUERY_PARAM = {
  STAFF_ID: 'staffId',
} as const;

export const STAFF_LIST_DEFAULTS = {
  PAGE_NUMBER: 1,
  PAGE_SIZE: 10,
} as const;

export const STAFF_LIST_DATE_FORMAT = 'mediumDate';

export const STAFF_LIST_STRINGS = {
  PAGE_TITLE: 'Staff Management',
  NAME_COLUMN_LABEL: 'Name',
  NAME_UNAVAILABLE_LABEL: '—',
  CATEGORY_COLUMN_LABEL: 'Category',
  DESCRIPTION_COLUMN_LABEL: 'Description',
  STATUS_COLUMN_LABEL: 'Status',
  CREATED_COLUMN_LABEL: 'Onboarded On',
  AVAILABILITY_ACTION_LABEL: 'Availability',
  DETAILS_ACTION_LABEL: 'View Details',
  ACTIVE_LABEL: 'Active',
  INACTIVE_LABEL: 'Inactive',
  LOAD_ERROR: 'Could not load the staff list. Please try again.',
} as const;

export const STAFF_AVAILABILITY_DIALOG_CONFIG = {
  WIDTH: '32rem',
  MAX_WIDTH: '90vw',
} as const;

export const STAFF_DETAIL_DIALOG_CONFIG = {
  WIDTH: '28rem',
  MAX_WIDTH: '90vw',
} as const;

export const STAFF_DETAIL_STRINGS = {
  DIALOG_TITLE: 'Staff Profile',
  NAME_LABEL: 'Name',
  NAME_UNAVAILABLE_LABEL: '—',
  CATEGORY_LABEL: 'Category',
  DESCRIPTION_LABEL: 'Description',
  DETAILS_LABEL: 'Details',
  STATUS_LABEL: 'Status',
  ONBOARDED_LABEL: 'Onboarded On',
  UPDATED_LABEL: 'Last Updated',
  ACTIVE_LABEL: 'Active',
  INACTIVE_LABEL: 'Inactive',
  LOADING: 'Loading staff profile...',
  LOAD_ERROR: 'Could not load this staff profile. Please try again.',
  CLOSE_LABEL: 'Close',
  DETAILS_TAB_LABEL: 'Details',
  COMMENTS_TAB_LABEL: 'Comments',
} as const;

export const STAFF_DETAIL_TAB = {
  DETAILS: 'details',
  COMMENTS: 'comments',
} as const;

export type StaffDetailTab = (typeof STAFF_DETAIL_TAB)[keyof typeof STAFF_DETAIL_TAB];

export const STAFF_COMMENT_HISTORY_STRINGS = {
  LOADING: 'Loading comments...',
  LOAD_ERROR: 'Could not load this staff member\u2019s comments. Please try again.',
  NO_COMMENTS_MESSAGE: 'No comments have been posted about this staff member yet.',
  RATING_PREFIX: 'Rating:',
} as const;

export const STAFF_COMMENT_DATE_FORMAT = 'medium';

export const STAFF_AVAILABILITY_STRINGS = {
  DIALOG_TITLE_PREFIX: 'Availability —',
  ADD_SLOT_TITLE: 'Add a Slot',
  DATE_LABEL: 'Date',
  START_TIME_LABEL: 'Start Time',
  END_TIME_LABEL: 'End Time',
  ADD_SLOT_LABEL: 'Add Slot',
  ADDING_LABEL: 'Adding...',
  DELETE_LABEL: 'Delete',
  DELETING_LABEL: 'Deleting...',
  LOADING_SLOTS: 'Loading slots...',
  NO_SLOTS_FOUND: 'No availability slots yet.',
  LOAD_ERROR: 'Could not load availability slots. Please try again.',
  SAVE_ERROR: 'Could not add this slot. Please try again.',
  DELETE_ERROR: 'Could not delete this slot. Please try again.',
  VALIDATION_ERROR: 'Please provide a date, start time, and end time.',
  TIME_RANGE_ERROR: 'End time must be after start time.',
  BOOKED_BADGE_LABEL: 'Booked',
  CANCELLED_BADGE_LABEL: 'Cancelled',
  CANNOT_DELETE_BOOKED_HINT: 'Booked slots cannot be deleted.',
  CLOSE_LABEL: 'Close',
} as const;

export const STAFF_AVAILABILITY_FORMATS = {
  DATE: 'yyyy-MM-dd',
  TIME: 'HH:mm',
} as const;
