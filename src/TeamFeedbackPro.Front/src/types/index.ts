
export type UserInfo = {
  id: number;
  nome: string;
  cargo: string;
};


export type FeedbackStatus = 'pendente' | 'aprovado' | 'rejeitado';

export type Feedback = {
  id: string;
  mensagem: string;
  dataEnvio: string; 
  author: UserInfo;
  target: UserInfo;
  status: FeedbackStatus; 
};