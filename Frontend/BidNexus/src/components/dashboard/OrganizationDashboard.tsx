import React from 'react';
import type { Auction, Statement } from '../../types';
import { StatCard } from '../ui/StatCard';
import { Badge, toneFromStatus } from '../ui/Badge';
import { Countdown } from '../ui/Countdown';
import { Button } from '../ui/Button';
import { Gavel, PlayCircle, Calendar, CheckCircle2, ArrowRight, Layers, AlertCircle, Award } from 'lucide-react';
import { LoadingState } from '../ui/LoadingState';
import { EmptyState } from '../ui/EmptyState';

interface OrganizationDashboardProps {
  auctions: Auction[];
  loading: boolean;
  onOpenAuction: (auction: Auction) => void;
  onCreateAuction: () => void;
  onGoToRegister: () => void;
  statements?: Statement[];
}

export const OrganizationDashboard: React.FC<OrganizationDashboardProps> = ({
  auctions,
  loading,
  onOpenAuction,
  onCreateAuction,
  onGoToRegister,
}) => {
  const liveAuctions = auctions.filter((a) => toneFromStatus(a.statusName) === 'live');
  const scheduledAuctions = auctions.filter((a) => toneFromStatus(a.statusName) === 'scheduled');
  const closedAuctions = auctions.filter((a) => toneFromStatus(a.statusName) === 'closed');

  // Attention required: Auctions closing within 24h or active with 0 bids
  const attentionRequired = auctions.filter((a) => {
    const tone = toneFromStatus(a.statusName);
    if (tone === 'live') {
      const endTime = new Date(a.auctionEndTime).getTime();
      const now = new Date().getTime();
      return endTime - now < 86400000; // closing within 24h
    }
    return false;
  });

  return (
    <div className="bn-dashboard-stack">
      {/* Hero Welcome Banner */}
      <section className="bn-hero-banner bn-hero-org">
        <div className="bn-hero-content">
          <span className="bn-eyebrow">ORGANIZATION WORKSTATION</span>
          <h2 className="bn-hero-title">Procurement Command Overview</h2>
          <p className="bn-hero-desc">
            Monitor real-time auctions, track vendor bidding activity, manage procurement requirements, and issue final award statements.
          </p>
        </div>
        <div className="bn-hero-actions">
          <Button variant="primary" size="lg" icon={<Gavel size={18} />} onClick={onCreateAuction}>
            Create New Auction
          </Button>
          <Button variant="outline" size="lg" onClick={onGoToRegister}>
            View Auction Register
          </Button>
        </div>
      </section>

      {/* Metric Stat Cards */}
      <div className="bn-stats-grid">
        <StatCard
          title="Total Auctions"
          value={auctions.length}
          icon={<Layers size={20} />}
          subtitle="All managed procurement events"
          tone="default"
        />
        <StatCard
          title="Active Live Now"
          value={liveAuctions.length}
          icon={<PlayCircle size={20} />}
          subtitle="Real-time bidding in progress"
          tone="live"
        />
        <StatCard
          title="Scheduled"
          value={scheduledAuctions.length}
          icon={<Calendar size={20} />}
          subtitle="Upcoming procurement auctions"
          tone="info"
        />
        <StatCard
          title="Completed & Awarded"
          value={closedAuctions.length}
          icon={<CheckCircle2 size={20} />}
          subtitle="Completed auction statements"
          tone="default"
        />
      </div>

      {/* Main Grid Section */}
      <div className="bn-dashboard-grid">
        {/* Active Market Column */}
        <div className="bn-dashboard-col">
          <div className="bn-section-header">
            <div>
              <span className="bn-eyebrow">LIVE MARKET</span>
              <h3 className="bn-section-title">Active Procurement Auctions</h3>
            </div>
            <Button variant="ghost" size="sm" onClick={onGoToRegister} icon={<ArrowRight size={14} />}>
              View All
            </Button>
          </div>

          {loading ? (
            <LoadingState message="Loading live market data..." />
          ) : liveAuctions.length > 0 ? (
            <div className="bn-auction-cards-list">
              {liveAuctions.slice(0, 4).map((auction) => (
                <div key={auction.id} className="bn-dashboard-card" onClick={() => onOpenAuction(auction)}>
                  <div className="bn-card-header">
                    <div className="bn-flex-center gap-2">
                      <Badge tone={auction.statusName}>{auction.statusName || 'Live'}</Badge>
                      <Countdown endTime={auction.auctionEndTime} startTime={auction.auctionStartTime} compact />
                    </div>
                    <span className="bn-auction-id">#{auction.id}</span>
                  </div>
                  <h4 className="bn-card-title">{auction.auctionName || auction.docNoYearly}</h4>
                  <p className="bn-card-meta">
                    {auction.docNoYearly} • {auction.isForwardAuction ? 'Forward Auction' : 'Reverse Auction'}
                  </p>
                  <div className="bn-card-footer">
                    <span className="bn-req-count">
                      {auction.auctionRequirements?.length || 0} line requirements
                    </span>
                    <Button variant="outline" size="sm">
                      Open Live Workstation →
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState
              title="No active live auctions"
              description="Auctions marked live by the procurement engine will appear here in real time."
              actionText="Create an Auction"
              onAction={onCreateAuction}
            />
          )}
        </div>

        {/* Attention & Recent Summary Column */}
        <div className="bn-dashboard-col">
          <div className="bn-section-header">
            <div>
              <span className="bn-eyebrow">ACTIONABLE ITEMS</span>
              <h3 className="bn-section-title">Requires Attention</h3>
            </div>
          </div>

          {attentionRequired.length > 0 ? (
            <div className="bn-attention-list">
              {attentionRequired.map((auction) => (
                <div key={auction.id} className="bn-attention-item" onClick={() => onOpenAuction(auction)}>
                  <AlertCircle size={18} className="bn-text-warning" />
                  <div className="bn-attention-info">
                    <strong>{auction.auctionName || auction.docNoYearly}</strong>
                    <small>Closing within 24 hours. Monitor active bids.</small>
                  </div>
                  <Button variant="ghost" size="sm">Review →</Button>
                </div>
              ))}
            </div>
          ) : (
            <div className="bn-attention-empty">
              <CheckCircle2 size={24} className="bn-text-success" />
              <p>All active auctions are operating smoothly within normal parameters.</p>
            </div>
          )}

          <div className="bn-section-header bn-mt-6">
            <div>
              <span className="bn-eyebrow">RECENTLY COMPLETED</span>
              <h3 className="bn-section-title">Completed Auctions</h3>
            </div>
          </div>

          {closedAuctions.length > 0 ? (
            <div className="bn-recent-closed-list">
              {closedAuctions.slice(0, 3).map((auction) => (
                <div key={auction.id} className="bn-closed-item" onClick={() => onOpenAuction(auction)}>
                  <Award size={18} className="bn-text-accent" />
                  <div>
                    <strong>{auction.auctionName || auction.docNoYearly}</strong>
                    <small>Doc #{auction.docNoYearly}</small>
                  </div>
                  <Badge tone="closed">Completed</Badge>
                </div>
              ))}
            </div>
          ) : (
            <p className="bn-text-muted bn-text-sm">No completed auctions recorded in this tenant scope yet.</p>
          )}
        </div>
      </div>
    </div>
  );
};
