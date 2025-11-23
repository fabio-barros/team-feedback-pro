import api from './api';
import type { CreateTeamRequest, TeamResult, UpdateTeamRequest } from '../types';

export const getAllTeams = async (): Promise<TeamResult[]> => {
  const response = await api.get<TeamResult[]>('/teams');
  return response.data;
};

export const getTeamById = async (id: string): Promise<TeamResult> => {
  const response = await api.get<TeamResult>(`/teams/${id}`);
  return response.data;
};

export const createTeam = async (data: CreateTeamRequest): Promise<TeamResult> => {
  const response = await api.post<TeamResult>('/teams', data);
  return response.data;
};

export const updateTeam = async (id: string, data: UpdateTeamRequest): Promise<TeamResult> => {
  const response = await api.put<TeamResult>(`/teams/${id}`, data);
  return response.data;
};

export const deleteTeam = async (id: string): Promise<void> => {
  await api.delete(`/teams/${id}`);
};