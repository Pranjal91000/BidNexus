import { useNow } from '../../lib/hooks';
import { clock, relative } from '../../lib/format';

const SOON_MS = 5 * 60 * 1000;

/** Inline "Ends in 12:48" text. Switches to orange in the last five minutes. */
export function Countdown({
  to,
  endTime,
  prefix = 'Ends in',
  endedLabel = 'Ended',
  compact: _compact,
  startTime: _startTime,
  size: _size,
}: {
  to?: string;
  endTime?: string;
  prefix?: string;
  endedLabel?: string;
  compact?: boolean;
  startTime?: string;
  size?: string;
}) {
  const target = to || endTime || '';
  const now = useNow();
  const ms = target ? new Date(target).getTime() - now : 0;
  if (!target || ms <= 0) return <span className="muted">{endedLabel}</span>;
  const soon = ms < SOON_MS;
  const text = ms > 24 * 3600 * 1000 ? relative(ms) : clock(ms);
  return <span className={`num${soon ? ' warn strong' : ''}`}>{prefix} {text}</span>;
}

/** The large timer used on the live auction screen. */
export function BigTimer({ to }: { to: string }) {
  const now = useNow();
  const ms = new Date(to).getTime() - now;
  const ended = ms <= 0;
  const soon = !ended && ms < SOON_MS;
  return (
    <div className={`timer${soon ? ' timer--soon' : ''}`} aria-live="off">
      <span className="small" style={{ fontWeight: 500, color: soon ? 'var(--warn)' : 'var(--muted)' }}>
        {ended ? 'Bidding ended' : soon ? 'Closing soon' : 'Time left'}
      </span>
      <span className="timer__value num">{ended ? '0:00' : clock(ms)}</span>
    </div>
  );
}
