export enum OrderStatus {
  Pending = 10,
  Confirmed = 20,
  Processing = 30,
  Completed = 40,
  Cancelled = 50,
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  productSKU: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  customerId: string;
  customerName: string;
  status: string;
  totalAmount: number;
  notes?: string;
  items: OrderItem[];
  createdAt: string;
}

export interface OrderFilterParams {
  startDate?: string;
  endDate?: string;
  status?: OrderStatus;
  customerId?: string;
  orderNumber?: string;
  customerName?: string;
  minAmount?: number;
  maxAmount?: number;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface OrderStatistics {
  totalOrders: number;
  totalRevenue: number;
  averageOrderValue: number;
  pendingOrders: number;
  confirmedOrders: number;
  processingOrders: number;
  completedOrders: number;
  cancelledOrders: number;
  todayOrders: number;
  todayRevenue: number;
  weekOrders: number;
  weekRevenue: number;
  monthOrders: number;
  monthRevenue: number;
  topCustomers: TopCustomer[];
  recentOrders: RecentOrderSummary[];
  dailyRevenue: DailyRevenue[];
}

export interface TopCustomer {
  customerId: string;
  customerName: string;
  orderCount: number;
  totalSpent: number;
}

export interface RecentOrderSummary {
  orderId: string;
  orderNumber: string;
  customerName: string;
  status: string;
  totalAmount: number;
  createdAt: string;
}

export interface DailyRevenue {
  date: string;
  orderCount: number;
  revenue: number;
}

export interface CreateOrderRequest {
  customerName: string;
  notes?: string;
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  productId: string;
  quantity: number;
}
