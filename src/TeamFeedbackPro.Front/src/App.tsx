import { useState, useEffect } from 'react';
import './App.css'
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { logout } from './services/authService';
import './index.css'

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [currentAuthPage, setCurrentAuthPage] = useState<'login' | 'register'>('login');

  useEffect(() => {
    const token = localStorage.getItem('access_token');
    if (token) {
      setIsAuthenticated(true);
    }
  }, []);

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    logout(); 
    setIsAuthenticated(false); 
    setCurrentAuthPage('login');
  };

  if (isAuthenticated) {

    return <HomePage onLogout={handleLogout}/>; 
  }

  if (currentAuthPage === 'register') {
    return (
      <RegisterPage 
        onRegisterSuccess={() => setCurrentAuthPage('login')}
        onGoToLogin={() => setCurrentAuthPage('login')}
      />
    );
  }

  return (
    <LoginPage 
      onLoginSuccess={handleLoginSuccess} 
      onGoToRegister={() => setCurrentAuthPage('register')}
    />
    
  )
}

export default App
