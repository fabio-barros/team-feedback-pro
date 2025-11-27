// --- AUTH & USER ---

export interface LoginRequest {
  email: string;
  password: string; 
}

export interface AuthenticationResult {
  token: string;
  user: {
    id: string;
    name: string;
    email: string;
    role: string;
  };
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  role: number;
  teamId?: string | null; 
}

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  role: string;
  teamId?: string;
}

// --- TEAM ---

export interface TeamResult {
  id: string;
  name: string;
  managerId?: string;
  members?: TeamMemberResult[];
}

export interface CreateTeamRequest {
  name: string;
  managerId?: string | null;
}

export interface UpdateTeamRequest {
  name: string;
  managerId?: string | null;
}

// --- FEEDBACK ---

export  const FeedbackType = {
  Feedback: 'Feedback',
  Praise: 'Praise',     
  Guidance: 'Guidance',
  Constructive: 'Constructive', 
} as const

export type FeedbackType = typeof FeedbackType[keyof typeof FeedbackType];

export  const  FeedbackCategory ={
  Technical: 'Technical',
  SoftSkill: 'SoftSkill',
  Management: 'Management',
  CodeQuality: 'CodeQuality',
} as const

export type FeedbackCategory = typeof FeedbackCategory[keyof typeof FeedbackCategory];


export type FeedbackStatus = 'Em análise' | 'Aprovado' | 'Rejeitado';

export type UserInfo = {
  id: string;
  name: string;
  cargo: string;
};

export interface TeamMemberResult {
  id: string; 
  name: string;
  email: string;
  role: string;
}

export interface CreateFeedbackRequest {
  recipientId: string;
  type: FeedbackType | string;
  category: FeedbackCategory | string;
  content: string;
  isAnonymous: boolean;
}

export interface FeedbackResult {
  id: string;
  authorId: string;
  authorName?: string; 
  recipientId: string;
  recipientName?: string;
  content: string;
  status: FeedbackStatus;
  createdAt: string;
  isAnonymous: boolean;
  type: FeedbackType | string;       
  category: FeedbackCategory | string;
}

export interface PaginatedResult<T> {
  items: T[];
  pageIndex: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface LookupItem {
  key: number; 
  value: string; 
}

export interface FeedbackFormDataResult {
  users: TeamMemberResult[];
  types: LookupItem[];
  categories: LookupItem[];
}