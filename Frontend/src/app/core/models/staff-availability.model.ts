export interface AvailabilitySlotDto {
  slotId: string;
  staffId: string;
  staffName: string;
  category: string;
  categoryId: string;
  date: string;
  startTime: string;
  endTime: string;
  isBooked: boolean;
  isCancelled: boolean;
  complaintId: string | null;
}

export interface CreateAvailabilitySlotItem {
  date: string;
  startTime: string;
  endTime: string;
}

export interface CreateAvailabilityRequestDto {
  slots: CreateAvailabilitySlotItem[];
}

export interface StaffAvailabilityFilterParams {
  staffId?: string;
  date?: string;
  categoryId?: string;
  isBooked?: boolean;
  fromDate?: string;
  toDate?: string;
}