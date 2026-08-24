export interface StaffSummaryDto {
  staffId: string;
  userId: string;
  categoryName: string;
  description: string;
  isActive: boolean;
  createdAt: string;
  staffName?: string;
}

export interface StaffResponseDto {
  staffId: string;
  userId: string;
  categoryId: string;
  categoryName: string;
  description: string;
  details: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  staffName?: string;
}
