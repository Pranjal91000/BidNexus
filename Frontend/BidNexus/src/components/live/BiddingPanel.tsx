import React, { useState } from 'react';
import type { Auction, Bid } from '../../types';
import { Button } from '../ui/Button';
import { TrendingDown, TrendingUp, AlertTriangle, CheckCircle2, ShieldCheck, Zap } from 'lucide-react';

interface BiddingPanelProps {
  auction: Auction;
  currentVendorBid: Bid | null;
  currentRank: number | null;
  leadingBidAmount: number | null;
  onSubmitBid: (amount: number) => Promise<void>;
  submitting: boolean;
  disabledReason?: string | null;
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2,
  }).format(amount);
};

export const BiddingPanel: React.FC<BiddingPanelProps> = ({
  auction,
  currentVendorBid,
  currentRank,
  leadingBidAmount,
  onSubmitBid,
  submitting,
  disabledReason,
}) => {
  const [bidAmountInput, setBidAmountInput] = useState<string>('');
  const [validationError, setValidationError] = useState<string>('');
  const [bidStatusNotice, setBidStatusNotice] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  const isForward = Boolean(auction.isForwardAuction);

  const handleQuickDecrement = (pct: number) => {
    const base = currentVendorBid ? currentVendorBid.netAmount : leadingBidAmount || 1000000;
    const delta = (base * pct) / 100;
    const newVal = isForward ? base + delta : Math.max(1, base - delta);
    setBidAmountInput(newVal.toFixed(2));
    setValidationError('');
  };

  const validateBid = (val: number): boolean => {
    if (isNaN(val) || val <= 0) {
      setValidationError('Enter a valid positive bid amount.');
      return false;
    }

    if (currentVendorBid) {
      if (!isForward && val >= currentVendorBid.netAmount) {
        setValidationError(`In a Reverse Auction, your new bid must be LOWER than your current bid (${formatCurrency(currentVendorBid.netAmount)}).`);
        return false;
      }
      if (isForward && val <= currentVendorBid.netAmount) {
        setValidationError(`In a Forward Auction, your new bid must be HIGHER than your current bid (${formatCurrency(currentVendorBid.netAmount)}).`);
        return false;
      }
    } else if (leadingBidAmount) {
      if (!isForward && val > leadingBidAmount) {
        setValidationError(`Suggested starting bid should be lower than current leading market bid (${formatCurrency(leadingBidAmount)}).`);
        // allow submission, but warn
      }
    }

    setValidationError('');
    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setBidStatusNotice(null);
    const amount = Number(bidAmountInput);

    if (!validateBid(amount)) return;

    try {
      await onSubmitBid(amount);
      setBidStatusNotice({ type: 'success', message: 'Bid accepted by procurement auction engine!' });
      setBidAmountInput('');
    } catch (err: any) {
      setBidStatusNotice({
        type: 'error',
        message: err instanceof Error ? err.message : 'Bid rejected by authoritative server.',
      });
    }
  };

  return (
    <div className="bn-bidding-workstation-panel">
      <div className="bn-panel-header-kicker">
        <span className="bn-eyebrow">BIDDING COMMAND PANEL</span>
        <span className="bn-auction-direction-tag">
          {isForward ? <TrendingUp size={14} /> : <TrendingDown size={14} />}
          {isForward ? 'FORWARD AUCTION (Highest Bid Wins)' : 'REVERSE AUCTION (Lowest Offer Wins)'}
        </span>
      </div>

      {/* Primary Status Banner */}
      <div className="bn-bidding-status-banner">
        <div className="bn-bid-stat-box">
          <span className="bn-stat-label">YOUR CURRENT BID</span>
          <span className="bn-stat-value-large bn-font-mono">
            {currentVendorBid ? formatCurrency(currentVendorBid.netAmount) : '— No Active Bid'}
          </span>
          {currentVendorBid && (
            <span className="bn-stat-subtext">Revision R{currentVendorBid.bidRevisionNo}</span>
          )}
        </div>

        <div className="bn-bid-stat-box bn-border-left">
          <span className="bn-stat-label">CURRENT POSITION / RANK</span>
          <span className={`bn-stat-value-large ${currentRank === 1 ? 'bn-text-success' : 'bn-text-warning'}`}>
            {currentRank ? `#${currentRank}` : '—'}
          </span>
          <span className="bn-stat-subtext">
            {currentRank === 1 ? '🥇 Leading Market Position' : currentRank ? `Rank ${currentRank} in auction` : 'Awaiting first revision'}
          </span>
        </div>

        {leadingBidAmount !== null && (
          <div className="bn-bid-stat-box bn-border-left">
            <span className="bn-stat-label">MARKET LEADING BID</span>
            <span className="bn-stat-value-large bn-font-mono bn-text-info">
              {auction.isBidPriceHidden ? 'Hidden Price' : formatCurrency(leadingBidAmount)}
            </span>
            <span className="bn-stat-subtext">Best submitted offer</span>
          </div>
        )}
      </div>

      {/* Input Action Console */}
      {disabledReason ? (
        <div className="bn-bidding-disabled-notice">
          <AlertTriangle size={20} />
          <span>{disabledReason}</span>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="bn-bid-console-form">
          {bidStatusNotice && (
            <div className={`bn-bid-notice bn-bid-notice-${bidStatusNotice.type}`}>
              {bidStatusNotice.type === 'success' ? <CheckCircle2 size={16} /> : <AlertTriangle size={16} />}
              <span>{bidStatusNotice.message}</span>
            </div>
          )}

          <div className="bn-bid-input-group">
            <label htmlFor="new-bid-amount-input" className="bn-label bn-text-sm">ENTER NEW REVISED BID AMOUNT (₹)</label>
            <div className="bn-currency-input-wrapper">
              <span className="bn-currency-symbol">₹</span>
              <input
                id="new-bid-amount-input"
                type="number"
                step="any"
                min="0.01"
                className="bn-bid-big-input"
                placeholder="0.00"
                value={bidAmountInput}
                onChange={(e) => {
                  setBidAmountInput(e.target.value);
                  setValidationError('');
                }}
                disabled={submitting}
                autoFocus
              />
            </div>
            {validationError && <p className="bn-error-msg bn-mt-1">{validationError}</p>}
          </div>

          {/* Quick Adjustment Shortcuts */}
          <div className="bn-quick-deltas">
            <span className="bn-text-xs bn-text-muted">QUICK STEP ADJUSTMENT:</span>
            <button type="button" className="bn-quick-btn" onClick={() => handleQuickDecrement(1)}>
              {isForward ? '+1%' : '-1%'}
            </button>
            <button type="button" className="bn-quick-btn" onClick={() => handleQuickDecrement(2.5)}>
              {isForward ? '+2.5%' : '-2.5%'}
            </button>
            <button type="button" className="bn-quick-btn" onClick={() => handleQuickDecrement(5)}>
              {isForward ? '+5%' : '-5%'}
            </button>
          </div>

          {/* Primary CTA Button */}
          <Button
            type="submit"
            variant="primary"
            size="lg"
            className="bn-w-full bn-bid-submit-btn"
            loading={submitting}
            icon={<Zap size={20} />}
          >
            {submitting ? 'VALIDATING & SUBMITTING BID...' : 'PLACE REVISED BID →'}
          </Button>

          <div className="bn-security-footer">
            <ShieldCheck size={13} />
            <span>Server-side validation enforces competitive price improvement rules in real-time.</span>
          </div>
        </form>
      )}
    </div>
  );
};
