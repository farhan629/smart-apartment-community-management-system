export interface UserLookupDto {
  id: string;
  userName: string | null;
  email: string | null;
  phone: string | null;
  photoUrl: string | null;
  role: string | null;
  flatId: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface RoleLookupDto {
  id: string;
  termValue: string;
  description: string;
  category: string;
}

export interface IdentityPagedResult<T> {
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  items: T[];
}
