import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Button } from '../../ui/Button'
import { createSprint } from '../../../services/sprintService';
import {  getTeamById } from '../../../services/teamService'; 
import type {  CreateSprintRequest, UserProfile } from '../../../types';
import './css/SprintConfigPage.css';


const sprintSchema = z.object({
  name: z.string().min(3, 'Nome deve ter no mínimo 3 caracteres'),
  description: z.string().optional(),
  startDate: z.string().min(1, 'Data inicial obrigatória'),
  endDate: z.string().min(1, 'Data final obrigatória'),
}).refine((data) => new Date(data.endDate) > new Date(data.startDate), {
  message: "A data final deve ser posterior à data inicial",
  path: ["endDate"],
});

type SprintFormInputs = z.infer<typeof sprintSchema>;

type Props = {
  user: UserProfile | null;
};

export const SprintConfigPage = ({ user }: Props) => {
  const [teamName, setTeamName] = useState('Carregando time...');
  
  const { 
    register, 
    handleSubmit, 
    formState: { errors, isSubmitting },
    reset 
  } = useForm<SprintFormInputs>({
    resolver: zodResolver(sprintSchema)
  });


  useEffect(() => {
    if (user?.teamId) {
      getTeamById(user.teamId)
        .then(team => setTeamName(team.name))
        .catch(() => setTeamName('Equipe Desconhecida'));
    }
  }, [user]);

  const onSubmit = async (data: SprintFormInputs) => {
    if (!user?.teamId) {
      alert("Erro: Você não está vinculado a nenhum time.");
      return;
    }
    try {
      
      const payload: CreateSprintRequest = {
        name: data.name,
        description: data.description,
        startAt: new Date(data.startDate).toISOString(), 
        endAt: new Date(data.endDate).toISOString()
      };

      await createSprint(payload);
      
      alert('Sprint criada com sucesso!');
      reset();
    } catch (error) {
      console.error(error);
      alert(error.response.data.message);
    }
  };

  if (!user?.teamId) {
    return <div className="sprint-config-container"><p>Você precisa estar em um time para criar Sprints.</p></div>;
  }

  return (
    <div className="sprint-config-container">
      <h3>Nova Sprint - {teamName}</h3>
      <p className="description">Cadastre um novo ciclo de trabalho para um time.</p>
      
      <form onSubmit={handleSubmit(onSubmit)} className="sprint-form">
        
        {/* Nome */}
        <div className="form-group">
          <label>Nome da Sprint:</label>
          <input 
            type="text" 
            placeholder="Ex: Sprint 24 - Q3" 
            {...register('name')} 
          />
          {errors.name && <span className="erro">{errors.name.message}</span>}
        </div>

        {/* Datas (Lado a Lado) */}
        <div className="row-2-col">
          <div className="form-group">
            <label>Data Início:</label>
            <input type="date" {...register('startDate')} />
            {errors.startDate && <span className="erro">{errors.startDate.message}</span>}
          </div>

          <div className="form-group">
            <label>Data Fim:</label>
            <input type="date" {...register('endDate')} />
            {errors.endDate && <span className="erro">{errors.endDate.message}</span>}
          </div>
        </div>

        {/* Descrição (Opcional) */}
        <div className="form-group">
          <label>Descrição (Opcional):</label>
          <textarea 
            rows={3} 
            placeholder="Objetivos principais desta sprint..." 
            {...register('description')} 
          />
        </div>

        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Salvando...' : 'Criar Sprint'}
        </Button>
      </form>
    </div>
  );
};