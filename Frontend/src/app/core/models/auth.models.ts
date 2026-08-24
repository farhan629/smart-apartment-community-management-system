export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface RegisterRequestDto {
  userName: string;
  email: string;
  password: string;
  phone: string;
  role_id: string;
  flat_id: string;
  photo?: File;
}

export interface RegisterManagementRequestDto {
  userName: string;
  email: string;
  password: string;
  phone: string;
  role_id: string;
  category_id?: string;
  photo?: File;
}

export interface LoginResponseDto {
  token: string;
  expiresAt: string;
}

export interface SuccessResponseDto {
  success: boolean;
  message: string;
}

export interface RefreshTokenResponseDto {
  token: string;
  expiresAt: string;
}

export interface ForgotPasswordRequestDto {
  phone: string;
}

export interface ForgotPasswordResponseDto {
  success: boolean;
  message: string;
}

export interface VerifyOtpResponseDto {
  resetToken: string;
}
export interface VerifyOtpDto {
  phone: string;
  otp: string;
}

export interface ResetPasswordDto {
  resetToken: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface OccupantRoleDto {
  id: string;
  termValue: string;
  description: string;
  category: string;
}

export interface FlatItemDto {
  id: string;
  number: string;
  block: string;
  floor: number;
  isAvailable: boolean;
  createdAt: string;
}

export interface FlatPaginationDto {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface FlatResponseDto {
  items: FlatItemDto[];
  pagination: FlatPaginationDto;
}

export interface PermissionResponseDto {
  userId: string;
  roleId: string;
  roleName: string;
  permissions: string[];
}
