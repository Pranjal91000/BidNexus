import React, { useEffect } from 'react';
import { X, AlertTriangle } from 'lucide-react';
import { Button } from './Button';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  subtitle?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  maxWidth?: 'sm' | 'md' | 'lg' | 'xl';
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  title,
  subtitle,
  children,
  footer,
  maxWidth = 'md',
}) => {
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };
    if (isOpen) {
      document.body.style.overflow = 'hidden';
      window.addEventListener('keydown', handleKeyDown);
    }
    return () => {
      document.body.style.overflow = 'auto';
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  return (
    <div className="bn-modal-backdrop" onClick={onClose}>
      <div
        className={`bn-modal bn-modal-${maxWidth}`}
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
      >
        <div className="bn-modal-header">
          <div>
            <h3 className="bn-modal-title">{title}</h3>
            {subtitle && <p className="bn-modal-subtitle">{subtitle}</p>}
          </div>
          <button className="bn-modal-close" onClick={onClose} aria-label="Close modal">
            <X size={18} />
          </button>
        </div>
        <div className="bn-modal-body">{children}</div>
        {footer && <div className="bn-modal-footer">{footer}</div>}
      </div>
    </div>
  );
};

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: 'danger' | 'warning' | 'primary';
  loading?: boolean;
}

export const ConfirmDialog: React.FC<ConfirmDialogProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = 'Confirm',
  cancelText = 'Cancel',
  variant = 'danger',
  loading = false,
}) => {
  if (!isOpen) return null;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={title}
      maxWidth="sm"
      footer={
        <div className="bn-flex-end gap-2">
          <Button variant="outline" onClick={onClose} disabled={loading}>
            {cancelText}
          </Button>
          <Button variant={variant === 'danger' ? 'danger' : 'primary'} onClick={onConfirm} loading={loading}>
            {confirmText}
          </Button>
        </div>
      }
    >
      <div className="bn-confirm-body">
        <div className={`bn-confirm-icon ${variant}`}>
          <AlertTriangle size={24} />
        </div>
        <p className="bn-confirm-message">{message}</p>
      </div>
    </Modal>
  );
};
