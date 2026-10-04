import type { ReactNode } from 'react';
import { PHASE_LABEL, type Phase } from '../../lib/auction';

export function Badge({ tone, children }: { tone?: 'live' | 'upcoming' | 'draft' | 'closing' | 'closed' | 'won' | 'warn'; children: ReactNode }) {
  return <span className={`badge${tone ? ` badge--${tone}` : ''}`}>{children}</span>;
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
