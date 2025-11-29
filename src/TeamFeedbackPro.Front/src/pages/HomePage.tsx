import { useState, useEffect } from 'react';
import { Nav } from '../components/ui/Nav';
import { Button } from '../components/ui/Button'; 
import { FeedbackCreateModal } from '../components/features/feedback/FeedbackCreateModal';
import { FeedbackList } from '../components/features/feedback/FeedbackList';
import { Spinner } from '../components/ui/Spinner'; 

import { getReceivedFeedbacks, getSentFeedbacks } from '../services/feedbackService';
import type { PaginatedResult, FeedbackResult } from '../types';
import { CadastroComponent } from '../components/features/cadastro/CadastroComponent';

import './css/HomePage.css';

type ViewState = 'home' | 'recebidos' | 'enviados' | 'novo-usuario';
type HomePageProps = {
  onLogout: () => void; 
};

export const HomePage = ({ onLogout }: HomePageProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [view, setView] = useState<ViewState>('home');

  const [feedbacks, setFeedbacks] = useState<FeedbackResult[]>([]);

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    if (view === 'home') {
      setFeedbacks([]); 
      return; 
    }

    const carregarFeedbacks = async () => {
      setIsLoading(true);
      setError(null);
      
      try {

        
        let dados:PaginatedResult<FeedbackResult>;
        
        if (view === 'recebidos') {
           dados = await getReceivedFeedbacks();
           setFeedbacks(dados.items);
        } else {
           dados = await getSentFeedbacks();
           setFeedbacks(dados.items);
        }
        
        
      } catch (err) {
        setError(err as Error);
      } finally {
        setIsLoading(false);
      }
    };

    carregarFeedbacks();
  }, [view]); 

  const renderView = () => {
    if (view === 'home') {
      return (
        <div className="content-welcome">
          <h1>Boas vindas ao Feedback Team!</h1>
          <p>Envie ou veja seus feedbacks.</p>
          <Button onClick={() => setIsModalOpen(true)}>
            + Criar Novo Feedback
          </Button>
        </div>
      );
    }

    if (isLoading) {
      return <Spinner />;
    }

    if (error) {
      return <p>Ocorreu um erro ao buscar os feedbacks.</p>;
    }

    return (
      <div className="content-list-wrapper">
        <h2>{view === 'recebidos' ? 'Feedbacks Recebidos' : view === 'enviados' ? 'Feedbacks Enviados' : 'Cadastro de Novo Usuário'}</h2>
        
        {view === 'novo-usuario' ? (
          <CadastroComponent></CadastroComponent>
        ) : (
          <FeedbackList
            feedbacks={feedbacks}
            perspectiva={view === 'enviados' ? 'target' : 'author'}
          />
      )}
      </div>
    );
  };

  return (
    <div className="homepage-layout">
      <Nav 
        currentView={view} 
        onViewChange={(newView) => setView(newView as ViewState)} 
        onLogout={onLogout}
      />
      
      <main className="painel-content">
        {renderView()}
      </main>

      <FeedbackCreateModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onFeedbackEnviado={() => {
          setIsModalOpen(false);
        const current = view;
            setView('home');
            setTimeout(() => setView(current), 10);
        }}
      />
    </div>
  );
};