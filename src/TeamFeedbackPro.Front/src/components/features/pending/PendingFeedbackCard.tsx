import { useState } from 'react';
import type { FeedbackResult} from '../../../types';
import { getEmojiForFeeling } from "../../../utils/feedbackUtils";
import './css/PendingFeedbackCard.css';

type FeedbackCardProps = {
  feedback: FeedbackResult;
  onApproveRequest: (item: FeedbackResult) => void;
  onRejectRequest: (item: FeedbackResult) => void;
};

export const FeedbackPendingCard = ({ feedback, onApproveRequest, onRejectRequest }: FeedbackCardProps) => {
  const [status] = useState("Em análise");

  const dataFormatada = new Date(feedback.createdAt).toLocaleDateString('pt-BR');

  const feelingText = feedback.feeling;
  const emoji = getEmojiForFeeling(feelingText);

  const getStatusClass = () => {
    if (status === "Aprovado") return "status--aprovado";
    if (status === "Rejeitado") return "status--rejeitado";
    return "status--pendente";
  };

  return (
    <article className="card">
      <header className="card-header">
        <div>
          <h3 className="card-titulo">
            {feedback.isAnonymous ? "Autor: Anônimo" : `Autor: ${feedback.authorName}`}
          </h3>
          <p className="card-subtitulo">Para: {feedback.recipientName}</p>
          <div style={{ display: 'flex', gap: '8px', alignItems: 'center', marginTop: '4px' }}>
          <span className="card-date">{dataFormatada}</span>
          {/* 3. Badge de Emoção */}
            {feelingText && (
              <span className="feeling-badge" title={feelingText}>
                {emoji} {feelingText}
              </span>
            )}
        </div>
        </div>
        

        <span className={`status-badge ${getStatusClass()}`}>
          {status}
        </span>
      </header>

      <div className="card-tags">
        <span className="tag tag-type">{feedback.type}</span>
        <span className="tag tag-category">{feedback.category}</span>
      </div>

      <p className="card-mensagem">"{feedback.content}"</p>

      {status === "Em análise" && (
        <footer className="card-actions">
          <button 
            className="btn-approve"
            onClick={() => onApproveRequest(feedback)}
          >
            Aprovar
          </button>

          <button 
            className="btn-reject"
            onClick={() => onRejectRequest(feedback)}
          >
            Rejeitar
          </button>
        </footer>
      )}
    </article>
  );
};