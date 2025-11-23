import api from './api';
import type { CreateFeedbackRequest
, FeedbackResult
, PaginatedResult
, TeamMemberResult
, FeedbackFormDataResult
 } from '../types/'; 

export const getFeedbackFormData = async (): Promise<FeedbackFormDataResult[]> => {
  const response = await api.get<FeedbackFormDataResult[]>('/feedbacks/feedback-form-data');
  return response.data;
};

export const getTeamMembers = async (): Promise<TeamMemberResult[]> => {
  const response = await api.get<TeamMemberResult[]>('/users/team-members');
  return response.data;
};

export const createFeedback = async (data: CreateFeedbackRequest): Promise<void> => {
  await api.post('/feedbacks', data);
};

export const getSentFeedbacks = async (page = 1, pageSize = 10): Promise<PaginatedResult<FeedbackResult>> => {
  const response = await api.get<PaginatedResult<FeedbackResult>>('/feedbacks/sent', {
    params: { page, pageSize }
  });
  return response.data;
};

// export const getFeedbacksRecebidos = async (userId: number): Promise<Feedback[]> => {
//   try {
    
//     const response = await api.get(`/feedbacks/recebidos/${userId}`);
//     return response.data; 
//   } catch (error) {
//     console.error("Erro ao buscar feedbacks recebidos:", error);
//     throw error; 
//   }
// };

// export const getFeedbacksEnviados = async (userId: number): Promise<Feedback[]> => {
//   try {
//     const response = await api.get(`/feedbacks/enviados/${userId}`);
//     return response.data;
//   } catch (error) {
//     console.error("Erro ao buscar feedbacks enviados:", error);
//     throw error;
//   }
// };
