export interface BookingSummaryDto {
  bookingId: string;
  userId: string;
  amenityName: string | null;
  slotType: string | null;
  slotLabel: string | null;
  slotDate: string;
  startTime: string;
  endTime: string;
  peopleCount: number;
  status: string | null;
  bookedAt: string;
  cancelledAt: string | null;
}

export interface BookingPaginationDto {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface BookingListResponseDto {
  data: BookingSummaryDto[] | null;
  pagination: BookingPaginationDto;
}

export interface ReportSummaryDto {
  totalBookings: number;
  totalPeople: number;
  activeBookings: number;
  cancelledBookings: number;
  completedBookings: number;
  utilizationRate: number;
}

export interface ReportResponseDto {
  filters?: unknown;
  summary: ReportSummaryDto;
  bookings: BookingSummaryDto[] | null;
  pagination: BookingPaginationDto;
}

export interface GetBookingsFilters {
  status?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface GetBookingReportFilters {
  amenityId?: string;
  slotType?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number;
  pageSize?: number;
}