export const IDENTITY_ENDPOINTS = {
  USERS: '/users',
  ROLE_MANAGEMENT: '/role/management',
} as const;

export const MANAGEMENT_ROLE_NAMES = {
  ADMIN: 'Admin',
  STAFF: 'Staff',
} as const;

export const USER_LOOKUP_DEFAULTS = {
  ROLE_USERS_LIMIT: 100,
} as const;

export const USER_QUERY_PARAM = {
  ROLE_ID: 'roleId',
  LIMIT: 'limit',
} as const;
