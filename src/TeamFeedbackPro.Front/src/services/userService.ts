import api from './api';
import type { UserInfo } from '../types'; 


export const getUserById = async (id: string): Promise<UserInfo> => {
  const response = await api.get<UserInfo>(`/users/${id}`);
  return response.data;
};