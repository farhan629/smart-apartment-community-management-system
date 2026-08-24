import { Pagination } from './visitor.model';

export interface VisitQrTokenEmbed {
  token: string;
  isActive: boolean;
}
export interface RejectVisitRequestDto {
  rejectionReason: string;
}

export interface Visit {
  id: string;
  visitorId: string;
  visitorName: string;
  visitorPhoneNumber: string;
  visitorEmail?: string;
  visitorType: string;
  hostUserId: string;
  flatId: string;
  purposeTypeId: string;
  purpose: string;
  statusId: string;
  status: string;
  startDate: string; 
  endDate: string;
  checkInTime?: string;  
  checkOutTime?: string;
  approvedBy?: string;
  approvedDate?: string;
  rejectionReason?: string;
  qrToken?: VisitQrTokenEmbed;
}

export interface VisitCreateResponse {
  id: string;
  status: string;
  visitorName: string;
  flatId: string;
  purpose: string;
  startDate: string;
  endDate: string;
}

export interface GetVisitsResponse {
  items: Visit[];
  pagination: Pagination;
}

export interface CreateVisitorInlineRequest {
  name: string;
  phoneNumber: string;
  email?: string;
  visitorTypeId: string;
}

export interface CreateVisitRequest {
  visitorId?: string;
  visitor?: CreateVisitorInlineRequest;
  purposeTypeId: string;
  startDate: string;
  endDate: string;
  blockNumber?: string;
  flatNumber?: string;
}

export interface UpdateVisitRequest {
  purposeTypeId?: string;
  startDate?: string;
  endDate?: string;
}

export interface GetVisitsFilters {
  id?: string;
  visitorId?: string;
  hostUserId?: string;
  flatId?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
  sortBy?: string;
  sortOrder?: string;
  page?: number;
  limit?: number;
}