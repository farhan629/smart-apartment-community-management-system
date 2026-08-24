export const ROLES = {
  ADMIN: 'Admin',
  RESIDENT: 'Resident',
  SECURITY: 'Security',
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];