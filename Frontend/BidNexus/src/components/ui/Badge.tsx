import type { ReactNode } from 'react';
import { PHASE_LABEL, type Phase } from '../../lib/auction';

export function toneFromStatus(status?: string): 'live' | 'upcoming' | 'draft' | 'closing' | 'closed' | 'won' | 'warn' {
  const s = (status || '').toLowerCase();
  if (s.includes('active') || s.includes('live') || s === 'open') return 'live';
  if (s.includes('close') || s.includes('complet')) return 'closed';
  if (s.includes('award')) return 'won';
  if (s.includes('pend') || s.includes('draft')) return 'draft';
  return 'upcoming';
}

export function Badge({
  tone,
  children,
  icon,
}: {
  tone?: 'live' | 'upcoming' | 'draft' | 'closing' | 'closed' | 'won' | 'warn' | 'info' | string;
  children: ReactNode;
  icon?: boolean;
}) {
  const safeTone = tone === 'info' ? 'upcoming' : tone;
  return (
    <span className={`badge${safeTone ? ` badge--${safeTone}` : ''}`}>
      {icon && <span className="dot dot--pulse" aria-hidden="true" />}
      {children}
    </span>
  );
}

export function PhaseBadge({ phase }: { phase: Phase }) {
  return (
    <Badge tone={phase}>
      {phase === 'live' && <span className="dot dot--pulse" aria-hidden="true" />}
      {PHASE_LABEL[phase]}
    </Badge>
  );
}

export function Rank({ rank }: { rank: number | null | undefined }) {
  if (!rank) return <span className="rank">—</span>;
  return <span className={`rank${rank === 1 ? ' rank--first' : ''}`}>L{rank}</span>;
}
