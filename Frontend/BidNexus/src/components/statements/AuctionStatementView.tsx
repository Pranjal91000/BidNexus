import React, { useState, useEffect } from 'react';
import type { Auction, Statement, Claims, RatingSummary } from '../../types';
import { Award, CheckCircle2, FileText, RefreshCw, Trophy, Star } from 'lucide-react';
import { Button } from '../ui/Button';
import { RatingModal } from '../ratings/RatingModal';
import { api } from '../../services/api';

interface StatementViewProps {
  auction: Auction;
  statements: Statement[];
  onRefresh: () => void;
  token?: string;
  claims?: Claims;
  onShowToast?: (msg: string, tone?: 'success' | 'error' | 'info') => void;
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2,
  }).format(amount);
};

export const StatementView: React.FC<StatementViewProps> = ({
  auction,
  statements,
  onRefresh,
  token,
  claims,
  onShowToast,
}) => {
  const [ratingModalOpen, setRatingModalOpen] = useState(false);
  const [ratings, setRatings] = useState<RatingSummary[]>([]);
  const [loadingRatings, setLoadingRatings] = useState(false);

  const winner = statements.find((s) => s.isWinner || s.rank === 1);

  const isOrg = claims?.role === 'Organization';
  const isWinningVendor = claims?.role === 'Vendor' && winner && claims.userId === winner.vendorId;

  // Determine if caller has already submitted a rating for this auction
  const alreadyRated = Boolean(
    claims && ratings.some((r) => r.submittedByTenantId === claims.tenantId)
  );

  useEffect(() => {
    if (token && auction.id) {
      loadRatings();
    }
  }, [token, auction.id]);

  const loadRatings = async () => {
    if (!token) return;
    setLoadingRatings(true);
    try {
      const data = await api.getAuctionRatings(token, auction.id);
      setRatings(data || []);
    } catch {
      // Ignore if no ratings endpoint response
    } finally {
      setLoadingRatings(false);
    }
  };

  if (statements.length === 0) {
    return (
      <div className="bn-statement-empty">
        <FileText size={36} className="bn-text-muted" />
        <h4>Auction Statement Pending</h4>
        <p>
          The authoritative statement and winner award breakdown will be issued by the auction engine upon official auction closure.
        </p>
        <Button variant="outline" size="sm" icon={<RefreshCw size={14} />} onClick={onRefresh} className="bn-mt-3">
          Check Statement Status
        </Button>
      </div>
    );
  }

  return (
    <div className="bn-statement-container">
      {/* Winner Highlight Banner */}
      {winner && (
        <section className="bn-winner-banner">
          <div className="bn-winner-badge-icon">
            <Trophy size={28} />
          </div>
          <div className="bn-winner-info">
            <span className="bn-eyebrow bn-text-success">OFFICIAL AWARD WINNER</span>
            <h3 className="bn-winner-name">{winner.vendorName || `Vendor #${winner.vendorId}`}</h3>
            <p className="bn-winner-details">
              Winning Bid Amount: <strong className="bn-font-mono">{formatCurrency(winner.netAmount)}</strong> • Reference Bid #{winner.bidId}
            </p>
          </div>
          <div className="bn-flex-center gap-2">
            <div className="bn-winner-award-chip">
              <CheckCircle2 size={16} /> Verified & Awarded
            </div>

            {/* Rating Trigger Buttons */}
            {token && isOrg && (
              alreadyRated ? (
                <span className="bn-badge bn-badge-winner">
                  <CheckCircle2 size={13} /> Vendor Rated
                </span>
              ) : (
                <Button
                  variant="primary"
                  size="sm"
                  icon={<Star size={14} />}
                  onClick={() => setRatingModalOpen(true)}
                >
                  Rate Winning Vendor
                </Button>
              )
            )}

            {token && isWinningVendor && (
              alreadyRated ? (
                <span className="bn-badge bn-badge-winner">
                  <CheckCircle2 size={13} /> Org Rated
                </span>
              ) : (
                <Button
                  variant="primary"
                  size="sm"
                  icon={<Star size={14} />}
                  onClick={() => setRatingModalOpen(true)}
                >
                  Rate Organization
                </Button>
              )
            )}
          </div>
        </section>
      )}

      {/* Statement Table */}
      <div className="bn-table-header-kicker">
        <span className="bn-eyebrow">AUTHORITATIVE STATEMENT</span>
        <h4 className="bn-table-title">Final Auction Rankings & Award Breakdown</h4>
      </div>

      <div className="bn-table-responsive">
        <table className="bn-table bn-statement-table">
          <thead>
            <tr>
              <th style={{ width: '80px' }}>Rank</th>
              <th>Vendor Name</th>
              <th>Winning Net Amount</th>
              <th>Bid Reference ID</th>
              <th>Award Outcome</th>
            </tr>
          </thead>
          <tbody>
            {statements.map((stmt) => (
              <tr key={stmt.id || stmt.bidId} className={stmt.isWinner ? 'bn-row-winner' : ''}>
                <td>
                  <span className={`bn-rank-pill ${stmt.rank === 1 ? 'bn-rank-first' : ''}`}>
                    #{stmt.rank}
                  </span>
                </td>
                <td>
                  <strong>{stmt.vendorName || `Vendor #${stmt.vendorId}`}</strong>
                </td>
                <td>
                  <span className="bn-font-mono bn-font-bold">{formatCurrency(stmt.netAmount)}</span>
                </td>
                <td>
                  <span className="bn-text-mono bn-text-muted">#BID-{stmt.bidId}</span>
                </td>
                <td>
                  {stmt.isWinner || stmt.rank === 1 ? (
                    <span className="bn-badge bn-badge-winner">
                      <Award size={13} /> WINNER AWARDED
                    </span>
                  ) : (
                    <span className="bn-badge bn-badge-scheduled">Unsuccessful Bid</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Ratings Ledger Section */}
      {ratings.length > 0 && (
        <div className="bn-rating-ledger-card">
          <div className="bn-flex-between bn-mb-2">
            <div>
              <span className="bn-eyebrow">VERIFIED REPUTATION LEDGER</span>
              <h5 className="bn-table-title">Auction Performance Ratings</h5>
            </div>
            <span className="bn-badge bn-badge-scheduled">
              {loadingRatings ? 'Loading...' : `${ratings.length} Submission(s)`}
            </span>
          </div>

          <div className="bn-ratings-stream">
            {ratings.map((rating) => (
              <div key={rating.id} className="bn-rating-entry bn-flex-between">
                <div>
                  <div className="bn-flex-center gap-2">
                    <span className="bn-badge bn-badge-info">{rating.ratingForName || 'Rating'}</span>
                    <span className="bn-text-muted bn-text-xs">
                      {new Date(rating.createdAt).toLocaleDateString()}
                    </span>
                  </div>
                  {rating.remarks && (
                    <p className="bn-text-sm bn-mt-1 bn-text-muted">"{rating.remarks}"</p>
                  )}
                </div>
                <div className="bn-flex-center gap-1">
                  <Star size={15} fill="#eab308" color="#eab308" />
                  <strong>{rating.averageScore.toFixed(1)}</strong>
                  <span className="bn-text-muted bn-text-xs">/ 5.0</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Requirement Line Items Summary */}
      <div className="bn-statement-summary-box bn-mt-6">
        <h5 className="bn-summary-title">Scope & Compliance Certification</h5>
        <p className="bn-summary-desc">
          This statement reflects the final authoritative evaluations rendered by the BidNexus procurement engine for Auction #{auction.id} ({auction.docNoYearly}). All bids were evaluated under strict multi-tenant isolation.
        </p>
      </div>

      {/* Rating Modal */}
      {ratingModalOpen && token && (
        <RatingModal
          isOpen={ratingModalOpen}
          onClose={() => setRatingModalOpen(false)}
          token={token}
          auctionId={auction.id}
          auctionName={auction.auctionName || auction.docNoYearly}
          targetName={
            isOrg
              ? winner?.vendorName || `Winning Vendor #${winner?.vendorId}`
              : auction.organization?.name || 'Procurement Organization'
          }
          targetRole={isOrg ? 'Vendor' : 'Organization'}
          onRatingSubmitted={() => {
            loadRatings();
            onRefresh();
          }}
          onShowToast={onShowToast}
        />
      )}
    </div>
  );
};

