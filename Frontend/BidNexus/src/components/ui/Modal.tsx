import { useCallback, useEffect, useRef, type ReactNode } from 'react';
import { X } from 'lucide-react';
import { useEscape } from '../../lib/hooks';

export interface ModalProps {
  open?: boolean;
  isOpen?: boolean;
  title: string;
  subtitle?: string;
  onClose: () => void;
  children: ReactNode;
  footer?: ReactNode;
  wide?: boolean;
  maxWidth?: string;
}

export function Modal({ open, isOpen, title, subtitle, onClose, children, footer, wide, maxWidth }: ModalProps) {
  const isVisible = open ?? isOpen ?? false;
  const dialogRef = useRef<HTMLDivElement>(null);
  const close = useCallback(() => onClose(), [onClose]);
  useEscape(isVisible, close);

  useEffect(() => {
    if (!isVisible) return;
    const previous = document.activeElement as HTMLElement | null;
    const first = dialogRef.current?.querySelector<HTMLElement>('input, select, textarea, button:not([data-close])');
    first?.focus();
    const overflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = overflow;
      previous?.focus?.();
    };
  }, [isVisible]);

  if (!isVisible) return null;

  return (
    <div className="modal-backdrop" onMouseDown={(e) => e.target === e.currentTarget && onClose()}>
      <div
        ref={dialogRef}
        className={`modal${wide ? ' modal--wide' : ''}`}
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        style={maxWidth ? { maxWidth } : undefined}
      >
        <div className="modal__head">
          <div>
            <h2 id="modal-title">{title}</h2>
            {subtitle && <p className="small muted" style={{ margin: '4px 0 0' }}>{subtitle}</p>}
          </div>
          <button type="button" className="icon-btn" onClick={onClose} aria-label="Close" data-close>
            <X size={18} />
          </button>
        </div>
        <div className="modal__body">{children}</div>
        {footer && <div className="modal__foot">{footer}</div>}
      </div>
    </div>
  );
}

export interface ConfirmProps {
  open?: boolean;
  isOpen?: boolean;
  title: string;
  message: ReactNode;
  confirmLabel?: string;
  confirmText?: string;
  danger?: boolean;
  variant?: string;
  busy?: boolean;
  loading?: boolean;
  onConfirm: () => void | Promise<void>;
  onCancel?: () => void;
  onClose?: () => void;
}

export function ConfirmDialog({
  open,
  isOpen,
  title,
  message,
  confirmLabel,
  confirmText,
  danger,
  variant,
  busy,
  loading,
  onConfirm,
  onCancel,
  onClose,
}: ConfirmProps) {
  const isVisible = open ?? isOpen ?? false;
  const cancel = onCancel ?? onClose ?? (() => {});
  const isBusy = busy ?? loading ?? false;
  const label = confirmLabel ?? confirmText ?? 'Confirm';
  const isDanger = danger ?? variant === 'danger';
  return (
    <Modal
      open={isVisible}
      title={title}
      onClose={cancel}
      footer={
        <>
          <button type="button" className="btn btn--secondary" onClick={cancel}>Cancel</button>
          <button type="button" className={`btn ${isDanger ? 'btn--danger' : 'btn--primary'}`} onClick={onConfirm} disabled={isBusy}>
            {isBusy && <span className="spinner" aria-hidden="true" />}
            {label}
          </button>
        </>
      }
    >
      <p>{message}</p>
    </Modal>
  );
}
