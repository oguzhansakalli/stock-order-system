import axiosClient from "./axios-client";
import {
  Order,
  OrderFilterParams,
  PagedResult,
  OrderStatistics,
} from "@/types/order";

export class OrdersAPI {
  private static baseUrl = "/Orders";

  static async getOrdersWithFilters(
    params: OrderFilterParams
  ): Promise<PagedResult<Order>> {
    const response = await axiosClient.get<PagedResult<Order>>(
      `${this.baseUrl}/filter`,
      { params }
    );
    return response.data;
  }

  static async getOrders(): Promise<Order[]> {
    const response = await axiosClient.get<Order[]>(this.baseUrl);
    return response.data;
  }

  static async getOrderById(id: string): Promise<Order> {
    const response = await axiosClient.get<Order>(`${this.baseUrl}/${id}`);
    return response.data;
  }

  static async getOrdersByCustomer(customerId: string): Promise<Order[]> {
    const response = await axiosClient.get<Order[]>(
      `${this.baseUrl}/customer/${customerId}`
    );
    return response.data;
  }

  static async getStatistics(
    startDate?: string,
    endDate?: string
  ): Promise<OrderStatistics> {
    const params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;

    const response = await axiosClient.get<OrderStatistics>(
      `${this.baseUrl}/statistics`,
      { params }
    );
    return response.data;
  }

  static async createOrder(data: {
    customerName: string;
    notes?: string;
    items: Array<{
      productId: string;
      quantity: number;
    }>;
  }): Promise<Order> {
    const response = await axiosClient.post<Order>(this.baseUrl, data);
    return response.data;
  }

  static async cancelOrder(id: string, reason?: string): Promise<void> {
    await axiosClient.patch(`${this.baseUrl}/${id}/cancel`, { reason });
  }
}
