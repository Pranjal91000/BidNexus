import React from 'react';
import type { Bid } from '../../types';
import { Award, ShieldAlert, UserCheck } from 'lucide-react';

interface LeaderboardTableProps {
  bids: Bid[];
  currentUserId: number;
  isBidPriceHidden: boolean;
  isClosed?: boolean;
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
  return isNaN(d.getTime()) ? val : d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
};

export const LeaderboardTable: React.FC<LeaderboardTableProps> = ({
  bids,
  currentUserId,
  isBidPriceHidden,
  isClosed = false,
}) => {
  if (bids.length === 0) {
    return (
      <div className="bn-leaderboard-empty">
        <Award size={32} className="bn-text-muted" />
        <h4>No bids accepted in this auction yet</h4>
        <p>As vendors submit competitive bids, rankings will automatically update here in real time.</p>
      </div>
    );
  }

  return (
    <div className="bn-leaderboard-container">
      <div className="bn-table-header-kicker">
        <div>
          <span className="bn-eyebrow">REAL-TIME RANKINGS</span>
          <h4 className="bn-table-title">Live Auction Leaderboard</h4>
        </div>
        {isBidPriceHidden && (
          <span className="bn-badge bn-badge-warning">
            <ShieldAlert size={13} /> Hidden Price Mode (Ranks Visible)
          </span>
        )}
      </div>

      <div className="bn-table-responsive">
        <table className="bn-table bn-leaderboard-table">
          <thead>
            <tr>
              <th style={{ width: '80px' }}>Rank</th>
              <th>Vendor Entity</th>
              <th>Bid Amount</th>
              <th style={{ width: '100px' }}>Revision</th>
              <th>Timestamp</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {bids.map((bid, index) => {
              const rank = index + 1;
              const isCurrentUser = bid.vendorId === currentUserId;
              const isWinner = isClosed && rank === 1;

              return (
                <tr
                  key={bid.id}
                  className={`bn-leaderboard-row ${isCurrentUser ? 'bn-row-current-user' : ''} ${rank === 1 ? 'bn-row-rank-1' : ''
                    }`}
                >
                  <td>
                    <div className="bn-rank-pill">
                      {rank === 1 ? (
                        <span className="bn-rank-first">🥇 #1</span>
                      ) : rank === 2 ? (
                        <span className="bn-rank-second">🥈 #2</span>
                      ) : rank === 3 ? (
                        <span className="bn-rank-third">🥉 #3</span>
                      ) : (
                        `#${rank}`
                      )}
                    </div>
                  </td>

                  <td>
                    <div className="bn-vendor-cell">
                      <strong className="bn-vendor-name">
                        {bid.vendor?.name || `Vendor #${bid.vendorId}`}
                      </strong>
                      {isCurrentUser && (
                        <span className="bn-you-badge">
                          <UserCheck size={11} /> YOUR VENDOR ACCOUNT
                        </span>
                      )}
                    </div>
                  </td>

                  <td>
                    <span className="bn-font-mono bn-amount-text">
                      {isBidPriceHidden && !isCurrentUser
                        ? '••••••••'
                        : formatCurrency(bid.netAmount)}
                    </span>
                  </td>

                  <td>
                    <span className="bn-revision-pill">R{bid.bidRevisionNo}</span>
                  </td>

                  <td>
                    <span className="bn-timestamp-text">{formatDate(bid.createdAt)}</span>
                  </td>

                  <td>
                    {isWinner ? (
                      <span className="bn-badge bn-badge-winner">
                        <Award size={12} /> WINNER
                      </span>
                    ) : bid.isCurrent ? (
                      <span className="bn-badge bn-badge-live">Active Leader</span>
                    ) : (
                      <span className="bn-badge bn-badge-scheduled">Superseded</span>
                    )}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};
