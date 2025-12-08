import api from './api';
import type { CreateFeedbackRequest
, FeedbackResult
, PaginatedResult
, TeamMemberResult
, FeedbackFormDataResult
 } from '../types/'; 

export const getFeedbackFormData = async (): Promise<FeedbackFormDataResult> => {
  const response = await api.get<FeedbackFormDataResult>('/feedbacks/feedback-form-data');
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

export const getReceivedFeedbacks = async (page = 1, pageSize = 10): Promise<PaginatedResult<FeedbackResult>> => {
  const response = await api.get<PaginatedResult<FeedbackResult>>('/feedbacks/received', {
    params: { page, pageSize }
  });
  return response.data;
};

export const getPendingFeedbacks = async (page = 1, pageSize = 10): Promise<PaginatedResult<FeedbackResult>> => {
  const response = await api.get<PaginatedResult<FeedbackResult>>('/feedbacks/review-peding', {
    params: { page, pageSize }
  });
  return response.data;
};

export const approveFeedback = async (feedbackId: string, review?: string): Promise<void> => {
  await api.patch('/feedbacks/approve', review ? { review } : {}, {
    params: { feedbackId } 
  });
};

export const rejectFeedback = async (feedbackId: string, review: string): Promise<void> => {
  await api.patch('/feedbacks/reject', { review }, { 
    params: { feedbackId } 
  });
};

