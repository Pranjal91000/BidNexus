import type { Auction, Requirement } from '../types';

/**
 * One place that decides what state an auction is in.
 * Server statuses: 1 Draft, 2 Authorized, 3 OpenForIntent, 4 IntentEvaluation, 5 Scheduled, 6 Open, 7 Completed.
 * The lifecycle worker flips Open/Completed on a poll, so the clock is used to bridge the gap.
 */
export type Phase = 'draft' | 'upcoming' | 'live' | 'closing' | 'closed';

export const STATUS = { Draft: 1, Authorized: 2, Scheduled: 5, Open: 6, Completed: 7 } as const;

const statusIdOf = (a: Auction): number | undefined => {
  if (a.statusId) return a.statusId;
  const n = (a.statusName || '').toLowerCase();
  if (n.includes('draft')) return STATUS.Draft;
  if (n.includes('complete') || n.includes('close')) return STATUS.Completed;
  if (n === 'open' || n.includes('live') || n.includes('active')) return STATUS.Open;
  return undefined;
};

export function phaseOf(a: Auction, now = Date.now()): Phase {
  const status = statusIdOf(a);
  if (status === STATUS.Draft) return 'draft';
  if (status === STATUS.Completed) return 'closed';
  const start = new Date(a.auctionStartTime).getTime();
  const end = new Date(a.auctionEndTime).getTime();
  if (now < start) return 'upcoming';
  if (now < end) return 'live';
  return 'closing';
}

/** The server only accepts bids while the status is Open. */
export const acceptsBids = (a: Auction, now = Date.now()): boolean =>
  statusIdOf(a) === STATUS.Open && now < new Date(a.auctionEndTime).getTime();

export const PHASE_LABEL: Record<Phase, string> = {
  draft: 'Draft',
  upcoming: 'Upcoming',
  live: 'Live',
  closing: 'Finalising',
  closed: 'Closed',
};

export const typeLabel = (a: Auction) => (a.isForwardAuction ? 'Forward auction' : 'Reverse auction');

/** Better bid comparator: lower wins in reverse auctions, higher in forward. */
export const isBetter = (a: number, b: number, forward: boolean) => (forward ? a > b : a < b);

export const auctionTitle = (a: Pick<Auction, 'auctionName' | 'docNoYearly'>) => a.auctionName || a.docNoYearly;


export const itemNameOf = (r: Requirement) => r.item?.itemName || r.item?.name || `Item #${r.itemId ?? r.id}`;
export const unitNameOf = (r: Requirement) => r.unit?.code || r.unit?.alias || r.unit?.name || r.unit?.unitName || 'unit';

/** Shared sort: soonest-ending first for live, soonest-starting for upcoming, newest first otherwise. */
export function sortForPhase(list: Auction[], phase: Phase): Auction[] {
  const t = (s: string) => new Date(s).getTime();
  const copy = [...list];
  if (phase === 'live') return copy.sort((a, b) => t(a.auctionEndTime) - t(b.auctionEndTime));
  if (phase === 'upcoming') return copy.sort((a, b) => t(a.auctionStartTime) - t(b.auctionStartTime));
  return copy.sort((a, b) => t(b.auctionEndTime) - t(a.auctionEndTime));
}
