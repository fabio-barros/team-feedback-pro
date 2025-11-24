import React from 'react';
import './css/CadastroComponent.css'; 

export const CadastroComponent = () => {
  return (
    <form>
      <div className="form-row">
        <label htmlFor="nome">Nome</label>
        <input name="nome" id="nome" type="text" required/>
      </div>

      <div className="form-row">
        <label htmlFor="email">Email</label>
        <input name="email" id="email" type="email" required/>
      </div>

      <div className="form-row">
        <label htmlFor="cargo">Cargo</label>
        <select name="cargo" id="cargo" required>
          <option value="1">Editor</option>
          <option value="2">Coordenador</option>
          <option value="3">Direção</option>
        </select>
      </div>

      <div className="form-row">
        <label htmlFor="time">Time</label>
        <select name="time" id="time">
          <option value="1">Time 1</option>
          <option value="2">Time 2</option>
          <option value="3">Time 3</option>
        </select>
      </div>

      <div className="form-row">
        <label htmlFor="pass">Senha</label>
        <input name="pass" id="pass" type="password" required/>
      </div>

      <div className="form-row">
        <label htmlFor="pass2">Repetir senha</label>
        <input name="pass2" id="pass2" type="password" required/>
      </div>

      <div className="form-actions">
        <button type="submit" className="btn-secondary">
          Enviar
        </button>
      </div>
    </form>
  );
};
