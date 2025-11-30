import React, { useEffect } from "react";
import "./css/Toast.css";

type ToastProps = {
  message: string;
  type: "success" | "error";
  onClose: () => void;
};

export const Toast = ({ message, type, onClose }: ToastProps) => {
  useEffect(() => {
    const t = setTimeout(() => onClose(), 3000);
    return () => clearTimeout(t);
  }, []);

  return (
    <div className={`toast toast-${type}`}>
      {message}
    </div>
  );
};
