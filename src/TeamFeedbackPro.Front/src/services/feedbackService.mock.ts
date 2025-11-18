
import type { Feedback } from '../types'; 

// --- Nosso "Banco de Dados Falso" ---

const FAKE_USER_FABIO = { id: 1, nome: 'Fábio Ribeiro', cargo: 'Dev Sênior' };
const FAKE_USER_ALINE = { id: 2, nome: 'Aline Limeira', cargo: 'Product Manager' };
const FAKE_USER_LOGADO = { id: 3, nome: 'Gabriel Reis', cargo: 'Dev Pleno' };

const FAKE_DB_RECEBIDOS: Feedback[] = [
  {
    id: 'fb-001',
    mensagem: 'Parabéns pelo ótimo trabalho no projeto X, sua organização foi chave!',
    dataEnvio: '2025-11-12T10:30:00Z',
    author: FAKE_USER_ALINE,
    target: FAKE_USER_LOGADO,
    status: 'aprovado', 
  },
  
];

const FAKE_DB_ENVIADOS: Feedback[] = [
  {
    id: 'fb-003',
    mensagem: 'Aline, obrigado pela ajuda no refinamento das tasks. Foi muito produtivo.',
    dataEnvio: '2025-11-09T09:15:00Z',
    author: FAKE_USER_LOGADO,
    target: FAKE_USER_ALINE,
    status: 'aprovado', 
  },
  {
    id: 'fb-002',
    mensagem: 'Gostei muito da sua apresentação na review, bem clara.',
    dataEnvio: '2025-11-10T14:00:00Z',
    author: FAKE_USER_LOGADO,
    target: FAKE_USER_FABIO,
    status: 'pendente', 
  },
];

// --- A Simulação da API ---

// Este é o "truque" para simular o tempo de espera da rede (latência)
// Assim você pode testar seus spinners de loading!
const simularLatencia = (ms: number = 1000) => {
  return new Promise(resolve => setTimeout(resolve, ms));
};

// --- Nossas Funções Mock (idênticas às reais) ---

export const getFeedbacksRecebidos = async (userId: number): Promise<Feedback[]> => {
  console.warn(`%c[MOCK ATIVO] getFeedbacksRecebidos(${userId})`, 'color: #FFA500;');
  
  await simularLatencia(); // Espera 1 segundo
  
  // A lógica do 'userId' aqui é só para mostrar, 
  // já que estamos retornando dados "chumbados"
  return FAKE_DB_RECEBIDOS;
};

export const getFeedbacksEnviados = async (userId: number): Promise<Feedback[]> => {
  console.warn(`%c[MOCK ATIVO] getFeedbacksEnviados(${userId})`, 'color: #FFA500;');
  
  await simularLatencia(500); // Meio segundo
  
  return FAKE_DB_ENVIADOS;
};