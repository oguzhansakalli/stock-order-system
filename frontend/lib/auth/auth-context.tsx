"use client";
import React, { createContext, useContext, useState, useEffect } from "react";
import { AuthService } from "./auth-service";
import { User, LoginRequest } from "@/types/auth";
import { signalRService } from "../signalr/signalr-service";
import toast from "react-hot-toast";

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    if (initialized) return;

    try {
      const token = AuthService.getToken();
      const currentUser = AuthService.getUser();

      if (token && currentUser && token.length > 100) {
        setUser(currentUser);

        // Connect SignalR on app initialization
        connectSignalR(token);
      } else {
        setUser(null);
      }
    } catch (error) {
      console.error("Auth initialization error:", error);
      setUser(null);
    } finally {
      setIsLoading(false);
      setInitialized(true);
    }
  }, [initialized]);

  const connectSignalR = async (token: string) => {
    try {
      await signalRService.connect(token);
      setupSignalRListeners();
    } catch (error) {
      console.error("SignalR connection error:", error);
    }
  };

  const setupSignalRListeners = () => {
    // Order Created
    signalRService.onOrderCreated((data) => {
      console.log("Order created:", data);
      toast.success(`New order created: ${data.orderNumber}`, {
        duration: 5000,
      });
    });

    // Order Status Changed
    signalRService.onOrderStatusChanged((data) => {
      console.log("Order status changed:", data);
      toast(`Order ${data.orderNumber} status: ${data.newStatus}`, {
        icon: "📦",
        duration: 4000,
      });
    });

    // Order Cancelled
    signalRService.onOrderCancelled((data) => {
      console.log("Order cancelled:", data);
      toast.error(`Order ${data.orderNumber} has been cancelled`);
    });
  };

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await AuthService.login(credentials);
      const newUser: User = {
        id: response.user.id,
        email: response.user.email,
        name: `${response.user.firstName} ${response.user.lastName}`,
        role: response.user.role,
      };
      setUser(newUser);

      // Connect SignalR after login
      await connectSignalR(response.accessToken);

      toast.success("Logged in successfully!");
    } catch (error) {
      console.error("Login error:", error);
      toast.error("Login failed. Please check your credentials.");
      throw error;
    }
  };

  const logout = () => {
    AuthService.logout();
    signalRService.disconnect();
    setUser(null);
    toast.success("Logged out successfully");
  };

  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
