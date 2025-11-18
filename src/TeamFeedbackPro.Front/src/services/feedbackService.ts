import api from './api';
import type { Feedback } from '../types/'; 


export const getFeedbacksRecebidos = async (userId: number): Promise<Feedback[]> => {
  try {
    
    const response = await api.get(`/feedbacks/recebidos/${userId}`);
    return response.data; 
  } catch (error) {
    console.error("Erro ao buscar feedbacks recebidos:", error);
    throw error; 
  }
};

export const getFeedbacksEnviados = async (userId: number): Promise<Feedback[]> => {
  try {
    const response = await api.get(`/feedbacks/enviados/${userId}`);
    return response.data;
  } catch (error) {
    console.error("Erro ao buscar feedbacks enviados:", error);
    throw error;
  }
};
