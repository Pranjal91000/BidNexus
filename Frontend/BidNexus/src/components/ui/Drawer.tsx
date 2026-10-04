import React, { useEffect } from 'react';
import { X } from 'lucide-react';

interface DrawerProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
}

export const Drawer: React.FC<DrawerProps> = ({ isOpen, onClose, title, children }) => {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'auto';
    }
    return () => {
      document.body.style.overflow = 'auto';
    };
  }, [isOpen]);

  if (!isOpen) return null;

  return (
    <div className="bn-drawer-backdrop" onClick={onClose}>
      <div className="bn-drawer" onClick={(e) => e.stopPropagation()}>
        <div className="bn-drawer-header">
          {title && <h3 className="bn-drawer-title">{title}</h3>}
          <button className="bn-drawer-close" onClick={onClose} aria-label="Close drawer">
            <X size={20} />
          </button>
        </div>
        <div className="bn-drawer-content">{children}</div>
      </div>
    </div>
  );
};
