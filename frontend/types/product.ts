export interface Product {
  id: string;
  name: string;
  sku: string;
  description?: string;
  price: number;
  stockQuantity: number;
  lowStockThreshold: number;
  isLowStock: boolean;
  isActive: boolean;
  createdAt: string;
}
// Request types
export interface CreateProductRequest {
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
  lowStockThreshold?: number;
  description?: string;
  isActive: boolean;
}

export interface UpdateProductRequest {
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
  lowStockThreshold?: number;
  description?: string;
  isActive: boolean;
}