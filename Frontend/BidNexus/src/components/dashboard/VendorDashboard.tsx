import React from 'react';
import type { Auction } from '../../types';
import { StatCard } from '../ui/StatCard';
import { Badge, toneFromStatus } from '../ui/Badge';
import { Button } from '../ui/Button';
import { Gavel, TrendingUp, Clock, CheckCircle2, ArrowRight, Store, Zap } from 'lucide-react';
import { LoadingState } from '../ui/LoadingState';
import { EmptyState } from '../ui/EmptyState';

interface VendorDashboardProps {
  auctions: Auction[];
  loading: boolean;
  onOpenAuction: (auction: Auction) => void;
  onGoToRegister: () => void;
  onGoToMyBids: () => void;
}

export const VendorDashboard: React.FC<VendorDashboardProps> = ({
  auctions,
  loading,
  onOpenAuction,
  onGoToRegister,
  onGoToMyBids,
}) => {
  const liveAuctions = auctions.filter((a) => toneFromStatus(a.statusName) === 'live');
  const availableAuctions = auctions.filter((a) => toneFromStatus(a.statusName) !== 'closed');
  const endingSoon = liveAuctions.filter((a) => {
    const end = new Date(a.auctionEndTime).getTime();
    const now = new Date().getTime();
    return end - now < 3600000; // < 1 hour
  });

  return (
    <div className="bn-dashboard-stack">
      {/* Hero Welcome Banner */}
      <section className="bn-hero-banner bn-hero-vendor">
        <div className="bn-hero-content">
          <span className="bn-eyebrow">VENDOR BIDDING WORKSTATION</span>
          <h2 className="bn-hero-title">Market Opportunities & Active Bids</h2>
          <p className="bn-hero-desc">
            Participate in live procurement auctions, submit technical line items, monitor real-time leaderboard ranks, and optimize your competitive pricing.
          </p>
        </div>
        <div className="bn-hero-actions">
          <Button variant="primary" size="lg" icon={<Zap size={18} />} onClick={onGoToRegister}>
            Explore Live Market
          </Button>
          <Button variant="outline" size="lg" icon={<TrendingUp size={18} />} onClick={onGoToMyBids}>
            Review My Bids
          </Button>
        </div>
      </section>

      {/* Metric Stat Cards */}
      <div className="bn-stats-grid">
        <StatCard
          title="Available Opportunities"
          value={availableAuctions.length}
          icon={<Gavel size={20} />}
          subtitle="Auctions accepting bids or upcoming"
          tone="default"
        />
        <StatCard
          title="Live Auctions Now"
          value={liveAuctions.length}
          icon={<Zap size={20} />}
          subtitle="Real-time bidding currently open"
          tone="live"
        />
        <StatCard
          title="Closing Soon"
          value={endingSoon.length}
          icon={<Clock size={20} />}
          subtitle="Ending within 60 minutes"
          tone="warning"
        />
        <StatCard
          title="Participated Auctions"
          value={auctions.length}
          icon={<Store size={20} />}
          subtitle="Total marketplace reach"
          tone="info"
        />
      </div>

      {/* Main Grid Section */}
      <div className="bn-dashboard-grid">
        {/* Available Live Auctions Column */}
        <div className="bn-dashboard-col">
          <div className="bn-section-header">
            <div>
              <span className="bn-eyebrow">LIVE BIDDING WORKSTATION</span>
              <h3 className="bn-section-title">Active Market Auctions</h3>
            </div>
            <Button variant="ghost" size="sm" onClick={onGoToRegister} icon={<ArrowRight size={14} />}>
              All Auctions
            </Button>
          </div>

          {loading ? (
            <LoadingState message="Fetching market auctions..." />
          ) : liveAuctions.length > 0 ? (
            <div className="bn-auction-cards-list">
              {liveAuctions.slice(0, 4).map((auction) => (
                <div key={auction.id} className="bn-dashboard-card" onClick={() => onOpenAuction(auction)}>
                  <div className="bn-card-header">
                    <Badge tone={auction.statusName}>{auction.statusName || 'Live'}</Badge>
                    <span className="bn-auction-type-chip">
                      {auction.isForwardAuction ? 'FORWARD' : 'REVERSE'} AUCTION
                    </span>
                  </div>
                  <h4 className="bn-card-title">{auction.auctionName || auction.docNoYearly}</h4>
                  <p className="bn-card-org">{auction.organization?.name || auction.docNoYearly}</p>
                  <div className="bn-card-footer">
                    <span className="bn-req-count">
                      {auction.auctionRequirements?.length || 0} line items
                    </span>
                    <Button variant="primary" size="sm">
                      Place Bid →
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState
              title="No live auctions right now"
              description="Check the auction register for upcoming scheduled procurement events."
              actionText="Browse Register"
              onAction={onGoToRegister}
            />
          )}
        </div>

        {/* Ending Soon & Market Information */}
        <div className="bn-dashboard-col">
          <div className="bn-section-header">
            <div>
              <span className="bn-eyebrow">HIGH PRIORITY</span>
              <h3 className="bn-section-title">Ending Soon</h3>
            </div>
          </div>

          {endingSoon.length > 0 ? (
            <div className="bn-attention-list">
              {endingSoon.map((auction) => (
                <div key={auction.id} className="bn-attention-item" onClick={() => onOpenAuction(auction)}>
                  <Clock size={18} className="bn-text-warning bn-pulse-slow" />
                  <div className="bn-attention-info">
                    <strong>{auction.auctionName || auction.docNoYearly}</strong>
                    <small>Final minutes. Enter workstation to update bid.</small>
                  </div>
                  <Button variant="primary" size="sm">Enter Desk</Button>
                </div>
              ))}
            </div>
          ) : (
            <div className="bn-attention-empty">
              <CheckCircle2 size={24} className="bn-text-info" />
              <p>No active auctions closing in the immediate hour.</p>
            </div>
          )}

          <div className="bn-section-header bn-mt-6">
            <div>
              <span className="bn-eyebrow">BIDDING COMPLIANCE</span>
              <h3 className="bn-section-title">Engine Guidelines</h3>
            </div>
          </div>

          <div className="bn-guidelines-box">
            <ul>
              <li><strong>Authoritative Validation:</strong> All bid amounts and calculations are verified server-side.</li>
              <li><strong>Revision Control:</strong> Each submitted bid increments revision number (R1, R2, ...).</li>
              <li><strong>Real-time SignalR:</strong> Leaderboard updates instantly without page refresh.</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};
