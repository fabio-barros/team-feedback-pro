import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { registerUser } from '../services/authService';
import { getAllTeams } from '../services/teamService';
import type { RegisterRequest, TeamResult } from '../types';
import './css/AuthPages.css';

const registerSchema = z.object({
  name: z.string().min(3, 'Nome deve ter no mínimo 3 letras'),
  email: z.string().email('Email inválido'),
  role: z.enum(['Member', 'Manager', 'Admin']),
  teamId: z.string().optional(), 
  password: z.string()
    .min(8, 'Mínimo de 8 caracteres')
    .regex(/[A-Z]/, 'Precisa de 1 letra maiúscula')
    .regex(/[a-z]/, 'Precisa de 1 letra minúscula')
    .regex(/[0-9]/, 'Precisa de 1 número'),
  confirmPassword: z.string()
}).refine((data) => data.password === data.confirmPassword, {
  message: "As senhas não conferem",
  path: ["confirmPassword"],
});

type RegisterFormInputs = z.infer<typeof registerSchema>;

type RegisterPageProps = {
  onRegisterSuccess: () => void;
  onGoToLogin: () => void;
};

const ROLE_MAP: Record<string, number> = {
  'Member': 0,
  'Manager': 1,
  'Admin': 2
};

export const RegisterPage = ({ onRegisterSuccess, onGoToLogin }: RegisterPageProps) => {
  const [teams, setTeams] = useState<TeamResult[]>([]);
  const [serverError, setServerError] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RegisterFormInputs>({
    resolver: zodResolver(registerSchema),
    defaultValues: { role: 'Member', teamId: '' }
  });

  
  useEffect(() => {
    getAllTeams()
      .then(setTeams)
      .catch(err => console.warn('Não foi possível carregar times (talvez precise estar logado? Verifique regra da API)', err));

  }, []);

  const onSubmit = async (data: RegisterFormInputs) => {
    try {
      setServerError('');

      const payload: RegisterRequest = {
        name: data.name,
        email: data.email,
        password: data.password,
        role: ROLE_MAP[data.role],
        teamId: (!data.teamId || data.teamId === '') ? null : data.teamId
      };

      await registerUser(payload);
      alert('Cadastro realizado com sucesso! Faça login.');
      onRegisterSuccess();
    } catch (error: any) {
      console.error("Erro no cadastro:", error);

      
      if (error.response?.data) {
        const data = error.response.data;
        
        // Erros de Validação (FluentValidation) -> Retorna { errors: { Email: [...] } }
        if (data.errors) {
            const mensagens = Object.values(data.errors).flat().join(' | ');
            setServerError(mensagens);
        } 
        // Erros de Domínio (BadRequest) -> Retorna { message: "Email já existe" }
        else if (data.message) {
            setServerError(data.message);
        } 
        else {
            setServerError('Erro desconhecido no servidor (400).');
        }
      } else {
        setServerError('Erro ao conectar. Verifique se a API está rodando.');
      }
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card wide">
        <h1>Crie sua conta</h1>
        <p className="auth-subtitle">Junte-se ao time</p>

        {serverError && <div className="auth-error">{serverError}</div>}

        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="form-group">
            <label>Nome Completo</label>
            <input {...register('name')} placeholder="Seu nome" />
            {errors.name && <span className="erro">{errors.name.message}</span>}
          </div>

          <div className="form-group">
            <label>Email</label>
            <input type="email" {...register('email')} placeholder="email@empresa.com" />
            {errors.email && <span className="erro">{errors.email.message}</span>}
          </div>

          <div className="row-2-col">
            <div className="form-group">
              <label>Cargo</label>
              <select {...register('role')}>
                <option value="Member">Membro</option>
                <option value="Manager">Gerente</option>
                <option value="Admin">Admin</option>
              </select>
              {errors.role && <span className="erro">{errors.role.message}</span>}
            </div>

            <div className="form-group">
              <label>Time (Opcional)</label>
              <select {...register('teamId')}>
                <option value="">Sem time / Selecione...</option>
                {teams.map(team => (
                  <option key={team.id} value={team.id}>{team.name}</option>
                ))}
              </select>
            </div>
          </div>

          <div className="row-2-col">
            <div className="form-group">
              <label>Senha</label>
              <input type="password" {...register('password')} placeholder="8+ chars, Maiusc, Minusc, Núm" />
              {errors.password && <span className="erro">{errors.password.message}</span>}
            </div>

            <div className="form-group">
              <label>Confirmar Senha</label>
              <input type="password" {...register('confirmPassword')} placeholder="Repita a senha" />
              {errors.confirmPassword && <span className="erro">{errors.confirmPassword.message}</span>}
            </div>
          </div>

          <button type="submit" className="btn-primary full-width" disabled={isSubmitting}>
            {isSubmitting ? 'Criando conta...' : 'Cadastrar'}
          </button>
        </form>

        <div className="auth-footer">
          Já tem conta? <button className="link-btn" onClick={onGoToLogin}>Fazer Login</button>
        </div>
      </div>
    </div>
  );
};