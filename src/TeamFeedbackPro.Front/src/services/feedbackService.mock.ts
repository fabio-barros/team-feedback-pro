import { 
  FeedbackType, 
  FeedbackCategory, 
  type FeedbackResult, 
  type PaginatedResult, 
  type CreateFeedbackRequest,
  type TeamMemberResult,
  type FeedbackStatus,
  type FeedbackFormDataResult
} from '../types';

// --- DADOS MOCKADOS (FAKE DB) ---

const CURRENT_USER_ID = 'user-logado-123';

const MOCK_USERS: TeamMemberResult[] = [
  { id: 'user-456', name: 'Aline Gerente', email: 'aline@empresa.com', role: 'Manager' },
  { id: 'user-789', name: 'Gabriel R Colega', email: 'gabrielr@empresa.com', role: 'Member' },
  { id: 'user-999', name: 'Fabio Tech Lead', email: 'fabio@empresa.com', role: 'Admin' },
];

let FAKE_FEEDBACKS: FeedbackResult[] = [
  {
    id: 'fb-1',
    authorId: 'user-456',
    authorName: 'Aline Gerente',
    recipientId: CURRENT_USER_ID,
    recipientName: 'Você',
    content: 'Excelente evolução técnica no último projeto, parabéns pela dedicação!',
    status: 'Approved',
    createdAt: new Date('2023-11-20').toISOString(),
    // @ts-ignore 
    type: FeedbackType.Praise, 
    // @ts-ignore
    category: FeedbackCategory.Technical 
  },
  {
    id: 'fb-2',
    authorId: CURRENT_USER_ID,
    authorName: 'Você',
    recipientId: 'user-789',
    recipientName: 'Gabriel R Colega',
    content: 'Gabriel, precisamos alinhar melhor a comunicação nas dailies.',
    status: 'Pending', 
    createdAt: new Date('2023-11-21').toISOString(),
    // @ts-ignore
    type: FeedbackType.Guidance,
    // @ts-ignore
    category: FeedbackCategory.SoftSkill
  }
];

const delay = (ms = 500) => new Promise(resolve => setTimeout(resolve, ms));

// --- FUNÇÕES DO SERVICE ---

export const getTeamMembers = async (): Promise<TeamMemberResult[]> => {
  await delay();
  return MOCK_USERS;
};

export const createFeedback = async (data: CreateFeedbackRequest): Promise<void> => {
  console.log("[MOCK] Criando feedback:", data);
  await delay(1000);

  
  const recipient = MOCK_USERS.find(u => u.id === data.recipientId);


  const newFeedback: FeedbackResult = {
    id: Math.random().toString(36).substr(2, 9),
    authorId: CURRENT_USER_ID,
    authorName: data.isAnonymous ? undefined : 'Você',
    recipientId: data.recipientId,
    recipientName: recipient ? recipient.name : 'Desconhecido',
    content: data.content,
    status: 'Pending', 
    createdAt: new Date().toISOString(),
    // @ts-ignore
    type: data.type as unknown as any, 
    category: data.category as unknown as any
  };

  FAKE_FEEDBACKS.unshift(newFeedback); 
};

export const getSentFeedbacks = async (page = 1, pageSize = 10): Promise<PaginatedResult<FeedbackResult>> => {
  await delay();

  const sent = FAKE_FEEDBACKS.filter(f => f.authorId === CURRENT_USER_ID);

  return {
    items: sent,
    pageIndex: page,
    totalPages: 1,
    totalCount: sent.length,
    hasPreviousPage: false,
    hasNextPage: false
  };
};


export const getFeedbacksRecebidos = async (userId: string): Promise<FeedbackResult[]> => {
  await delay();

  
  const received = FAKE_FEEDBACKS.filter(f => 
    f.recipientId === userId && f.status !== 'Pending'
  );

  return received;
};

export const getFeedbacksEnviados = async (userId: string): Promise<FeedbackResult[]> => {
    const result = await getSentFeedbacks();
    return result.items;
};

export const getFeedbackFormData = async (): Promise<FeedbackFormDataResult> => {
  await delay();
  return {
    // Retornamos os mesmos usuários mockados, mas dentro da estrutura nova
    recipients: MOCK_USERS 
  };
};