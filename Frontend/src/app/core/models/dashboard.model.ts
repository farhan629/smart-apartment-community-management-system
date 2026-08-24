export type AccentKey = 'success' | 'info' | 'danger' | 'primary';

export interface StatCard {
  label: string;
  value: string;
  icon: string;
  accent: AccentKey;
}

export interface CategoryTrendPoint {
  category: string;
  value: number;
}

export interface ComplaintRow {
  complaintId: string;
  category: string;
  description: string;
  status: string;
  priority: string;
  createdAt: string;
}

export interface DashboardData {
  cards: StatCard[];
  trend: CategoryTrendPoint[];
  resolvedCount: number;
  pendingCount: number;
  complaints: ComplaintRow[];
}
