import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { login } from '../services/authService';
import './css/AuthPages.css'; 

const loginSchema = z.object({
  email: z.string().email('Email inválido'),
  password: z.string().min(1, 'Senha é obrigatória'),
});

type LoginFormInputs = z.infer<typeof loginSchema>;

type LoginPageProps = {
  onLoginSuccess: () => void;
  onGoToRegister: () => void;
};

export const LoginPage = ({ onLoginSuccess, onGoToRegister }: LoginPageProps) => {
  const [errorMsg, setErrorMsg] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginFormInputs>({
    resolver: zodResolver(loginSchema)
  });

  const onSubmit = async (data: LoginFormInputs) => {
    try {
      setErrorMsg('');
      await login(data);
      onLoginSuccess(); 
    } catch (error: any) {
      console.error(error);
      if (error.response?.status === 401) {
        setErrorMsg('Email ou senha incorretos.');
      } else {
        setErrorMsg('Erro ao conectar com o servidor.');
      }
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1>Entrar</h1>
        <p className="auth-subtitle">Bem-vindo de volta ao TeamFeedbackPro</p>

        {errorMsg && <div className="auth-error">{errorMsg}</div>}

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="form-group">
            <label>Email</label>
            <input type="email" {...register('email')} placeholder="seu@email.com" />
            {errors.email && <span className="erro">{errors.email.message}</span>}
          </div>

          <div className="form-group">
            <label>Senha</label>
            <input type="password" {...register('password')} placeholder="******" />
            {errors.password && <span className="erro">{errors.password.message}</span>}
          </div>

          <button type="submit" className="btn-primary full-width" disabled={isSubmitting}>
            {isSubmitting ? 'Entrando...' : 'Acessar Painel'}
          </button>
        </form>

        <div className="auth-footer">
          Não tem conta? <button className="link-btn" onClick={onGoToRegister}>Cadastre-se</button>
        </div>
      </div>
    </div>
  );
};