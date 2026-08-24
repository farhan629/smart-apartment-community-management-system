export const AUTH_ENDPOINTS = {
  LOGIN: '/auth/login',
  SIGNUP: '/auth/signup',
  REGISTER: '/auth/register',
  REFRESH_TOKEN: '/auth/refresh-token',
  LOGOUT: '/auth/logout',
  PERMISSION_ME: '/Permission/me',
  FLATS: '/flats',
  ROLE_OCCUPANT: '/role/occupant',
  FORGOT_PASSWORD: '/auth/forgot-password',
  VERIFY_OTP: '/auth/verify-otp',
  RESET_PASSWORD: '/auth/reset-password',
  CHANGE_PASSWORD: '/auth/change-password',
} as const;

export const AUTH_STORAGE_KEYS = {
  ACCESS_TOKEN: 'accessToken',
  REFRESH_TOKEN: 'refreshToken',
  USER_ROLE: 'userRole',
  SESSION_TOKEN: 'sessionToken',
} as const;

export const AUTH_MESSAGES = {
  LOGIN_FAILED: 'Invalid email or password.',
  REGISTER_FAILED: 'Registration failed. Please try again.',
  REGISTER_SUCCESS: 'Account created. Please log in.',
  GENERIC_ERROR: 'Something went wrong. Please try again.',
} as const;

export const AUTH_ROUTES = {
  LOGIN: 'login',
  REGISTER: 'register',
  FORGOT_PASSWORD: 'forgot-password',
  RESET_PASSWORD: 'reset-password',
} as const;
