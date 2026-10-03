import React from 'react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'danger' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  icon?: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  loading = false,
  icon,
  children,
  className = '',
  disabled,
  ...props
}) => {
  return (
    <button
      className={`bn-btn bn-btn-${variant} bn-btn-${size} ${loading ? 'bn-btn-loading' : ''} ${className}`}
      disabled={disabled || loading}
      {...props}
    >
      {loading ? (
        <span className="bn-spinner" aria-hidden="true" />
      ) : (
        icon && <span className="bn-btn-icon">{icon}</span>
      )}
      <span>{children}</span>
    </button>
  );
};
