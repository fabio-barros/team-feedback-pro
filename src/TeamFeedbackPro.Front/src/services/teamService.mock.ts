
import type { TeamResult } from '../types';

const delay = (ms = 500) => new Promise(resolve => setTimeout(resolve, ms));

const FAKE_TEAMS: TeamResult[] = [
  { id: 'team-001', name: 'Desenvolvimento' },
  { id: 'team-002', name: 'Recursos Humanos' },
  { id: 'team-003', name: 'Vendas' },
  { id: 'team-004', name: 'Infraestrutura' },
];

export const getAllTeams = async (): Promise<TeamResult[]> => {
  console.log("[MOCK TEAM] Buscando times...");
  await delay();
  return FAKE_TEAMS;
};

