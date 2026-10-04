import { X } from 'lucide-react';

export type ToastTone = 'success' | 'error' | 'warning' | 'info';
export interface ToastMessage {
  id: string;
  message: string;
  type: ToastTone;
}

export function Toasts({ toasts, onDismiss }: { toasts: ToastMessage[]; onDismiss: (id: string) => void }) {
  return (
    <div className="toasts" aria-live="polite">
      {toasts.map((t) => (
        <div key={t.id} className={`toast${t.type === 'error' ? ' toast--error' : ''}`}>
          <span>{t.message}</span>
          <button type="button" className="toast__close" onClick={() => onDismiss(t.id)} aria-label="Dismiss">
            <X size={16} />
          </button>
        </div>
      ))}
    </div>
  );
}
