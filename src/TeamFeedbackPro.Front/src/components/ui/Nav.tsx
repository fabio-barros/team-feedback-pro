import React from 'react';
import './css/Nav.css'; 

type navProps = {
  currentView: string; 
  onViewChange: (view: string) => void;
};

export const Nav = ({ currentView, onViewChange }: navProps) => {
  
  // Helper para checar se o botão está ativo
  const isActive = (view: string) => view === currentView;

  return (
    <nav className="nav">
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
      </ul>
    </nav>
  );
};