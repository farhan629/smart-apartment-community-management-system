import { environment } from '../../../environments/environment';

const GATEWAY = environment.apiBaseUrl;

export const API_CONFIG = {
  GATEWAY,
  COMPLAINTS: `${GATEWAY}/complaint`,
  ASSIGNMENTS: `${GATEWAY}/complaint/assignments`,
  STAFF_ASSIGNMENTS: `${GATEWAY}/staff/assignments`,
  ESCALATIONS: `${GATEWAY}/escalations`,
  STAFF: `${GATEWAY}/staff`,
  STAFF_AVAILABILITY: `${GATEWAY}/staff/availability`,
  REPORTS: `${GATEWAY}/reports`,
  JOBS: `${GATEWAY}/jobs`,
  LOOKUPS: `${GATEWAY}/complaint/lookups`,
  LOOKUP_CATEGORIES: `${GATEWAY}/complaint/lookups/categories`,
  AMENITY_UPLOAD: `${GATEWAY}/amenity/upload`,
  PERMISSIONS_ME: `${GATEWAY}/permission/me`,
  PERMISSIONS: `${GATEWAY}/permission`,
  USERS: `${GATEWAY}/users`,
  APPROVALS: `${GATEWAY}/approval`,
  ROLE_MANAGEMENT: `${GATEWAY}/role/management`,
  ROLE_OCCUPANT: `${GATEWAY}/role/occupant`,
  AUTH: {
    LOGIN: `${GATEWAY}/auth/login`,
    REGISTER: `${GATEWAY}/auth/register`,
  },
  VISITS: `${GATEWAY}/visits`,
  VISITORS: `${GATEWAY}/visitor`,
  VISITOR_TYPES: `${GATEWAY}/visitor-types`,
  PURPOSE_TYPES: `${GATEWAY}/purpose-types`,
  VISIT_CHECKIN: `${GATEWAY}/visits/checkin`,
  VISIT_CHECKOUT: `${GATEWAY}/visits/checkout`,
  BOOKING: `${GATEWAY}/booking`,
  BOOKING_REPORT: `${GATEWAY}/booking/report`,
} as const;
