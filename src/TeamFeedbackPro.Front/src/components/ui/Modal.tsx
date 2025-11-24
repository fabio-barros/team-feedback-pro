import React from 'react';
import './css/Modal.css';

type ModalProps = {
  isOpen: boolean;       
  onClose: () => void; 
  title: string;         
  children: React.ReactNode; 
};

export const Modal = ({ isOpen, onClose, title, children }: ModalProps) => {
  if (!isOpen) {
    return null;
  }


  return (
   
    <div className="modal__overlay" onClick={onClose}>
      
      
      <div className="modal__content" onClick={(e) => e.stopPropagation()}>
        
        
        <div className="modal__header">
          <h2 className="modal__title">{title}</h2>
          <button className="modal__close-btn" onClick={onClose}>
            &times; 
          </button>
        </div>

        
        <div className="modal__body">
          {children}
        </div>

      </div>
    </div>
  );
};