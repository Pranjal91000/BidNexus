import React from 'react';
import { CheckCircle2, AlertTriangle, Info, XCircle, X } from 'lucide-react';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastMessage {
  id: string;
  type: ToastType;
  title?: string;
  message: string;
}

interface ToastProps {
  toasts: ToastMessage[];
  onDismiss: (id: string) => void;
}

export const ToastContainer: React.FC<ToastProps> = ({ toasts, onDismiss }) => {
  if (toasts.length === 0) return null;

  return (
    <div className="bn-toast-container" role="region" aria-label="Notifications">
      {toasts.map((toast) => {
        const Icon =
          toast.type === 'success'
            ? CheckCircle2
            : toast.type === 'error'
            ? XCircle
            : toast.type === 'warning'
            ? AlertTriangle
            : Info;

        return (
          <div key={toast.id} className={`bn-toast bn-toast-${toast.type}`}>
            <span className="bn-toast-icon">
              <Icon size={18} />
            </span>
            <div className="bn-toast-content">
              {toast.title && <strong className="bn-toast-title">{toast.title}</strong>}
              <p className="bn-toast-msg">{toast.message}</p>
            </div>
            <button
              className="bn-toast-dismiss"
              onClick={() => onDismiss(toast.id)}
              aria-label="Dismiss notification"
            >
              <X size={14} />
            </button>
          </div>
        );
      })}
    </div>
  );
};
