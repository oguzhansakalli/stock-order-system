import axiosClient from "./axios-client";
import {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
} from "@/types/product";

export class ProductsAPI {
  private static baseUrl = "/Products";

  static async getAllProducts(): Promise<Product[]> {
    const response = await axiosClient.get<Product[]>(this.baseUrl);
    return response.data;
  }
  static async getProductById(id: string): Promise<Product> {
    const response = await axiosClient.get<Product>(`${this.baseUrl}/${id}`);
    return response.data;
  }
  static async createProduct(product: CreateProductRequest): Promise<Product> {
    const response = await axiosClient.post<Product>(this.baseUrl, product);
    return response.data;
  }
  static async updateProduct(
    id: string,
    product: UpdateProductRequest
  ): Promise<Product> {
    const response = await axiosClient.put<Product>(
      `${this.baseUrl}/${id}`,
      product
    );
    return response.data;
  }
  static async deleteProduct(id: string): Promise<void> {
    await axiosClient.delete<void>(`${this.baseUrl}/${id}`);
  }
  static async getLowStockProducts(): Promise<Product[]> {
    const response = await axiosClient.get<Product[]>(
      `${this.baseUrl}/low-stock`
    );
    return response.data;
  }
  static async updateStock(id: string, quantity: number): Promise<void> {
    await axiosClient.patch(`${this.baseUrl}/${id}/stock`, { quantity });
  }
}
