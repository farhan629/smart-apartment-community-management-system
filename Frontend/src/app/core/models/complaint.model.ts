export interface ComplaintSummaryDto {
  complaintId: string;
  residentId: string;
  description: string;
  complaintType: string;
  category: string;
  categoryId: string;
  categoryImg: string | null;
  priority: string;
  status: string;
  scheduledDate: string | null;
  scheduledTime: string | null;
  createdAt: string;
}

export interface ComplaintDetailDto extends ComplaintSummaryDto {
  scheduledSlotId: string | null;
  cancelledAt: string | null;
  cancellationReason: string | null;
  updatedAt: string | null;
}

export interface CreateComplaintRequestDto {
  complaintTypeRefId: string;
  categoryId: string;
  priorityRefId: string;
  description: string;
  preferredDate: string;
  preferredTime?: string | null;
}
export interface ComplaintStatusUpdateRequestDto {
  status: 'InProgress' | 'Resolved';
}

export interface ComplaintCancelRequestDto {
  cancellationReason: string;
}
export interface ComplaintFilterParams {
  status?: string;
  priority?: string;
  categoryId?: string;
  fromDate?: string;
  toDate?: string;
  page?: number;
  limit?: number;
}