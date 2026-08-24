import {
  EDITABLE_VISIT_STATUSES,
  APPROVABLE_VISIT_STATUSES,
  CANCELLABLE_VISIT_STATUSES,
} from '../constants/visit.constants';

export function canUpdateVisit(status: string): boolean {
  return (EDITABLE_VISIT_STATUSES as string[]).includes(status);
}

export function canApproveVisit(status: string): boolean {
  return (APPROVABLE_VISIT_STATUSES as string[]).includes(status);
}

export function canCancelVisit(status: string): boolean {
  return (CANCELLABLE_VISIT_STATUSES as string[]).includes(status);
}