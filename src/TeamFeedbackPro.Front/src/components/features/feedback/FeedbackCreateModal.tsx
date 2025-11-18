import { useForm, type SubmitHandler } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import './css/FeedbackCreateModal.css';

import { Modal } from '../../ui/Modal';


const feedbackSchema = z.object({
  targetId: z.string().min(1, 'Você precisa selecionar uma pessoa.'),
  mensagem: z.string().min(10, 'O feedback precisa ter pelo menos 10 caracteres.'),
});


type FeedbackFormInputs = z.infer<typeof feedbackSchema>;


type FeedbackCreateModalProps = {
  isOpen: boolean;
  onClose: () => void;
  onFeedbackEnviado: () => void; 
};

export const FeedbackCreateModal = ({ isOpen, onClose }: FeedbackCreateModalProps) => {
  
  const { 
    register, 
    handleSubmit, 
    formState: { errors, isSubmitting },
    reset 
  } = useForm<FeedbackFormInputs>({
    resolver: zodResolver(feedbackSchema)
  });

  
  const onSubmit: SubmitHandler<FeedbackFormInputs> = async (data) => {
    try {
      
      reset();    
      onClose();  
    } catch (error) {
      console.error('Erro ao enviar feedback', error);
    }
  };


  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Enviar Novo Feedback">
      <form onSubmit={handleSubmit(onSubmit)}>
        
        
        <div className="form-group">
          <label htmlFor="targetId">Para:</label>
          <select id="targetId" {...register('targetId')}>
            <option value="">Selecione...</option>
            {/* Buscar da API */}
            <option value="1">Aline Limeira</option>
            <option value="2">Fábio Ribeiro</option>
          </select>
          {errors.targetId && <span className="erro">{errors.targetId.message}</span>}
        </div>

        {/* Campo: Mensagem */}
        <div className="form-group">
          <label htmlFor="mensagem">Mensagem:</label>
          <textarea 
            id="mensagem" 
            {...register('mensagem')} 
            rows={5}
          />
          {errors.mensagem && <span className="erro">{errors.mensagem.message}</span>}
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