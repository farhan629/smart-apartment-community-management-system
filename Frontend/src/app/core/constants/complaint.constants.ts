export const COMPLAINT_LIST_DEFAULTS = {
  PAGE_NUMBER: 1,
  PAGE_SIZE: 10,
} as const;

export const COMPLAINT_PAGE_SIZE_OPTIONS: number[] = [10, 25, 50];

export const VIEW_MODE = {
  MINE: 'mine',
  ALL: 'all',
} as const;

export type ViewMode = (typeof VIEW_MODE)[keyof typeof VIEW_MODE];

export const COMPLAINT_DIALOG_CONFIG = {
  DETAIL_PANEL_CLASS: 'slide-panel-dialog',
  DETAIL_BACKDROP_CLASS: 'slide-panel-dialog-backdrop',
  DETAIL_WIDTH: '28rem',
  DETAIL_HEIGHT: '100vh',
  CREATE_WIDTH: '90%',
  CREATE_MAX_WIDTH: '90%',
  CREATE_PANEL_CLASS: 'create-complaint-dialog',
} as const;
export interface RefDataOption {
  id: string;
  label: string;
}

export const COMPLAINT_TYPE_OPTIONS: RefDataOption[] = [
  { id: 'a0000000-0000-0000-0000-000000000001', label: 'Service Request' },
  { id: 'a0000000-0000-0000-0000-000000000002', label: 'Complaint' },
];

export const COMPLAINT_PRIORITY_OPTIONS: RefDataOption[] = [
  { id: 'b0000000-0000-0000-0000-000000000001', label: 'Urgent' },
  { id: 'b0000000-0000-0000-0000-000000000002', label: 'General' },
  { id: 'b0000000-0000-0000-0000-000000000003', label: 'Moderate' },
];

export const COMPLAINT_CATEGORY_OPTIONS: RefDataOption[] = [
  { id: 'e0000000-0000-0000-0000-000000000001', label: 'AC' },
  { id: 'e0000000-0000-0000-0000-000000000002', label: 'Plumbing' },
  { id: 'e0000000-0000-0000-0000-000000000003', label: 'Electrical' },
  { id: 'e0000000-0000-0000-0000-000000000004', label: 'Carpentry' },
  { id: 'e0000000-0000-0000-0000-000000000005', label: 'Cleaning' },
  { id: 'e0000000-0000-0000-0000-000000000006', label: 'Pest Control' },
];

export const COMPLAINT_STATUS = {
  OPEN: 'Open',
  ASSIGNED: 'Assigned',
  IN_PROGRESS: 'InProgress',
  RESOLVED: 'Resolved',
  CLOSED: 'Closed',
  CANCELLED: 'Cancelled',
} as const;

export type ComplaintStatus = (typeof COMPLAINT_STATUS)[keyof typeof COMPLAINT_STATUS];

export const ASSIGNMENT_STATUS = {
  PENDING: 'Pending',
  ACCEPTED: 'Accepted',
  DENIED: 'Denied',
  ESCALATED: 'Escalated',
  COMPLETED: 'Completed',
} as const;

export type AssignmentStatus = (typeof ASSIGNMENT_STATUS)[keyof typeof ASSIGNMENT_STATUS];

export const ASSIGNMENT_MODE = {
  ASSIGN: 'assign',
  REASSIGN: 'reassign',
} as const;

export type AssignmentMode = (typeof ASSIGNMENT_MODE)[keyof typeof ASSIGNMENT_MODE];

export const ASSIGN_STAFF_DIALOG_CONFIG = {
  WIDTH: '28rem',
  MAX_WIDTH: '90vw',
} as const;

export const COMPLAINT_ROUTE_PARAM = 'complaintId';

export const KEYBOARD_KEYS = {
  ESCAPE: 'Escape',
} as const;

export const COMPLAINT_DETAIL_STRINGS = {
  PANEL_TITLE: 'Complaint Details',
  BACK_TO_COMPLAINTS: 'Back to Complaints',
  LOAD_ERROR: 'Something went wrong loading this complaint.',
  RAISED_ON: 'Raised on',
  COMPLAINT_TYPE_LABEL: 'Complaint Type',
  PRIORITY_LABEL: 'Priority',
  PREFERRED_DATE_LABEL: 'Preferred Date',
  PREFERRED_TIME_LABEL: 'Preferred Time',
  LAST_UPDATED_LABEL: 'Last Updated',
  EMPTY_VALUE: '—',

  CANCELLED_LABEL: 'Cancelled',
  CANCELLED_ON_PREFIX: 'on',

  ASSIGNED_TO_LABEL: 'Assigned To',
  DUE_PREFIX: 'Due',
  DENIAL_REASON_PREFIX: 'Denial reason:',

  ASSIGN_STAFF_LABEL: 'Assign Staff',
  REASSIGN_STAFF_LABEL: 'Reassign Staff',
  START_PROGRESS_LABEL: 'Start Progress',
  MARK_RESOLVED_LABEL: 'Mark Resolved',
  UPDATING_LABEL: 'Updating...',
  CANCEL_COMPLAINT_LABEL: 'Cancel Complaint',

  CANCEL_REASON_FIELD_LABEL: 'Reason for cancellation',
  CANCEL_REASON_PLACEHOLDER: "Let us know why you're cancelling this complaint...",
  CANCELLING_LABEL: 'Cancelling...',
  CONFIRM_CANCELLATION_LABEL: 'Confirm Cancellation',

  STATUS_UPDATE_ERROR: 'Could not update the status. Please try again.',
  CANCEL_REASON_REQUIRED_ERROR: 'Please provide a reason for cancellation.',
  CANCEL_ERROR: 'Could not cancel this complaint. Please try again.',

  RESIDENT_NAME_LABEL: 'Name',
  RESIDENT_EMAIL_LABEL: 'Email',
  RESIDENT_BLOCK_LABEL: 'Block',
  RESIDENT_FLAT_NUMBER_LABEL: 'Flat No.',
} as const;

export const COMPLAINT_DATE_FORMAT = 'mediumDate';

export const COMPLAINT_DATETIME_FORMAT = 'medium';

export const COMPLAINT_LIST_STRINGS = {
  MY_ASSIGNMENTS_TAB: 'My Assignments',
  ALL_COMPLAINTS_TAB: 'All Complaints',
  STATUS_FILTER_PLACEHOLDER: 'All Status',
  CATEGORY_FILTER_PLACEHOLDER: 'All Categories',
  SEARCH_PLACEHOLDER: 'Search complaints...',
  COMPLAINT_COLUMN_LABEL: 'Complaint',
  ASSIGNMENT_STATUS_COLUMN_LABEL: 'Assignment Status',
  ASSIGNED_ON_COLUMN_LABEL: 'Assigned On',
  DUE_DATE_COLUMN_LABEL: 'Due Date',
  PRIORITY_COLUMN_LABEL: 'Priority',
  STATUS_COLUMN_LABEL: 'Status',
  ACCEPT_LABEL: 'Accept',
  DENY_LABEL: 'Deny',
  DENY_REASON_PROMPT: 'Reason for denying this assignment:',
} as const;

export const COMPLAINT_FILTER_KEYS = {
  STATUS: 'status',
  CATEGORY: 'category',
} as const;

export const ESCALATION_CHECK_STRINGS = {
  RUN_CHECK_LABEL: 'Run Escalation Check',
  RUNNING_LABEL: 'Running...',
  SUCCESS_MESSAGE: (count: number): string =>
    count === 1 ? '1 complaint was escalated.' : `${count} complaints were escalated.`,
  NO_ESCALATIONS_MESSAGE: 'No complaints needed escalation.',
  ERROR_MESSAGE: 'Could not run the escalation check. Please try again.',
} as const;

export const CREATE_COMPLAINT_STRINGS = {
  PAGE_TITLE: 'Raise a Complaint',

  COMPLAINT_TYPE_LABEL: 'Complaint Type',
  COMPLAINT_TYPE_PLACEHOLDER: 'Select a type',
  COMPLAINT_TYPE_REQUIRED: 'Complaint type is required.',

  CATEGORY_PLACEHOLDER: 'Select a category',
  CATEGORY_REQUIRED: 'Category is required.',

  PRIORITY_LABEL: 'Priority',
  PRIORITY_PLACEHOLDER: 'Select priority',
  PRIORITY_REQUIRED: 'Priority is required.',

  DESCRIPTION_PLACEHOLDER: 'Describe the issue in detail...',
  DESCRIPTION_REQUIRED: 'Description is required.',
  DESCRIPTION_MIN_LENGTH: 'Please provide at least 10 characters.',
  DESCRIPTION_MAX_LENGTH: 500,

  PREFERRED_DATE_LABEL: 'Preferred Date',
  PREFERRED_DATE_REQUIRED: 'Preferred date is required.',

  PREFERRED_TIME_LABEL: 'Preferred Time (optional)',
  SUBMIT_ERROR: 'Could not submit your complaint. Please try again.',
  SUBMITTING: 'Submitting...',
} as const;

export const ASSIGN_STAFF_DIALOG_STRINGS = {
  ASSIGN_TITLE: 'Assign Staff',
  REASSIGN_TITLE: 'Reassign Staff',
  SHOWING_STAFF_IN_PREFIX: 'Showing staff in',
  NO_EXACT_MATCH_NOTE: '(no exact match — showing all staff)',
  LOADING_STAFF: 'Loading staff...',
  LOAD_ERROR: 'Could not load the staff list. Please try again.',
  NO_STAFF_FOUND: 'No staff found.',
  SHOW_ALL_CATEGORIES: 'Show staff from all categories',
  DUE_DATE_LABEL: 'Due date',
  ASSIGN_ACTION_LABEL: 'Assign',
  REASSIGN_ACTION_LABEL: 'Reassign',
  SAVING_LABEL: 'Saving...',
  VALIDATION_ERROR: 'Select a staff member and a due date.',
  SAVE_ERROR: 'Could not save the assignment. Please try again.',
};

export const FORM_VALIDATORS = {
  REQUIRED: 'required',
  MIN_LENGTH: 'minlength',
} as const;

export const COMPLAINT_SUB_RESOURCE = {
  COMMENTS: 'comments',
  PROGRESS_LOG: 'progress-log',
} as const;

export const COMPLAINT_QUERY_PARAM = {
  COMPLAINT_ID: 'complaintId',
} as const;

export const ASSIGNMENT_QUERY_PARAM = {
  ASSIGNMENT_ID: 'assignmentId',
} as const;

export const COMMENT_TABS = {
  COMMENTS: 'comments',
  PROGRESS: 'progress',
} as const;

export type CommentTab = (typeof COMMENT_TABS)[keyof typeof COMMENT_TABS];

export const COMMENT_VALIDATION = {
  TEXT_MAX_LENGTH: 1000,
  RATING_MIN: 1,
  RATING_MAX: 5,
} as const;

export const COMMENT_SECTION_STRINGS = {
  SECTION_TITLE: 'Comments & Progress',
  COMMENTS_TAB_LABEL: 'Comments',
  PROGRESS_TAB_LABEL: 'Progress Log',
  ADD_COMMENT_PLACEHOLDER: 'Share an update or ask a question...',
  RATING_LABEL: 'Rate the service (optional)',
  POST_LABEL: 'Post Comment',
  POSTING_LABEL: 'Posting...',
  NO_COMMENTS_MESSAGE: 'No comments yet.',
  NO_PROGRESS_MESSAGE: 'No progress updates yet.',
  LOAD_COMMENTS_ERROR: 'Could not load comments. Please try again.',
  LOAD_PROGRESS_ERROR: 'Could not load the progress log. Please try again.',
  POST_COMMENT_ERROR: 'Could not post your comment. Please try again.',
  COMMENT_REQUIRED_ERROR: 'Please write a comment before posting.',
  YOU_LABEL: 'You',
  SUPPORT_TEAM_LABEL: 'Support Team',
  RATING_PREFIX: 'Rated',
} as const;

export const ESCALATION_VALIDATION = {
  REASON_MAX_LENGTH: 1000,
} as const;

export const ESCALATION_STRINGS = {
  SECTION_TITLE: 'Escalation',
  LOAD_ERROR: 'Could not load escalation details. Please try again.',
  NOT_ESCALATED_MESSAGE: 'This complaint has not been escalated.',
  ESCALATE_FORM_TITLE: 'Escalate this Complaint',
  REASON_PLACEHOLDER: 'Explain why this complaint needs urgent attention...',
  ESCALATE_SUBMIT_LABEL: 'Submit Escalation',
  RE_ESCALATE_SUBMIT_LABEL: 'Escalate Again',
  ESCALATING_LABEL: 'Escalating...',
  REASON_REQUIRED_ERROR: 'Please provide a reason for escalating.',
  ESCALATE_ERROR: 'Could not escalate this complaint. Please try again.',
  ESCALATED_ON_LABEL: 'Escalated on',
  REASON_LABEL: 'Reason',
  RESOLVED_BADGE_LABEL: 'Resolved',
  UNRESOLVED_BADGE_LABEL: 'Unresolved',
  RESOLUTION_DATE_LABEL: 'Resolution date',
  MARK_RESOLVED_TITLE: 'Mark Escalation as Resolved',
  RESOLUTION_DATE_INPUT_LABEL: 'Resolution Date',
  MARK_RESOLVED_SUBMIT_LABEL: 'Mark Resolved',
  MARKING_RESOLVED_LABEL: 'Saving...',
  RESOLUTION_DATE_REQUIRED_ERROR: 'Please select a resolution date.',
  UPDATE_ERROR: 'Could not update the escalation. Please try again.',
} as const;
