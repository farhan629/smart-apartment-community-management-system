export const VIEW_MODE = {
  MINE: 'mine',
  ALL: 'all',
} as const;

export const BOOKING_STATUS = {
  BOOKED: 'booked',
  CONFIRMED: 'confirmed',
  PENDING: 'pending',
  APPROVED: 'approved',
  UPCOMING: 'upcoming',
  COMPLETED: 'completed',
  SUCCESS: 'success',
  CANCELLED: 'cancelled',
  REJECTED: 'rejected',
  IN_PROGRESS: 'in progress',
} as const;

export const BADGE_CLASSES = {
  SUCCESS: 'history-badge--success',
  INFO: 'history-badge--info',
  DANGER: 'history-badge--danger',
  NEUTRAL: 'history-badge--neutral',
} as const;

export const AMENITY_ICONS = {
  POOL: { icon: 'pool', bg: '#0d7d6f' },
  GYM: { icon: 'fitness_center', bg: '#6366f1' },
  COURT: { icon: 'sports_basketball', bg: '#d97706' },
  CLUBHOUSE: { icon: 'meeting_room', bg: '#0f766e' },
  DEFAULT: { icon: 'event', bg: '#6b7280' },
} as const;

export const BOOKING_CANCEL = {
  MESSAGE: 'Are you sure you want to cancel this booking?',
  REASON: 'Cancelled by resident',
  SUCCESS: 'Booking cancelled successfully.',
  FAILED: 'Failed to cancel booking. Please try again.',
} as const;

export const SLOT_TYPES = {
  TIME_COUNT: 'TIME_COUNT',
  TIME: 'TIME',
} as const;

export const SLOT_TYPE_LABELS = {
  SHARED: 'Shared Session',
  PRIVATE: 'Private Booking',
} as const;

export const AMENITY_STATUS = {
  AVAILABLE: 'AVAILABLE',
  LIMITED: 'LIMITED',
  RESERVED: 'RESERVED SOON',
} as const;

export const AMENITY_STATUS_CLASSES = {
  AVAILABLE: 'status-badge--available',
  LIMITED: 'status-badge--limited',
  RESERVED: 'status-badge--reserved',
} as const;

export const AMENITY_DASHBOARD_STRINGS = {
  PAGE_TITLE: 'Amenities Booking',
  PAGE_SUBTITLE: 'Reserve community facilities for your personal use. Please ensure you follow the residency safety guidelines for each space.',
  FILTER_BUTTON: 'Filters',
  BANNER_BADGE: 'COMMUNITY UPDATE',
  BANNER_TITLE_1: 'Resident Community Spaces',
  BANNER_TITLE_2: 'Are Now Open 24/7',
  BANNER_DESCRIPTION: 'Enjoy our updated facilities with new hygiene standards and automated entry systems.',
  EXPLORE_BUTTON: 'Explore All',
  BANNER_ALT: 'Community Park',
  ADD_AMENITY_BUTTON: 'Create New Amenity',
} as const;

export const AMENITY_CARDS_STRINGS = {
  CAPACITY_PREFIX: 'Cap: ',
  EMPTY_STATE: 'No amenities found.',
  MENU_UPDATE: 'Update Amenity',
  MENU_DELETE: 'Delete Amenity',
  MENU_SLOT: 'Slot',
} as const;

export const BOOKING_HISTORY_STRINGS = {
  TAB_MY_BOOKINGS: 'My Bookings',
  TAB_ALL_BOOKINGS: 'All Bookings',
  TITLE_ALL_BOOKINGS: 'All Community Bookings',
  TITLE_MY_BOOKINGS: 'Recent Booking History',
  VIEW_ALL_LINK: 'View All History',
  COL_AMENITY: 'Amenity',
  COL_DATE: 'Date',
  COL_TIME_SLOT: 'Time Slot',
  COL_GUESTS: 'Guests',
  COL_STATUS: 'Status',
  COL_ACTION: 'Action',
  CANCEL_BOOKING: 'Cancel Booking',
  NO_ACTIONS: 'No Actions',
  EMPTY_STATE: 'No booking history found.',
  NO_GUESTS: 'No Guests',
  ONE_GUEST: '1 Guest',
  GUESTS_SUFFIX: ' Guests',
  PAGE_LABEL: 'Page',
  OF_LABEL: 'of',
  ERROR_MSG: 'Error loading user bookings' ,
  ERROR_MSG2: 'Error loading admin booking report',
  STATUS_ALL: 'All Statuses',
  STATUS_BOOKED: 'Booked',
  STATUS_COMPLETED: 'Completed',
  STATUS_CANCELLED: 'Cancelled',
  FILTER_STATUS: 'Status',
  FILTER_FROM_DATE: 'From Date',
  FILTER_TO_DATE: 'To Date',
  FILTER_RESET: 'Reset',
} as const;

export const AMENITY_CALENDER_STRINGS = {
  TITLE: 'Select Date',
} as const;

export const AMENITY_BOOKING_STRINGS = {
  TITLE: 'Available Slots',
  LEGEND_BOOKED: 'Booked',
  LEGEND_AVAILABLE: 'Available',
  LOADING: 'Loading available slots...',
  MORNING_TITLE: 'MORNING SESSIONS',
  AFTERNOON_TITLE: 'AFTERNOON SESSIONS',
  EVENING_TITLE: 'EVENING SESSIONS',
  FULLY_BOOKED: 'Fully Booked',
  SPOTS_LEFT: ' spots left',
  EMPTY_MORNING: 'No morning slots available for this date.',
  EMPTY_AFTERNOON: 'No afternoon slots available for this date.',
  EMPTY_EVENING: 'No evening slots available for this date.',
} as const;

export const AMENITY_DOWNBAR_STRINGS = {
  DATE_LABEL: 'DATE SELECTED',
  TIME_LABEL: 'TIME SLOT',
  TIME_PLACEHOLDER: 'Select a time slot',
  CANCEL_BUTTON: 'Cancel Selection',
  CONFIRM_BUTTON: 'Confirm Booking',
} as const;

export const AMENITY_BOOKING_PAGE_STRINGS = {
  TOAST_CONFIRMED: 'Booking confirmed successfully!',
  HEADER_SUBTITLE: 'Select a date and an available slot to reserve this amenity.',
  POLICY_TITLE: 'Booking Policy',
  POLICY_TEXT: 'Bookings must be made 24 hours in advance. Cancellations allowed up to 4 hours before slot.',
  BOOKING_FAILED: 'Booking failed. Please try again.',
  DIALOG_TITLE: 'Confirm Booking',
  DIALOG_SUBTITLE: 'Please review the reservation details before confirming.',
  BTN_CONFIRM: 'Confirm Booking',
  BTN_BOOKING: 'Booking...',
  BTN_CANCEL: 'Cancel',
  BOOKING_TITLE_SUFFIX: ' Booking',
} as const;

export const AMENITY_ROUTES = {
  BASE: '/amenities',
  BOOK_SUFX: 'book',
  BOOKINGS_SUFX: 'bookings',
  CANCEL_SUFX: 'cancel',
} as const;

export const AMENITY_ICON_KEYWORDS = {
  POOL: 'pool',
  GYM: ['gym', 'fitness'],
  COURT: ['court', 'sports', 'basketball', 'tennis'],
  CLUBHOUSE: 'clubhouse',
} as const;

export const API_GATEWAY_REPLACE = '/gateway';

export const STATUS_KEYWORDS = {
  LIMIT: 'limit',
  RESERVED: ['reserve', 'full', 'busy', 'unavailable'],
} as const;

export const AMENITY_DEFAULTS = {
  NAME: 'Amenity',
} as const;

export const WEEK_DAYS = ['MO', 'TU', 'WE', 'TH', 'FR', 'SA', 'SU'] as const;

export const TOAST_DURATION = 2200;

export const CALENDER_NUMBERS = {
  FIRST_DAY: 1,
  ZERO: 0,
  SUNDAY_INDEX: 6,
  MONTH_OFFSET_PREV: -1,
  MONTH_OFFSET_NEXT: 1,
  HOURS_MIDNIGHT: 0,
  MINUTES_MIDNIGHT: 0,
  SECONDS_MIDNIGHT: 0,
  MILLISECONDS_MIDNIGHT: 0,
} as const;

export const BOOKING_PAGE_NUMBERS = {
  TZ_MS_MULTIPLIER: 60000,
  DEFAULT_PEOPLE_COUNT: 1,
} as const;

export const SLOT_HOURS = {
  MORNING_START: 6,
  AFTERNOON_START: 12,
  EVENING_START: 18,
  RADIX_DECIMAL: 10,
} as const;

export const DASHBOARD_SCROLL = {
  ELEMENT_ID: 'amenities-list',
  BEHAVIOR: 'smooth' as const,
  BLOCK: 'start' as const,
} as const;

export const BOOKING_HISTORY_PAGE_STRINGS = {
  PAGE_TITLE: 'Booking History',
  BACK_LINK: 'Back to Amenities',
  BREADCRUMB_HOME: 'Dashboard',
  BREADCRUMB_AMENITIES: 'Amenities',
  BREADCRUMB_HISTORY: 'History',
} as const;

export const DASHBOARD_NUMBERS = {
  HISTORY_LIMIT: 5,
} as const;

export const PAGINATION_NUMBERS = {
  DEFAULT_PAGE: 1,
  PAGE_SIZE: 5,
} as const;

export const CANCELLATION_STRINGS = {
  PAGE_TITLE: 'Cancel Booking',
  SUBTITLE: 'Please confirm details and specify a reason for cancellation.',
  LABEL_AMENITY: 'Amenity',
  LABEL_DATE: 'Date',
  LABEL_SLOT: 'Time Slot',
  LABEL_STATUS: 'Status',
  LABEL_REASON: 'Cancellation Reason',
  PLACEHOLDER_REASON: 'Why are you cancelling this booking? (e.g. Schedule change, unwell, etc.)',
  BTN_BACK: 'Go Back',
  BTN_CONFIRM: 'Confirm Cancellation',
  BTN_CANCELLING: 'Cancelling...',
  TOAST_SUCCESS: 'Booking cancelled successfully!',
  ERROR_LOAD: 'Failed to load booking details.',
  ERROR_SUBMIT: 'Cancellation failed. Please try again.',
  VALIDATION_REASON: 'Cancellation reason is required.',
  CONFIRM_ALERT: 'Do you want to cancel the booking?',
  LOADING_DETAILS: 'Loading booking details...',
} as const;

export const SLOT_TYPE_IDS = {
  TIME_COUNT: '11111111-1111-1111-1111-111111111111',
  TIME: '22222222-2222-2222-2222-222222222222',
} as const;

export const AMENITY_STATUS_IDS = {
  AVAILABLE: '33333333-3333-3333-3333-333333333333',
  MAINTENANCE: '44444444-4444-4444-4444-444444444444',
} as const;

export const ADD_NEW_AMENITY_STRINGS = {
  PAGE_TITLE: 'Create New Amenity',
  PAGE_SUBTITLE: 'Configure details and policies to add a new facility for community use.',
  LABEL_NAME: 'Amenity Name',
  PLACEHOLDER_NAME: 'e.g. Squash Court, Barbecue Area',
  LABEL_LOCATION: 'Location / Floor',
  PLACEHOLDER_LOCATION: 'e.g. Building B, 3rd Floor',
  LABEL_SLOT_TYPE: 'Slot Type / Booking Policy',
  LABEL_STATUS: 'Status',
  LABEL_RULES: 'Booking Rules & Guidelines',
  PLACEHOLDER_RULES: 'e.g. Maximum booking duration is 2 hours. Proper sports attire required.',
  LABEL_IMAGE: 'Facility Image',
  PLACEHOLDER_IMAGE: 'Select an image file or drop it here',
  BTN_BACK: 'Back to Dashboard',
  BTN_SUBMIT: 'Create Amenity',
  BTN_SUBMITTING: 'Creating Facility...',
  TOAST_SUCCESS: 'Amenity created successfully!',
  ERROR_SUBMIT: 'Failed to create amenity. Please check input values.',
  ERROR_UPLOAD: 'Image upload failed. Please try again.',
  VALIDATION_NAME: 'Amenity name is required.',
  VALIDATION_LOCATION: 'Location is required.',
  LABEL_SLOT_TIME_COUNT: 'Time Count (Shared / Private Slots)',
  LABEL_SLOT_TIME_ONLY: 'Time Only (Simple Reservation)',
  LABEL_STATUS_AVAILABLE: 'Available',
  LABEL_STATUS_MAINTENANCE: 'Under Maintenance',
  LABEL_UPLOADING_IMAGE: 'Uploading image...',
  BTN_CANCEL: 'Cancel',
} as const;

export const UPDATE_AMENITY_STRINGS = {
  PAGE_TITLE: 'Update Amenity',
  BTN_SUBMIT: 'Save Changes',
  BTN_SUBMITTING: 'Saving Changes...',
  TOAST_SUCCESS: 'Amenity updated successfully!',
  ERROR_SUBMIT: 'Failed to update amenity. Please check input values.',
  BTN_CANCEL: 'Cancel',
} as const;

export const DELETE_AMENITY_STRINGS = {
  PAGE_TITLE: 'Delete Amenity',
  BTN_SUBMIT: 'Delete Amenity',
  BTN_SUBMITTING: 'Deleting...',
  MESSAGE_CONFIRM: 'Are you sure you want to delete this amenity? This action cannot be undone.',
  TOAST_SUCCESS: 'Amenity deleted successfully!',
  ERROR_SUBMIT: 'Failed to delete amenity. Please try again.',
  LABEL_AMENITY: 'Amenity:',
  BTN_CANCEL: 'Cancel',
} as const;

export const AMENITY_SLOT_STRINGS = {
  TITLE_MANAGE: 'Manage Slots',
  TAB_CREATE: 'Create Slots',
  TAB_MANAGE: 'Manage Slots',
  FORM_TITLE: 'Add Slot Details',
  LABEL_SLOT_LABEL: 'Slot Label (e.g. Morning)',
  PLACEHOLDER_SLOT_LABEL: 'e.g. Morning Slot',
  LABEL_DATE: 'Slot Date *',
  LABEL_START_TIME: 'Start Time *',
  LABEL_END_TIME: 'End Time *',
  LABEL_MAX_CAPACITY: 'Max Capacity *',
  BTN_ADD_QUEUE: 'Add to Queue',
  TITLE_QUEUE: 'Queue',
  BTN_SUBMIT_QUEUE: 'Submit All Slots',
  COL_LABEL: 'Label',
  COL_DATE: 'Date',
  COL_TIME_RANGE: 'Time Range',
  COL_CAPACITY: 'Capacity',
  COL_BOOKINGS: 'Bookings',
  EMPTY_SLOTS: 'No slots found for this amenity.',
  EMPTY_QUEUE: 'No slots added to the queue yet. Use the form on the left to add slots.',
} as const;
