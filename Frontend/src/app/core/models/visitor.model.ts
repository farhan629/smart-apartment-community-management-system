export interface Visitor {
  id: string;
  name: string;
  phoneNumber: string;
  email?: string;
  visitorTypeId: string;
  photoUrl?: string;
  visitorType: string;
}

export interface CreateVisitorRequest {
  name: string;
  phoneNumber: string;
  email?: string;
  visitorTypeId: string;
}

export interface UpdateVisitorRequest {
  name?: string;
  phoneNumber?: string;
  email?: string;
  visitorTypeId?: string;
}

export interface GetVisitorsResponse {
  items: Visitor[];
  pagination: Pagination;
}

export interface Pagination {
  page: number;
  limit: number;
  totalCount: number;
  totalPages?: number;
}

export interface RefTermOption {
  id: string;
  name: string;
}
export interface UpdateVisitorRequest {
  name?: string;
  phoneNumber?: string;
  email?: string;
  visitorTypeId?: string;
}