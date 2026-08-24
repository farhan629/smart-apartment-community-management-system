export interface NotificationDto {
  id: string;
  userId: string;
  templateId: string;
  visitId?: string;
  complaintId?: string;
  amenityBookingId?: string;
  title: string;
  message: string;
  isRead: boolean;
}

export interface GetNotificationsResponse {
  items: NotificationDto[];
  pagination: {
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
  };
}

export interface MarkAllReadResponse {
  updated: number;
}

export interface DeleteAllResponse {
  deleted: number;
}
