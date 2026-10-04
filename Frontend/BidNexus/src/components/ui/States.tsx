import type { ReactNode } from 'react';

export function Loading({ label = 'Loading' }: { label?: string }) {
  return (
    <div className="loading" role="status">
      <span className="spinner" aria-hidden="true" />
      {label}
    </div>
  );
}

export function Empty({ title, children, action }: { title: string; children?: ReactNode; action?: ReactNode }) {
  return (
    <div className="empty">
      <span className="empty__title">{title}</span>
      {children && <span className="small">{children}</span>}
      {action && <div style={{ marginTop: 8 }}>{action}</div>}
    </div>
  );
}

export function Callout({ tone, children }: { tone?: 'error' | 'warn' | 'info'; children: ReactNode }) {
  return (
    <div className={`callout${tone ? ` callout--${tone}` : ''}`} role={tone === 'error' ? 'alert' : undefined}>
      {children}
    </div>
  );
}

export function Stat({ label, value, sub }: { label: string; value: ReactNode; sub?: ReactNode }) {
  return (
    <div className="card stat">
      <span className="stat__label">{label}</span>
      <span className="stat__value num">{value}</span>
      {sub && <span className="stat__sub">{sub}</span>}
    </div>
  );
}
