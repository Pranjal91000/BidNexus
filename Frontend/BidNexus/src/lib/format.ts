const inrFull = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 0 });
const inrPaise = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', minimumFractionDigits: 2, maximumFractionDigits: 2 });

/** ₹18,42,500 — whole rupees, Indian grouping. */
export const inr = (n: number | null | undefined): string => (n == null || isNaN(n) ? '—' : inrFull.format(n));

/** ₹52,400.50 — for unit rates where paise matter. */
export const inrRate = (n: number | null | undefined): string => {
  if (n == null || isNaN(n)) return '—';
  return Number.isInteger(n) ? inrFull.format(n) : inrPaise.format(n);
};

/** ₹18.4 L / ₹2.1 Cr — for tiles and chart axes. */
export const inrCompact = (n: number | null | undefined): string => {
  if (n == null || isNaN(n)) return '—';
  const abs = Math.abs(n);
  if (abs >= 1e7) return `₹${trim(n / 1e7)} Cr`;
  if (abs >= 1e5) return `₹${trim(n / 1e5)} L`;
  if (abs >= 1e3) return `₹${trim(n / 1e3)} K`;
  return inrFull.format(n);
};

const trim = (v: number) => (Math.abs(v) >= 100 ? v.toFixed(0) : v.toFixed(1).replace(/\.0$/, ''));

export const pct = (n: number | null | undefined, digits = 1): string =>
  n == null || isNaN(n) ? '—' : `${n.toFixed(digits).replace(/\.0$/, '')}%`;

export const qty = (n: number): string => new Intl.NumberFormat('en-IN', { maximumFractionDigits: 3 }).format(n);

export const dateTime = (iso?: string | null): string => {
  if (!iso) return '—';
  const d = new Date(iso);
  if (isNaN(d.getTime())) return '—';
  return d.toLocaleString('en-IN', { day: 'numeric', month: 'short', year: 'numeric', hour: 'numeric', minute: '2-digit' });
};

export const dateShort = (iso?: string | null): string => {
  if (!iso) return '—';
  const d = new Date(iso);
  if (isNaN(d.getTime())) return '—';
  const sameYear = d.getFullYear() === new Date().getFullYear();
  return d.toLocaleString('en-IN', { day: 'numeric', month: 'short', ...(sameYear ? {} : { year: 'numeric' }), hour: 'numeric', minute: '2-digit' });
};

export const time = (iso?: string | null): string => {
  if (!iso) return '—';
  const d = new Date(iso);
  return isNaN(d.getTime()) ? '—' : d.toLocaleTimeString('en-IN', { hour: 'numeric', minute: '2-digit' });
};

/** 1:42:10 / 12:48 / 0:09 */
export const clock = (ms: number): string => {
  const total = Math.max(0, Math.floor(ms / 1000));
  const h = Math.floor(total / 3600);
  const m = Math.floor((total % 3600) / 60);
  const s = total % 60;
  const pad = (v: number) => String(v).padStart(2, '0');
  return h > 0 ? `${h}:${pad(m)}:${pad(s)}` : `${m}:${pad(s)}`;
};

/** "in 3 days", "in 2 h", "in 14 min" — for schedules further out than a ticking clock is useful. */
export const relative = (ms: number): string => {
  const min = Math.round(ms / 60000);
  if (min < 60) return `${Math.max(1, min)} min`;
  const h = Math.round(min / 60);
  if (h < 48) return `${h} h`;
  return `${Math.round(h / 24)} days`;
};

export const monthLabel = (yyyyMm: string): string => {
  const [y, m] = yyyyMm.split('-').map(Number);
  return new Date(y, (m || 1) - 1, 1).toLocaleString('en-IN', { month: 'short' });
};
