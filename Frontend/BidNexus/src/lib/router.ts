import { useCallback, useEffect, useState } from 'react';
import type { Page } from '../types';

/**
 * Tiny hash router: #/overview, #/auctions?tab=closed, #/auctions/12, #/masters, #/profile.
 * Keeps the browser Back button and shareable links working without a router dependency.
 */
export interface Route {
  page: Page;
  auctionId: number | null;
  tab: string | null;
}

const PAGES: Page[] = ['overview', 'auctions', 'masters', 'profile'];

export function parseHash(hash: string): Route {
  const [path, query = ''] = hash.replace(/^#\/?/, '').split('?');
  const [first, second] = path.split('/');
  const page = (PAGES as string[]).includes(first) ? (first as Page) : 'overview';
  const id = page === 'auctions' && second ? Number(second) : NaN;
  const tab = new URLSearchParams(query).get('tab');
  return { page, auctionId: Number.isFinite(id) ? id : null, tab };
}

export function toHash(r: Partial<Route> & { page: Page }): string {
  let h = `#/${r.page}`;
  if (r.page === 'auctions' && r.auctionId) h += `/${r.auctionId}`;
  if (r.tab) h += `?tab=${encodeURIComponent(r.tab)}`;
  return h;
}

export function useRoute() {
  const [route, setRoute] = useState<Route>(() => parseHash(window.location.hash));
  useEffect(() => {
    const onChange = () => setRoute(parseHash(window.location.hash));
    window.addEventListener('hashchange', onChange);
    return () => window.removeEventListener('hashchange', onChange);
  }, []);
  const navigate = useCallback((r: Partial<Route> & { page: Page }) => {
    const next = toHash(r);
    if (window.location.hash !== next) window.location.hash = next;
    window.scrollTo(0, 0);
  }, []);
  return [route, navigate] as const;
}
