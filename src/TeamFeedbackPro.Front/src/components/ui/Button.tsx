import React from 'react';
import './css/Button.css'; 


type ButtonProps = React.ComponentProps<'button'> & {
    
};

export const Button = ({ children, className, ...props }: ButtonProps) => {
  return (
    
    <button className={`btn ${className || ''}`} {...props}>
      {children}
    </button>
  );
};