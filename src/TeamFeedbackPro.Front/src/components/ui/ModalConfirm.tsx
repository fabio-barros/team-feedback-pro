import React from "react";
import "./css/ModalConfirm.css";

type ModalConfirmProps = {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  textareaEnabled?: boolean;           // <--- NOVO
  textareaValue?: string;             // <--- NOVO
  onTextareaChange?: (v: string) => void;
  onConfirm: () => void;
  onCancel: () => void;
};


export const ModalConfirm = ({
  open,
  title,
  message,
  confirmLabel = "Confirmar",
  cancelLabel = "Cancelar",
  textareaEnabled = false,
  textareaValue = "",   
  onTextareaChange,
  onConfirm,
  onCancel
}: ModalConfirmProps) => {

  if (!open) return null;

  return (
    <div className="modal-overlay">
      <div className="modal-box">
        <h3>{title}</h3>
        <p>{message}</p>
        {textareaEnabled && (
          <textarea
            className="modal-textarea"
            placeholder="Digite a justificativa da rejeição..."
            value={textareaValue}
            onChange={(e) => onTextareaChange?.(e.target.value)}
            required
          />
        )}

        <div className="modal-actions">
          <button className="btn-cancel" onClick={onCancel}>{cancelLabel}</button>
          <button className="btn-confirm" onClick={onConfirm}>{confirmLabel}</button>
        </div>
      </div>
    </div>
  );
};
