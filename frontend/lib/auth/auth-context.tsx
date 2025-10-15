"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import { AuthService } from "./auth-service";
import { User, LoginRequest } from "@/types/auth";
import { signalRService } from "../signalr/signalr-service";

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

  useEffect(() => {
    console.log("🔵 AuthProvider: useEffect starting...");

    // Check if user is already logged in
    const initAuth = async () => {
      try {
        console.log("🔵 Checking localStorage for user...");
        const currentUser = AuthService.getUser();
        const token = AuthService.getToken();

        console.log("🔵 User from localStorage:", currentUser);
        console.log("🔵 Token exists:", !!token);

        if (currentUser && token) {
          console.log("🔵 User found, setting user state...");
          setUser(currentUser);

          // Don't wait for SignalR in initial load
          console.log("🔵 Connecting to SignalR in background...");
          signalRService.connect(token).catch((error) => {
            console.error("❌ SignalR connection failed:", error);
          });
        } else {
          console.log("🔵 No user found in localStorage");
        }
      } catch (error) {
        console.error("❌ Auth initialization error:", error);
      } finally {
        console.log("🔵 Setting isLoading to false");
        setIsLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      console.log("🔵 Login attempt:", credentials.email);
      const response = await AuthService.login(credentials);

      const newUser: User = {
        id: response.user.id,
        email: response.user.email,
        name: `${response.user.firstName} ${response.user.lastName}`,
        role: response.user.role,
      };

      console.log("✅ Login successful:", newUser);
      setUser(newUser);

      // TODO: SignalR - Şimdilik devre dışı
      // signalRService.connect(response.refreshToken).catch((error) => {
      //   console.error('❌ SignalR connection failed:', error);
      // });
    } catch (error) {
      console.error("❌ Login error:", error);
      throw error;
    }
  };

  const logout = () => {
    console.log("🔵 Logging out...");
    signalRService.disconnect();
    AuthService.logout();
    setUser(null);
  };

  console.log(
    "🔵 AuthProvider render - isLoading:",
    isLoading,
    "isAuthenticated:",
    !!user
  );

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
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
