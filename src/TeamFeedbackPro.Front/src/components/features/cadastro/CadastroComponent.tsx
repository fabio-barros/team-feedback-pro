import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { registerUser } from '../../../services/authService';
import { getAllTeams } from '../../../services/teamService';
import type { RegisterRequest, TeamResult } from '../../../types';
import './css/CadastroComponent.css';

const registerSchema = z.object({
  nome: z.string().min(3, 'Nome deve ter no mínimo 3 letras'),
  email: z.string().email('Email inválido'),
  cargo: z.enum(['1', '2', '3']),
  time: z.string().optional(),
  senha: z.string()
    .min(8, 'Mínimo de 8 caracteres')
    .regex(/[A-Z]/, 'Precisa de 1 letra maiúscula')
    .regex(/[a-z]/, 'Precisa de 1 letra minúscula')
    .regex(/[0-9]/, 'Precisa de 1 número'),
  repetirSenha: z.string()
}).refine((data) => data.senha === data.repetirSenha, {
  message: "As senhas não conferem",
  path: ["repetirSenha"]
});

type FormData = z.infer<typeof registerSchema>;

export const CadastroComponent = () => {
  const [teams, setTeams] = useState<TeamResult[]>([]);
  const [serverError, setServerError] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(registerSchema)
  });

  useEffect(() => {
    getAllTeams().then(setTeams).catch(console.warn);
  }, []);

  const onSubmit = async (data: FormData) => {
    try {
      setServerError('');

      const payload: RegisterRequest = {
        name: data.nome,
        email: data.email,
        password: data.senha,
        role: parseInt(data.cargo), 
        teamId: data.time || null
      };

      await registerUser(payload);
      alert('Cadastro realizado com sucesso!');
    } catch (error: any) {
      console.error(error);
      setServerError(error.response?.data?.message || 'Erro ao conectar com o servidor');
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      {serverError && <div className="erro">{serverError}</div>}

      <div className="form-row">
        <label htmlFor="nome">Nome</label>
        <input id="nome" {...register('nome')} />
        {errors.nome && <span className="erro">{errors.nome.message}</span>}
      </div>

      <div className="form-row">
        <label htmlFor="email">Email</label>
        <input id="email" type="email" {...register('email')} />
        {errors.email && <span className="erro">{errors.email.message}</span>}
      </div>

      <div className="form-row">
        <label htmlFor="cargo">Cargo</label>
        <select id="cargo" {...register('cargo')}>
          <option value="1">Member</option>
          <option value="2">Manager</option>
          <option value="3">Admin</option>
        </select>
        {errors.cargo && <span className="erro">{errors.cargo.message}</span>}
      </div>

      <div className="form-row">
        <label htmlFor="time">Time</label>
        <select id="time" {...register('time')}>
          <option value="">Sem time</option>
          {teams.map(team => (
            <option key={team.id} value={team.id}>{team.name}</option>
          ))}
        </select>
      </div>

      <div className="form-row">
        <label htmlFor="senha">Senha</label>
        <input id="senha" type="password" {...register('senha')} />
        {errors.senha && <span className="erro">{errors.senha.message}</span>}
      </div>

      <div className="form-row">
        <label htmlFor="repetirSenha">Repetir senha</label>
        <input id="repetirSenha" type="password" {...register('repetirSenha')} />
        {errors.repetirSenha && <span className="erro">{errors.repetirSenha.message}</span>}
      </div>

      <div className="form-actions">
        <button type="submit" disabled={isSubmitting}>Enviar</button>
      </div>
    </form>
  );
};
