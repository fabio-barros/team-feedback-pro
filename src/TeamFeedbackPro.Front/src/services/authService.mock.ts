
import type { LoginRequest, AuthenticationResult, RegisterRequest } from '../types';

const delay = (ms = 1000) => new Promise(resolve => setTimeout(resolve, ms));

export const login = async (data: LoginRequest): Promise<AuthenticationResult> => {
  console.log(`[MOCK LOGIN] Tentando logar com: ${data.email}`);
  await delay();

  if (data.email === 'erro@teste.com') {
    throw { response: { status: 401 } };
  }

  const mockResponse: AuthenticationResult = {
    token: "fake-jwt-token-123456",
    user: {
      id: "user-guid-123",
      name: "Usuário Mock",
      email: data.email,
      role: "Member" 
    }
  };

  localStorage.setItem('access_token', mockResponse.token);
  localStorage.setItem('user_data', JSON.stringify(mockResponse.user));
  
  return mockResponse;
};

export const registerUser = async (data: RegisterRequest): Promise<void> => {
  console.log(`[MOCK REGISTER] Dados recebidos:`, data);
  await delay();

  if (data.email === 'existente@teste.com') {
    throw { 
      response: { 
        data: { errors: { Email: ["Este email já está em uso."] } } 
      } 
    };
  }

  console.log("[MOCK REGISTER] Usuário cadastrado com sucesso!");
};

export const logout = () => {
  localStorage.removeItem('access_token');
  localStorage.removeItem('user_data');
  window.location.reload();
};