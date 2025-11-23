import api from './api';
import type { LoginRequest, AuthenticationResult, RegisterRequest, UserProfile } from '../types';

export const login = async (data: LoginRequest): Promise<AuthenticationResult> => {
  const response = await api.post<AuthenticationResult>('/auth/login', data);
  if (response.data.token) {
    localStorage.setItem('access_token', response.data.token);
    localStorage.setItem('user_data', JSON.stringify(response.data.user));
  }
  return response.data;
};

export const registerUser = async (data: RegisterRequest): Promise<void> => {
  await api.post('/auth/register', data);
};

export const getMe = async (): Promise<UserProfile> => {
  const response = await api.get<UserProfile>('/auth/me');
  return response.data;
};

export const logout = () => {
  localStorage.removeItem('access_token');
  localStorage.removeItem('user_data');
  window.location.reload(); 
};