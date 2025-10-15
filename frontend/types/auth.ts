export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  refreshToken: string;
  expiresIn: string;
  user: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
    isActive: boolean;
  };
}

export interface User {
  id: string;
  email: string;
  name: string;
  role?: string;
}
