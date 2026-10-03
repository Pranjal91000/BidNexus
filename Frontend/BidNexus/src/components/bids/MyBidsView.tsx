import React, { useState, useEffect } from 'react';
import type { Auction, Bid, Claims } from '../../types';
import { RefreshCw } from 'lucide-react';
import { Button } from '../ui/Button';
import { Badge } from '../ui/Badge';
import { LoadingState } from '../ui/LoadingState';
import { EmptyState } from '../ui/EmptyState';
import { api } from '../../services/api';

interface MyBidsViewProps {
  auctions: Auction[];
  token: string;
  claims: Claims;
  onOpenAuction: (auction: Auction) => void;
  onShowToast: (message: string, type?: 'success' | 'error' | 'warning' | 'info') => void;
}

export const MyBidsView: React.FC<MyBidsViewProps> = ({
  auctions,
  token,
  claims,
  onOpenAuction,
  onShowToast,
}) => {
  const [bidsByAuction, setBidsByAuction] = useState<{ auction: Auction; bids: Bid[] }[]>([]);
  const [loading, setLoading] = useState(true);

  const loadMyBids = async () => {
    if (!claims.userId) return;
    setLoading(true);
    try {
      const results = await Promise.all(
        auctions.slice(0, 30).map(async (a) => {
          try {
            const bids = await api.getBidHistory(token, a.id, claims.userId);
            return { auction: a, bids: bids || [] };
          } catch {
            return { auction: a, bids: [] };
          }
        })
      );
      setBidsByAuction(results.filter((r) => r.bids.length > 0));
    } catch (err: any) {
      onShowToast(err instanceof Error ? err.message : 'Unable to load bid activity', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadMyBids();
  }, [auctions, token, claims.userId]);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 2,
    }).format(amount);
  };

  const formatDate = (val?: string) => {
    if (!val) return '—';
    const d = new Date(val);
    return isNaN(d.getTime()) ? val : d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' });
  };

  return (
    <div className="bn-my-bids-stack">
      <div className="bn-section-header">
        <div>
          <span className="bn-eyebrow">VENDOR ACTIVITY</span>
          <h2 className="bn-section-title">My Bidding History & Revisions</h2>
        </div>
        <Button variant="outline" size="sm" icon={<RefreshCw size={14} />} onClick={loadMyBids}>
          Refresh Bids
        </Button>
      </div>

      {loading ? (
        <LoadingState message="Fetching your bid history..." />
      ) : bidsByAuction.length > 0 ? (
        <div className="bn-my-bids-list">
          {bidsByAuction.map(({ auction, bids }) => {
            const activeBid = bids.find((b) => b.isCurrent) || bids[0];

            return (
              <div key={auction.id} className="bn-my-bids-card">
                <div className="bn-card-header">
                  <div className="bn-flex-center gap-2">
                    <Badge tone={auction.statusName}>{auction.statusName || 'Scheduled'}</Badge>
                    <span className="bn-type-badge">
                      {auction.isForwardAuction ? 'FORWARD' : 'REVERSE'} AUCTION
                    </span>
                  </div>
                  <Button variant="outline" size="sm" onClick={() => onOpenAuction(auction)}>
                    Open Workstation →
                  </Button>
                </div>

                <h3 className="bn-card-title">{auction.auctionName || auction.docNoYearly}</h3>
                <p className="bn-card-meta">{auction.docNoYearly} • {auction.organization?.name || 'Organization'}</p>

                {activeBid && (
                  <div className="bn-active-bid-summary bn-mt-3">
                    <div>
                      <span className="bn-text-xs bn-text-muted">YOUR ACTIVE BID (R{activeBid.bidRevisionNo})</span>
                      <div className="bn-font-mono bn-amount-large bn-text-accent">
                        {formatCurrency(activeBid.netAmount)}
                      </div>
                    </div>
                    <span className="bn-text-xs bn-text-muted">Submitted: {formatDate(activeBid.createdAt)}</span>
                  </div>
                )}

                <div className="bn-revisions-table-container bn-mt-3">
                  <span className="bn-text-xs bn-font-bold bn-text-muted">ALL REVISIONS ({bids.length})</span>
                  <table className="bn-table bn-input-sm bn-mt-1">
                    <thead>
                      <tr>
                        <th>Revision</th>
                        <th>Amount</th>
                        <th>Timestamp</th>
                        <th>Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {bids.map((b) => (
                        <tr key={b.id}>
                          <td>
                            <strong>R{b.bidRevisionNo}</strong>
                          </td>
                          <td>
                            <span className="bn-font-mono">{formatCurrency(b.netAmount)}</span>
                          </td>
                          <td>
                            <span className="bn-text-xs">{formatDate(b.createdAt)}</span>
                          </td>
                          <td>
                            {b.isCurrent ? (
                              <span className="bn-badge bn-badge-live">Current</span>
                            ) : (
                              <span className="bn-badge bn-badge-scheduled">Superseded</span>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        <EmptyState
          title="No bidding activity found"
          description="Your submitted bids and revision history across live market auctions will appear here."
        />
      )}
    </div>
  );
};
