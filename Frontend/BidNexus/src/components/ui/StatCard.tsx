import React from 'react';

interface StatCardProps {
  title: string;
  value: number | string;
  icon?: React.ReactNode;
  subtitle?: string;
  tone?: 'default' | 'live' | 'warning' | 'info';
  className?: string;
}

export const StatCard: React.FC<StatCardProps> = ({
  title,
  value,
  icon,
  subtitle,
  tone = 'default',
  className = '',
}) => {
  return (
    <div className={`bn-stat-card bn-stat-${tone} ${className}`}>
      <div className="bn-stat-head">
        <span className="bn-stat-title">{title}</span>
        {icon && <span className="bn-stat-icon">{icon}</span>}
      </div>
      <div className="bn-stat-value">{value}</div>
      {subtitle && <div className="bn-stat-subtitle">{subtitle}</div>}
    </div>
  );
};
