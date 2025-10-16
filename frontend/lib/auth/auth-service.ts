import axiosClient from "../api/axios-client";
import { LoginRequest, LoginResponse, User } from "@/types/auth";

export class AuthService {
  static async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await axiosClient.post<LoginResponse>(
      "/Auth/login",
      credentials
    );

    if (typeof window !== "undefined") {
      const accessToken = response.data.accessToken;

      if (accessToken) {
        localStorage.setItem("token", accessToken);
      } else {
        throw new Error("No access token received from server");
      }

      const user = {
        id: response.data.user.id,
        email: response.data.user.email,
        name: `${response.data.user.firstName} ${response.data.user.lastName}`,
        role: response.data.user.role,
      };

      localStorage.setItem("user", JSON.stringify(user));
    }

    return response.data;
  }

  static logout(): void {
    if (typeof window !== "undefined") {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
  }

  static getToken(): string | null {
    if (typeof window === "undefined") return null;
    return localStorage.getItem("token");
  }

  static getUser(): User | null {
    if (typeof window === "undefined") return null;

    const userStr = localStorage.getItem("user");
    if (!userStr) return null;

    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  }

  static isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
