export interface UserSearchResultDto {
  id: string;
  userName: string;
  email: string;
  phone?: string;
  role?: string;
  isActive: boolean;
}

export interface PaginatedUserResponse {
  items: UserSearchResultDto[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
}

export interface UserPermissionResponseDto {
  userId: string;
  permissions: string[];
}

export interface PermissionAssignItemDto {
  permissionCode: string;
  isAllowed: boolean;
}

export interface AssignPermissionsRequestDto {
  userId: string;
  permissions: PermissionAssignItemDto[];
}
