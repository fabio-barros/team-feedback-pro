import React, { useEffect, useState } from 'react';
import { type FeedbackResult, type FeedbackStatus } from '../../../types';
import './css/FeedbackCard.css';
import { getUserById } from '../../../services/userService';

type FeedbackCardProps = {
  feedback: FeedbackResult;
  perspectiva: 'target' | 'author';
};

const getStatusClass = (status: FeedbackStatus) => {
  if (status === 'Aprovado') return 'status--aprovado';
  if (status === 'Rejeitado') return 'status--rejeitado';
  return 'status--pendente';
};

export const FeedbackCard = ({ feedback, perspectiva }: FeedbackCardProps) => {

  const [nomeExibicao, setNomeExibicao] = useState<string>('Carregando...');

  useEffect(() => {
    const carregarNome = async () => {
      try {
        let idParaBuscar = '';
        let ehAnonimo = false;

        if (perspectiva === 'author') {
          idParaBuscar = feedback.authorId;
          ehAnonimo = feedback.isAnonymous;
        } else {
          idParaBuscar = feedback.recipientId;
        }

        if (perspectiva === 'author' && ehAnonimo) {
          setNomeExibicao('Anônimo');
          return;
        }

        const nomeNoObjeto = perspectiva === 'target' ? feedback.authorName : feedback.recipientName;

        if (nomeNoObjeto) {
          setNomeExibicao(nomeNoObjeto);
        } else if (idParaBuscar) {
          const user = await getUserById(idParaBuscar);
          setNomeExibicao(user.name); 
        } else {
          setNomeExibicao('Usuário Desconhecido');
        }
      } catch (error) {
        console.error("Erro ao buscar nome do usuário:", error);
        setNomeExibicao('Erro ao carregar nome');
      }
    };
    carregarNome();
  }, [feedback, perspectiva]);

  const titulo = perspectiva === 'author' ? `De: ${nomeExibicao}` : `Para: ${nomeExibicao}`;
  const dataFormatada = new Date(feedback.createdAt).toLocaleDateString('pt-BR');

  return (
    <article className="card">
      <header className="card-header">
        <div>
          <h3 className="card-titulo">{titulo}</h3>
          <span className="card-date">{dataFormatada}</span>
        </div>

        {/* O Status Badge */}
        <span className={`status-badge ${getStatusClass(feedback.status)}`}>
          {feedback.status}
        </span>
      </header>

      <div className="card-tags">
        <span className="tag tag-type">{feedback.type}</span>
        <span className="tag tag-category">{feedback.category}</span>
      </div>
      <p className="card-mensagem">
        "{feedback.content}"
      </p>

    </article>
  );
};