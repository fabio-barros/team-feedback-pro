import React from 'react';
import type { Feedback, FeedbackStatus } from '../../../types';
import './css/FeedbackCard.css'; 

type FeedbackCardProps = {
  feedback: Feedback;
  perspectiva: 'target' | 'author'; 
};


const getStatusClass = (status: FeedbackStatus) => {
  if (status === 'aprovado') return 'status--aprovado';
  if (status === 'rejeitado') return 'status--rejeitado';
  return 'status--pendente'; 
};

export const FeedbackCard = ({ feedback, perspectiva }: FeedbackCardProps) => {
  
  const pessoaEmDestaque = perspectiva === 'author' 
    ? feedback.author 
    : feedback.target;
  
  const titulo = perspectiva === 'author'
    ? `De: Anônimo `
    : `Para: ${pessoaEmDestaque.nome} `;

  return (
    <article className="card">
      <header className="card-header">
        <div>
          <h3 className="card-titulo">{titulo}</h3>
          
        </div>
        
        {/* O Status Badge */}
        <span className={`status-badge ${getStatusClass(feedback.status)}`}>
          {feedback.status}
        </span>
      </header>
      
      <p className="card-mensagem">
        "{feedback.mensagem}"
      </p>

      <footer className="card-footer">
        {new Date(feedback.dataEnvio).toLocaleDateString()}
      </footer>
    </article>
  );
};