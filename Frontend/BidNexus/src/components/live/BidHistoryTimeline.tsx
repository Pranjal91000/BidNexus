import React from 'react';
import type { Bid } from '../../types';
import { History, ArrowDown, ArrowUp } from 'lucide-react';

interface BidHistoryTimelineProps {
  history: Bid[];
  isForward?: boolean;
}

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

export const BidHistoryTimeline: React.FC<BidHistoryTimelineProps> = ({ history }) => {
  if (history.length === 0) {
    return (
      <div className="bn-history-empty">
        <History size={28} className="bn-text-muted" />
        <p>No revision history recorded for your vendor account in this auction yet.</p>
      </div>
    );
  }

  return (
    <div className="bn-history-timeline-container">
      <div className="bn-table-header-kicker">
        <span className="bn-eyebrow">AUDIT TRAIL</span>
        <h4 className="bn-table-title">Your Vendor Bid Revisions</h4>
      </div>

      <div className="bn-timeline-list">
        {history.map((bid, index) => {
          const isLatest = index === 0;
          const prevBid = history[index + 1];
          let diffText = '';

          if (prevBid) {
            const diff = bid.netAmount - prevBid.netAmount;
            if (diff !== 0) {
              diffText = `${diff > 0 ? '+' : ''}${formatCurrency(diff)}`;
            }
          }

          return (
            <div key={bid.id} className={`bn-timeline-item ${isLatest ? 'is-latest' : ''}`}>
              <div className="bn-timeline-marker">
                <span className="bn-revision-number">R{bid.bidRevisionNo}</span>
              </div>
              <div className="bn-timeline-card">
                <div className="bn-flex-between">
                  <span className="bn-font-mono bn-amount-large">{formatCurrency(bid.netAmount)}</span>
                  {isLatest ? (
                    <span className="bn-badge bn-badge-live">Current Active Offer</span>
                  ) : (
                    <span className="bn-badge bn-badge-scheduled">Superseded</span>
                  )}
                </div>

                <div className="bn-timeline-meta">
                  <span className="bn-text-muted bn-text-xs">
                    Bid ID #{bid.id} • Submitted on {formatDate(bid.createdAt)}
                  </span>
                  {diffText && (
                    <span className={`bn-diff-tag ${bid.netAmount < (prevBid?.netAmount || 0) ? 'bn-text-success' : 'bn-text-accent'}`}>
                      {bid.netAmount < (prevBid?.netAmount || 0) ? <ArrowDown size={12} /> : <ArrowUp size={12} />}
                      {diffText}
                    </span>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};
