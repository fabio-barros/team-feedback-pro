import api from './api';
import type { CreateSprintRequest, DashboardData } from '../types';

// Mock ou chamada real
export const createSprint = async (config: CreateSprintRequest): Promise<void> => {
  // await api.post('/sprints/configure', config);
  console.log("Configurando sprints:", config);
  return Promise.resolve();
};

export const getDashboardData = async (): Promise<DashboardData[]> => {
  // await api.get('/dashboard/emotions');
  
  // MOCK 
  return Promise.resolve([
    {
      sprintId: '1',
      sprintName: 'Sprint 23',
      totalFeedbacks: 15,
      emotions: { feliz: 8, pensativo: 3, surpreso: 2, triste: 1, raiva: 1 }
    },
    {
      sprintId: '2',
      sprintName: 'Sprint 24 (Atual)',
      totalFeedbacks: 10,
      emotions: { feliz: 4, pensativo: 2, surpreso: 0, triste: 3, raiva: 1 }
    }
  ]);
};