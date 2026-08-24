export type { AssignPermissionsRequestDto } from './manage-permissions.models';

export interface UserListItem {
  id: string;
  userName: string;
  email: string;
  phone?: string;
  role?: string;
  isActive: boolean;
}

export interface UpdateUserRequestDto {
  userName: string;
  phone: string;
  photoUrl: string;
}

export interface ManagementRoleDto {
  id: string;
  termValue: string;
  description: string;
  category: string;
}

export interface CategoryDto {
  id: string;
  name: string;
  description: string;
  img: string;
}

export interface UploadResponseDto {
  imageUrl: string;
}

export interface UserDetailDto {
  id: string;
  userName: string;
  email: string;
  phone?: string;
  photoUrl?: string;
  role?: string;
  flatId?: string;
  isActive: boolean;
  createdAt: string;
}

export interface RoleOptionDto {
  id: string;
  termValue: string;
  description: string;
  category: string;
}
