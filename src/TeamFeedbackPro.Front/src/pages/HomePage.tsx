import { useState, useEffect } from 'react';
import { Nav } from '../components/ui/Nav';
import { Button } from '../components/ui/Button'; 
import { FeedbackCreateModal } from '../components/features/feedback/FeedbackCreateModal';
import { FeedbackList } from '../components/features/feedback/FeedbackList';
import { Spinner } from '../components/ui/Spinner'; 

import { getFeedbacksRecebidos } from '../services/feedbackService.mock';
import { getFeedbacksEnviados } from '../services/feedbackService.mock';
import type { Feedback } from '../types';

import './css/HomePage.css';

type ViewState = 'home' | 'recebidos' | 'enviados';


export const HomePage = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [view, setView] = useState<ViewState>('home');
  const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
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
        const userId = 3;
        let dados: Feedback[] = [];
        
        if (view === 'recebidos') {
          dados = await getFeedbacksRecebidos(userId);
          
          dados = dados.filter(fb => fb.status !== 'pendente');
          
        } else if (view === 'enviados') {
          dados = await getFeedbacksEnviados(userId);
        }
        
        setFeedbacks(dados);
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
        <h2>{view === 'recebidos' ? 'Feedbacks Recebidos' : 'Feedbacks Enviados'}</h2>
        
        <div className="scrollable-list-area">
          <FeedbackList 
            feedbacks={feedbacks}
            perspectiva={view === 'enviados' ? 'target' : 'author'}
          />
        </div>
      </div>
    );
  };

  return (
    <div className="homepage-layout">
      <Nav 
        currentView={view} 
        onViewChange={(newView) => setView(newView as ViewState)} 
      />
      
      <main className="painel-content">
        {renderView()}
      </main>

      <FeedbackCreateModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onFeedbackEnviado={() => setIsModalOpen(false)}
      />
    </div>
  );
};