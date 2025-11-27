import { useEffect, useState } from 'react';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

import './css/FeedbackCreateModal.css';

import { Modal } from '../../ui/Modal';
import { createFeedback, getFeedbackFormData } from '../../../services/feedbackService';
import { FeedbackCategory, type TeamMemberResult, FeedbackType, type CreateFeedbackRequest, type FeedbackFormDataResult } from '../../../types';


const feedbackSchema = z.object({
  recipientId: z.string().min(1, 'Selecione um destinatário'),
  content: z.string().min(20, 'Mínimo de 20 caracteres').max(2000, 'Máximo de 2000'),
  type: z.coerce.number(),
  category: z.coerce.number(),
  isAnonymous: z.boolean()
});

type FeedbackFormInputs = z.infer<typeof feedbackSchema>;

type FeedbackCreateModalProps = {
  isOpen: boolean;
  onClose: () => void;
  onFeedbackEnviado?: () => void;
};

export const FeedbackCreateModal = ({ isOpen, onClose, onFeedbackEnviado }: FeedbackCreateModalProps) => {

  const [formData, setFormData] = useState<FeedbackFormDataResult>({
    users: [],
    types: [],
    categories: []
  });

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset
  } = useForm({
    resolver: zodResolver(feedbackSchema),
    defaultValues: {
      recipientId: '',
      content: '',
      type: 0,
      category: 0,
      isAnonymous: false,
    }
  });

  useEffect(() => {
    if (isOpen) {
      getFeedbackFormData()
        .then(data => {
          console.log("Dados do formulário carregados:", data);
          setFormData(data);
        })
        .catch(error => console.error('Erro ao carregar dados do formulário:', error));
    }
  }, [isOpen]);

  console.log("DADOS RECEBIDOS DO BACKEND:", getFeedbackFormData());

  const onSubmit: SubmitHandler<FeedbackFormInputs> = async (data) => {
    try {
      const payload: CreateFeedbackRequest = {
        ...data,
        type: data.type as any,
        category: data.category as any,
      };

      await createFeedback(payload);

      reset();
      onClose();
      if (onFeedbackEnviado) onFeedbackEnviado();
    } catch (error) {
      console.error('Erro ao enviar feedback', error)
      alert('Erro ao enviar feedback. Tente novamente.');
    }
  };


  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Novo Feedback">
      <form onSubmit={handleSubmit(onSubmit)}>


        <div className="form-group">
          <label>Para:</label>
          <select  {...register('recipientId')}>
            <option value="">Selecione...</option>
            {formData.users.map(user => (
              <option key={user.id} value={user.id}>
                {user.name} ({user.role})
              </option>
            ))}
          </select>
          {errors.recipientId && <span className="erro">{errors.recipientId.message}</span>}
        </div>

        <div style={{ display: 'flex', gap: '1rem' }}>
          <div className="form-group">
            <label>Tipo:</label>
            <select {...register('type')}>
              {formData.types.map(t => (
                <option key={t.key} value={t.key}>
                  {t.value} 
                </option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Categoria:</label>
            <select {...register('category')}>
              {formData.categories.map(c => (
                <option key={c.key} value={c.key}>
                  {c.value} 
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Campo: Mensagem */}
        <div className="form-group">
          <label>Mensagem:</label>
          <textarea
            {...register('content')}
            rows={5}
          />
          {errors.content && <span className="erro">{errors.content.message}</span>}
        </div>

        {/* 4. Anônimo */}
        <div className="form-group checkbox">
          <label>
            <input type="checkbox" {...register('isAnonymous')} /> Enviar anonimamente?
          </label>
        </div>

        <div className="form-actions">
          <button type="button" onClick={onClose} className="btn-secondary">Cancelar</button>
          <button type="submit" className="btn-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Enviando...' : 'Enviar'}
          </button>
        </div>

      </form>
    </Modal>
  );
};