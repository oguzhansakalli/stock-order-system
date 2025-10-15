import axiosClient from '../api/axios-client';
import { LoginRequest, LoginResponse, User } from '@/types/auth';

export class AuthService {
  static async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await axiosClient.post<LoginResponse>('/Auth/login', credentials);
    
    console.log('🔵 Login response:', response.data);
    
    // Save token and user to localStorage (only in browser)
    if (typeof window !== 'undefined') {
      // Use refreshToken as token
      localStorage.setItem('token', response.data.refreshToken);
      
      // Map backend user structure to frontend User
      const user = {
        id: response.data.user.id,
        email: response.data.user.email,
        name: `${response.data.user.firstName} ${response.data.user.lastName}`,
        role: response.data.user.role,
      };
      
      localStorage.setItem('user', JSON.stringify(user));
      console.log('✅ Token and user saved to localStorage');
    }
    
    return response.data;
  }

  static logout(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
  }

  static getToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('token');
  }

  static getUser(): User | null {
    if (typeof window === 'undefined') return null;
    
    const userStr = localStorage.getItem('user');
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