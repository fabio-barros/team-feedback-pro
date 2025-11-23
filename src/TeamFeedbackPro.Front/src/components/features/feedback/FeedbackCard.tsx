import React from 'react';
import  { FeedbackType,FeedbackCategory, type FeedbackResult, type FeedbackStatus } from '../../../types';
import './css/FeedbackCard.css'; 

type FeedbackCardProps = {
  feedback: FeedbackResult;
  perspectiva: 'target' | 'author'; 
};

const getStatusLabel = (status: FeedbackStatus) => {
  switch (status) {
    case 'Approved': return 'Aprovado';
    case 'Rejected': return 'Rejeitado';
    case 'Pending': return 'Pendente';
    default: return status;
  }
};

const getStatusClass = (status: FeedbackStatus) => {
  if (status === 'Approved') return 'status--aprovado';
  if (status === 'Rejected') return 'status--rejeitado';
  return 'status--pendente'; 
};

const TypeLabels: Record<number, string> = {
  [FeedbackType.Feedback]: 'Feedback',
  [FeedbackType.Praise]: 'Elogio',
  [FeedbackType.Guidance]: 'Orientação'
};

const CategoryLabels: Record<number, string> = {
  [FeedbackCategory.Technical]: 'Técnico',
  [FeedbackCategory.SoftSkill]: 'Comportamental',
  [FeedbackCategory.Management]: 'Gestão'
};

export const FeedbackCard = ({ feedback, perspectiva }: FeedbackCardProps) => {
  
  let titulo = '';
  let nomePessoa = '';

  if (perspectiva === 'author') {
    nomePessoa = feedback.authorName || 'Anônimo';
    titulo = `De: ${nomePessoa}`;
  } else {
    nomePessoa = feedback.recipientName;
    titulo = `Para: ${nomePessoa}`;
  }

  const dataFormatada = new Date(feedback.createdAt).toLocaleDateString('pt-BR');
  const tipoTexto = TypeLabels[FeedbackType as unknown as number] || 'Feedback';
  const categoriaTexto = CategoryLabels[FeedbackCategory as unknown as number] || 'Geral';

  

  return (
    <article className="card">
      <header className="card-header">
        <div>
          <h3 className="card-titulo">{titulo}</h3>
          <span className="card-date">{dataFormatada}</span>
        </div>
        
        {/* O Status Badge */}
        <span className={`status-badge ${getStatusClass(feedback.status)}`}>
          {getStatusLabel(feedback.status)}
        </span>
      </header>
      
      <div className="card-tags">
        <span className="tag tag-type">{tipoTexto}</span>
        <span className="tag tag-category">{categoriaTexto}</span>
      </div>
      <p className="card-mensagem">
        "{feedback.content}"
      </p>

    </article>
  );
};