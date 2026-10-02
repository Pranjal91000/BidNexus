import React from 'react';
import { Inbox } from 'lucide-react';
import { Button } from './Button';

interface EmptyStateProps {
  title: string;
  description: string;
  icon?: React.ReactNode;
  actionText?: string;
  onAction?: () => void;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  title,
  description,
  icon,
  actionText,
  onAction,
}) => {
  return (
    <div className="bn-empty-state">
      <div className="bn-empty-icon">{icon || <Inbox size={32} />}</div>
      <h3 className="bn-empty-title">{title}</h3>
      <p className="bn-empty-desc">{description}</p>
      {actionText && onAction && (
        <Button variant="outline" size="sm" onClick={onAction} className="bn-mt-4">
          {actionText}
        </Button>
      )}
    </div>
  );
};
