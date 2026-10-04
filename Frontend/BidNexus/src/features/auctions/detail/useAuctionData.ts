import { useCallback, useEffect, useRef, useState } from 'react';
import type { Auction, Bid, BidActivity, Statement, VendorAuctionResult } from '../../../types';
import { api } from '../../../services/api';
import { useAuctionSignalR } from '../../../services/signalr';
import { errorMessage, useApp } from '../../../lib/appContext';
import { phaseOf } from '../../../lib/auction';

export interface AuctionData {
  auction: Auction | null;
  error: string;
  loading: boolean;
  leaderboard: Bid[];
  activity: BidActivity[];
  history: Bid[];
  statement: Statement[];
  myResult: VendorAuctionResult | null;
  connection: string;
  reload: () => Promise<void>;
}

/**
 * Loads everything the detail screen needs for the caller's role and the auction's phase,
 * and keeps it fresh through SignalR (new bids, start, close).
 */
export function useAuctionData(auctionId: number): AuctionData {
  const { token, claims, isVendor, isOrg, refresh, toast } = useApp();
  const [auction, setAuction] = useState<Auction | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [leaderboard, setLeaderboard] = useState<Bid[]>([]);
  const [activity, setActivity] = useState<BidActivity[]>([]);
  const [history, setHistory] = useState<Bid[]>([]);
  const [statement, setStatement] = useState<Statement[]>([]);
  const [myResult, setMyResult] = useState<VendorAuctionResult | null>(null);

  const reload = useCallback(async () => {
    try {
      const a = await api.getAuctionById(token, auctionId);
      setAuction(a);
      setError('');
      const phase = phaseOf(a);
      const started = phase === 'live' || phase === 'closing' || phase === 'closed';

      const jobs: Promise<unknown>[] = [];
      if (started) {
        jobs.push(api.getLeaderboard(token, auctionId).then((r) => setLeaderboard(r || [])).catch(() => setLeaderboard([])));
        jobs.push(api.getBidActivity(token, auctionId).then((r) => setActivity(r || [])).catch(() => setActivity([])));
      }
      if (isVendor && started) {
        jobs.push(api.getBidHistory(token, auctionId, claims.userId).then((r) => setHistory(r || [])).catch(() => setHistory([])));
      }
      if (isOrg && phase === 'closed') {
        jobs.push(api.getAuctionStatement(token, auctionId).then((r) => setStatement(r || [])).catch(() => setStatement([])));
      }
      if (isVendor && phase === 'closed') {
        jobs.push(api.getMyResult(token, auctionId).then(setMyResult).catch(() => setMyResult(null)));
      }
      await Promise.all(jobs);
    } catch (err) {
      setError(errorMessage(err, 'Could not load this auction.'));
    } finally {
      setLoading(false);
    }
  }, [token, auctionId, isVendor, isOrg, claims.userId]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- fetch on mount / id change
    setLoading(true);
    reload();
  }, [reload]);

  // SignalR callbacks must be stable or the hub reconnects on every render.
  const reloadRef = useRef(reload);
  const refreshRef = useRef(refresh);
  const toastRef = useRef(toast);
  useEffect(() => {
    reloadRef.current = reload;
    refreshRef.current = refresh;
    toastRef.current = toast;
  }, [reload, refresh, toast]);

  const onBid = useCallback(() => {
    reloadRef.current();
  }, []);
  const onStarted = useCallback(() => {
    toastRef.current('The auction is now live.', 'info');
    reloadRef.current();
  }, []);
  const onCompleted = useCallback(() => {
    toastRef.current('The auction has closed.', 'info');
    reloadRef.current();
    refreshRef.current();
  }, []);

  const { status } = useAuctionSignalR(token, auctionId, onBid, undefined, onCompleted, onStarted);

  // If the clock passes the end but the server event is missed, poll until results exist.
  useEffect(() => {
    if (!auction) return;
    const phase = phaseOf(auction);
    if (phase !== 'closing' && phase !== 'live') return;
    const ms = phase === 'live' ? Math.max(1000, new Date(auction.auctionEndTime).getTime() - Date.now() + 1500) : 10000;
    const id = window.setTimeout(() => reloadRef.current(), ms);
    return () => window.clearTimeout(id);
  }, [auction]);

  return { auction, error, loading, leaderboard, activity, history, statement, myResult, connection: status, reload };
}
