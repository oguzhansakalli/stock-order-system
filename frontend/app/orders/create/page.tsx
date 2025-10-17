"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import DashboardLayout from "@/components/layout/dashboard-layout";
import { OrdersAPI } from "@/lib/api/orders-api";
import { ProductsAPI } from "@/lib/api/products-api";
import { Product } from "@/types/product";
import { CreateOrderRequest, CreateOrderItemRequest } from "@/types/order";
import {
  ArrowLeftIcon,
  PlusIcon,
  TrashIcon,
} from "@heroicons/react/24/outline";

interface OrderItem extends CreateOrderItemRequest {
  product?: Product;
}

export default function CreateOrderPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [products, setProducts] = useState<Product[]>([]);
  const [loadingProducts, setLoadingProducts] = useState(true);

  const [customerName, setCustomerName] = useState("");
  const [notes, setNotes] = useState("");
  const [items, setItems] = useState<OrderItem[]>([
    { productId: "", quantity: 1 },
  ]);

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoadingProducts(true);
      const data = await ProductsAPI.getAllProducts();
      setProducts(data.filter((p) => p.isActive));
    } catch (err) {
      console.error("Failed to load products:", err);
      alert("Failed to load products");
    } finally {
      setLoadingProducts(false);
    }
  };

  const addItem = () => {
    setItems([...items, { productId: "", quantity: 1 }]);
  };

  const removeItem = (index: number) => {
    if (items.length === 1) {
      alert("Order must have at least one item");
      return;
    }
    setItems(items.filter((_, i) => i !== index));
  };

  const updateItem = (index: number, field: keyof OrderItem, value: any) => {
    const newItems = [...items];
    newItems[index] = { ...newItems[index], [field]: value };

    if (field === "productId") {
      const product = products.find((p) => p.id === value);
      newItems[index].product = product;
    }

    setItems(newItems);
  };

  const calculateTotal = () => {
    return items.reduce((total, item) => {
      const product = products.find((p) => p.id === item.productId);
      return total + (product ? product.price * item.quantity : 0);
    }, 0);
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("tr-TR", {
      style: "currency",
      currency: "TRY",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!customerName.trim()) {
      alert("Customer Name is required");
      return;
    }

    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      if (!item.productId) {
        alert(`Please select a product for item ${i + 1}`);
        return;
      }
      if (item.quantity <= 0) {
        alert(`Quantity for item ${i + 1} must be greater than 0`);
        return;
      }

      const product = products.find((p) => p.id === item.productId);
      if (product && item.quantity > product.stockQuantity) {
        alert(
          `Not enough stock for ${product.name}. Available: ${product.stockQuantity}`
        );
        return;
      }
    }

    const orderRequest: CreateOrderRequest = {
      customerName: customerName.trim(),
      notes: notes.trim() || undefined,
      items: items.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
    };

    try {
      setLoading(true);
      const newOrder = await OrdersAPI.createOrder(orderRequest);
      alert(
        `Order created successfully!\nOrder Number: ${newOrder.orderNumber}\nCustomer ID: ${newOrder.customerId}`
      );
      router.push(`/orders/${newOrder.id}`);
    } catch (err: any) {
      console.error("Failed to create order:", err);
      alert(err.response?.data?.message || "Failed to create order");
    } finally {
      setLoading(false);
    }
  };

  return (
    <DashboardLayout>
      <div className="max-w-4xl mx-auto">
        <div className="mb-6">
          <button
            onClick={() => router.push("/orders")}
            className="flex items-center text-gray-600 hover:text-gray-900 mb-4"
          >
            <ArrowLeftIcon className="w-5 h-5 mr-2" />
            Back to Orders
          </button>
          <h1 className="text-3xl font-bold text-gray-900">Create New Order</h1>
          <p className="text-gray-500 mt-1">Fill in the order details</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Customer Information */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-4">
              Customer Information
            </h2>
            <div>
              <label
                htmlFor="customerName"
                className="block text-sm font-medium text-gray-700 mb-2"
              >
                Customer Name <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                id="customerName"
                value={customerName}
                onChange={(e) => setCustomerName(e.target.value)}
                required
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-900 placeholder:text-gray-400"
                placeholder="Enter customer name"
              />
              <p className="text-xs text-gray-500 mt-1">
                Customer ID will be automatically generated
              </p>
            </div>
          </div>

          {/* Order Items */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-900">
                Order Items
              </h2>
              <button
                type="button"
                onClick={addItem}
                className="flex items-center gap-2 px-3 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm"
              >
                <PlusIcon className="w-4 h-4" />
                Add Item
              </button>
            </div>

            {loadingProducts ? (
              <div className="text-center py-8">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
                <p className="text-gray-500 mt-2">Loading products...</p>
              </div>
            ) : (
              <div className="space-y-4">
                {items.map((item, index) => {
                  const selectedProduct = products.find(
                    (p) => p.id === item.productId
                  );
                  const itemTotal = selectedProduct
                    ? selectedProduct.price * item.quantity
                    : 0;

                  return (
                    <div
                      key={index}
                      className="border border-gray-200 rounded-lg p-4"
                    >
                      <div className="flex items-start gap-4">
                        <div className="flex-1 grid grid-cols-1 md:grid-cols-3 gap-4">
                          <div className="md:col-span-2">
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                              Product <span className="text-red-500">*</span>
                            </label>
                            <select
                              value={item.productId}
                              onChange={(e) =>
                                updateItem(index, "productId", e.target.value)
                              }
                              required
                              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-900"
                            >
                              <option value="">Select a product</option>
                              {products.map((product) => (
                                <option key={product.id} value={product.id}>
                                  {product.name} -{" "}
                                  {formatCurrency(product.price)} (Stock:{" "}
                                  {product.stockQuantity})
                                </option>
                              ))}
                            </select>
                            {selectedProduct &&
                              selectedProduct.stockQuantity < 10 && (
                                <p className="text-xs text-orange-600 mt-1">
                                  Low stock: {selectedProduct.stockQuantity}{" "}
                                  remaining
                                </p>
                              )}
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                              Quantity <span className="text-red-500">*</span>
                            </label>
                            <input
                              type="number"
                              min="1"
                              max={selectedProduct?.stockQuantity || 9999}
                              value={item.quantity}
                              onChange={(e) =>
                                updateItem(
                                  index,
                                  "quantity",
                                  parseInt(e.target.value) || 1
                                )
                              }
                              required
                              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-900"
                            />
                          </div>
                        </div>

                        <div className="flex flex-col items-end gap-2">
                          {selectedProduct && (
                            <div className="text-right">
                              <p className="text-sm text-gray-500">
                                Item Total
                              </p>
                              <p className="text-lg font-semibold text-gray-900">
                                {formatCurrency(itemTotal)}
                              </p>
                            </div>
                          )}
                          <button
                            type="button"
                            onClick={() => removeItem(index)}
                            className="text-red-600 hover:text-red-900 p-2"
                            title="Remove item"
                          >
                            <TrashIcon className="w-5 h-5" />
                          </button>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          {/* Notes */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-4">
              Additional Notes
            </h2>
            <textarea
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={4}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-900 placeholder:text-gray-400 resize-none"
              placeholder="Add any notes for this order..."
            />
          </div>

          {/* Order Summary */}
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold text-gray-900 mb-4">
              Order Summary
            </h2>
            <div className="space-y-3">
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Items Count:</span>
                <span className="font-medium text-gray-900">
                  {items.reduce((sum, item) => sum + item.quantity, 0)} items
                </span>
              </div>
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Products:</span>
                <span className="font-medium text-gray-900">
                  {items.filter((item) => item.productId).length} /{" "}
                  {items.length} selected
                </span>
              </div>
              <div className="pt-3 border-t border-gray-200">
                <div className="flex justify-between">
                  <span className="text-lg font-semibold text-gray-900">
                    Total Amount:
                  </span>
                  <span className="text-2xl font-bold text-blue-600">
                    {formatCurrency(calculateTotal())}
                  </span>
                </div>
              </div>
            </div>
          </div>

          {/* Actions */}
          <div className="flex items-center justify-end gap-4">
            <button
              type="button"
              onClick={() => router.push("/orders")}
              className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading || loadingProducts}
              className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? "Creating Order..." : "Create Order"}
            </button>
          </div>
        </form>
      </div>
    </DashboardLayout>
  );
}
