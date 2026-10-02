import React from 'react';

export type BadgeTone = 'live' | 'scheduled' | 'closed' | 'cancelled' | 'forward' | 'reverse' | 'winner' | 'info' | 'warning';

interface BadgeProps {
  tone?: BadgeTone | string;
  children: React.ReactNode;
  icon?: boolean;
  className?: string;
}

export function toneFromStatus(statusName?: string): BadgeTone {
  if (!statusName) return 'scheduled';
  const s = statusName.toLowerCase();
  if (s.includes('active') || s.includes('live')) return 'live';
  if (s.includes('close') || s.includes('complete') || s.includes('award') || s.includes('ended')) return 'closed';
  if (s.includes('cancel')) return 'cancelled';
  return 'scheduled';
}

export const Badge: React.FC<BadgeProps> = ({ tone = 'scheduled', children, icon = true, className = '' }) => {
  const normalizedTone = typeof tone === 'string' ? toneFromStatus(tone) : tone;

  return (
    <span className={`bn-badge bn-badge-${normalizedTone} ${className}`}>
      {icon && normalizedTone === 'live' && <span className="bn-pulse-dot" />}
      {children}
    </span>
  );
};
