import { useState, useEffect } from 'react';
import { Nav } from '../components/ui/Nav';
import { Button } from '../components/ui/Button'; 
import { FeedbackCreateModal } from '../components/features/feedback/FeedbackCreateModal';
import { FeedbackList } from '../components/features/feedback/FeedbackList';
import { Spinner } from '../components/ui/Spinner';
import { 
  getReceivedFeedbacks, 
  getSentFeedbacks
} from '../services/feedbackService';
import type { PaginatedResult, FeedbackResult, UserProfile } from '../types';
import { CadastroComponent } from '../components/features/cadastro/CadastroComponent';
import { FeedbackPendingPage } from '../components/features/pending/PendingFeedbackPage';
import { SprintConfigPage } from '../components/features/admin/SprintConfigPage';
import { DashboardPage } from '../components/features/dashboard/DashboardPage';

import { getMe } from '../services/authService';
import './css/HomePage.css';

type ViewState = 'home' | 'recebidos' | 'enviados' | 'novo-usuario'| 'pendentes'| 'sprints' | 'dashboard';
type HomePageProps = {
  onLogout: () => void; 
};

export const HomePage = ({ onLogout }: HomePageProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [view, setView] = useState<ViewState>('home');

  const [feedbacks, setFeedbacks] = useState<FeedbackResult[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  const [role, setRole] = useState<number | null>(null); 
  const [user, setUser] = useState<UserProfile | null>(null);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const data = await getMe(); 
        setUser(data);

        // data.role já é number? Se vier string, converta:
        setRole(typeof data.role === 'string' ? Number(data.role) : data.role);
      } catch (err) {
        console.error("Erro ao buscar dados do usuário:", err);
      }
    };

    fetchUser();
  }, []);

  useEffect(() => {

    const viewsSemFeedback = ['home', 'pendentes', 'novo-usuario', 'sprints', 'dashboard'];
    if (viewsSemFeedback.includes(view)) {
      setFeedbacks([]);
      return;
    }

    const carregarFeedbacks = async () => {
      setIsLoading(true);
      setError(null);
      try {
        let dados: PaginatedResult<FeedbackResult>;
        if (view === 'recebidos') {
          dados = await getReceivedFeedbacks();
        } else {
          dados = await getSentFeedbacks();
        }
        setFeedbacks(dados.items);
      } catch (err) {
        setError(err as Error);
      } finally {
        setIsLoading(false);
      }
    };

    carregarFeedbacks();
  }, [view]);

  const renderView = () => {

    if (!user && (view === 'sprints' || view === 'dashboard')) {
  return <Spinner />;
}

    const managerViews = ['pendentes', 'novo-usuario', 'sprints', 'dashboard'];
    if (role === null && (managerViews.includes(view))) {
      return <Spinner />;
    }

    // Permissão apenas para managers (role === 1)
    if (managerViews.includes(view) && role !== 1) {
      return <p>Você não tem permissão para acessar esta página.</p>;
    }

    if (view === 'home') {
      return (
        <div className="content-welcome">
          <h1>Boas vindas ao Feedback Team{user ? `, ${user.name}` : ''}!</h1>
          <p>Envie ou veja seus feedbacks.</p>
          <Button onClick={() => setIsModalOpen(true)}>
            + Criar Novo Feedback
          </Button>
        </div>
      );
    }

    if (isLoading) return <Spinner />;
    if (error) return <p>Ocorreu um erro ao buscar os feedbacks.</p>;

    return (
      <div className="content-list-wrapper">
        <h2>
          {view === 'recebidos' && 'Feedbacks Recebidos'}
          {view === 'enviados' && 'Feedbacks Enviados'}
          {view === 'pendentes' && 'Feedbacks Pendentes'}
          {view === 'novo-usuario' && 'Cadastro de Usuário'}
          {view === 'sprints' && 'Configuração de Sprints'}
          {view === 'dashboard' && 'Dashboard de Clima'}
        </h2>

        {view === 'novo-usuario' && role === 1 && <CadastroComponent />}
        {view === 'pendentes' && role === 1 && <FeedbackPendingPage />}

        {view === 'sprints' && <SprintConfigPage user={user}/>}
        {view === 'dashboard' && <DashboardPage user={user} />}
        {(view === 'recebidos' || view === 'enviados') && (
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
        role={role} 
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
