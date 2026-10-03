import React from 'react';
import { RefreshCw, AlertCircle } from 'lucide-react';
import { Button } from './Button';

interface LoadingStateProps {
  message?: string;
}

export const LoadingState: React.FC<LoadingStateProps> = ({ message = 'Loading workspace data...' }) => {
  return (
    <div className="bn-loading-state">
      <div className="bn-loading-spinner" />
      <p className="bn-loading-text">{message}</p>
    </div>
  );
};

interface ErrorStateProps {
  title?: string;
  message: string;
  onRetry?: () => void;
}

export const ErrorState: React.FC<ErrorStateProps> = ({
  title = 'System Notice',
  message,
  onRetry,
}) => {
  return (
    <div className="bn-error-state">
      <div className="bn-error-icon">
        <AlertCircle size={28} />
      </div>
      <div>
        <h4 className="bn-error-title">{title}</h4>
        <p className="bn-error-msg">{message}</p>
      </div>
      {onRetry && (
        <Button variant="outline" size="sm" icon={<RefreshCw size={14} />} onClick={onRetry}>
          Retry
        </Button>
      )}
    </div>
  );
};
