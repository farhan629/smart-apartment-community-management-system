export interface ApprovalDetailDto {
  id: string;
  userId: string;
  userName: string | null;
  email: string | null;
  flatId: string;
  flatNumber: string | null;
  block: string | null;
  residentType: string | null;
  isApproved: boolean;
  status: string;
  remarks: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface PaginatedApprovalResponseDto {
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  items: ApprovalDetailDto[];
}

export interface UpdateApprovalRequestDto {
  isApproved: boolean;
  remarks?: string | null;
}

export interface UpdateApprovalResponseDto {
  message: string;
  approval: ApprovalDetailDto;
}
