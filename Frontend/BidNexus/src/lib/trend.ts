import type { TrendPoint } from '../components/charts/TrendChart';
import type { BidActivity } from '../types';

/**
 * Turns a time-ordered list of bid events into the "best bid in the market" over time.
 * Each bidder's latest amount counts; the best across bidders is the market line.
 */
export function bestOverTime(
  events: { t: number; v: number | null; bidder: string }[],
  forward: boolean,
): TrendPoint[] {
  const latest = new Map<string, number>();
  const out: TrendPoint[] = [];
  for (const e of events) {
    if (e.v == null) continue;
    latest.set(e.bidder, e.v);
    const vals = [...latest.values()];
    const best = forward ? Math.max(...vals) : Math.min(...vals);
    if (!out.length || out[out.length - 1].v !== best) out.push({ t: e.t, v: best });
  }
  return out;
}

/** Opening best vs current best, as a percentage improvement for the buyer. */
export function improvementFrom(activity: BidActivity[], forward: boolean): { opening: number; current: number; percent: number } | null {
  const first = new Map<string, number>();
  const latest = new Map<string, number>();
  for (const e of activity) {
    if (e.netAmount == null) continue;
    if (!first.has(e.bidder)) first.set(e.bidder, e.netAmount);
    latest.set(e.bidder, e.netAmount);
  }
  if (!first.size) return null;
  const pick = (vals: number[]) => (forward ? Math.max(...vals) : Math.min(...vals));
  const opening = pick([...first.values()]);
  const current = pick([...latest.values()]);
  if (!opening) return null;
  const percent = forward ? ((current - opening) / opening) * 100 : ((opening - current) / opening) * 100;
  return { opening, current, percent };
}
