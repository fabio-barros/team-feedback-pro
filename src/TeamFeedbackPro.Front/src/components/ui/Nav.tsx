import React from 'react';
import './css/Nav.css'; 

type navProps = {
  currentView: string; 
  onViewChange: (view: string) => void;
  onLogout: () => void;
  role: number | null;
};

export const Nav = ({ currentView, onViewChange, onLogout, role }: navProps) => {
  
  const isActive = (view: string) => view === currentView;

  return (
    <nav className="nav">
      <div className="nav-content">
      <ul className="nav-list">
        
        <li className="nav-item">
          <button 
            className={`nav-button ${isActive('home') ? 'active' : ''}`}
            onClick={() => onViewChange('home')}
          >
            Início
          </button>
        </li>

        <li className="nav-item">
          <button 
            className={`nav-button ${isActive('recebidos') ? 'active' : ''}`}
            onClick={() => onViewChange('recebidos')}
          >
            Reconhecimentos
          </button>
        </li>

  
        <li className="nav-item">
          <button 
            className={`nav-button ${isActive('recebidos') ? 'active' : ''}`}
            onClick={() => onViewChange('recebidos')}
          >
            Feedbacks Recebidos
          </button>
        </li>



        <li className="nav-item">
          <button 
            className={`nav-button ${isActive('enviados') ? 'active' : ''}`}
            onClick={() => onViewChange('enviados')}
          >
            Feedbacks Enviados
          </button>
        </li>
        {role === 1 && (
  <>
    <li className="nav-item">
      <button 
        className={`nav-button ${isActive('pendentes') ? 'active' : ''}`}
        onClick={() => onViewChange('pendentes')}
      >
        Feedbacks Pendentes
      </button>
    </li>
    <li className="nav-item">
      <button 
        className={`nav-button ${isActive('novo-usuario') ? 'active' : ''}`}
        onClick={() => onViewChange('novo-usuario')}
      >
        Novo Usuário
      </button>
    </li>
    <li className="nav-item">
      <button 
        className={`nav-button ${isActive('sprints') ? 'active' : ''}`}
        onClick={() => onViewChange('sprints')}
      >
        Sprints
      </button>
    </li>
    <li className="nav-item">
      <button 
        className={`nav-button ${isActive('dashboard') ? 'active' : ''}`}
        onClick={() => onViewChange('dashboard')}
      >
        Dashboard
      </button>
    </li>
  </>
)}
      </ul>
      <div className="nav-footer">
          <button className="nav-button logout-btn" onClick={onLogout}>
            🚪 Sair
          </button>
        </div>
        </div>
    </nav>
  );
};