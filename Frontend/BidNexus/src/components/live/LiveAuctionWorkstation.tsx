import React, { useState, useEffect, useCallback } from 'react';
import type { Auction, Bid, Claims, Statement, BidDetailSaveRequest } from '../../types';
import { Badge, toneFromStatus } from '../ui/Badge';
import { Button } from '../ui/Button';
import { Countdown } from '../ui/Countdown';
import { Tabs } from '../ui/Tabs';
import { BiddingPanel } from './BiddingPanel';
import { LeaderboardTable } from './LeaderboardTable';
import { BidHistoryTimeline } from './BidHistoryTimeline';
import { RequirementListTable } from './RequirementListTable';
import { useAuctionSignalR, type BidAcceptedPayload } from '../../services/signalr';
import { api } from '../../services/api';
import { ArrowLeft, RefreshCw, Award, Layers, Trophy, Clock, Wifi } from 'lucide-react';
import { LoadingState } from '../ui/LoadingState';
import { StatementView } from '../statements/AuctionStatementView';

interface LiveAuctionWorkstationProps {
  auction: Auction;
  token: string;
  claims: Claims;
  onBack: () => void;
  onShowToast: (message: string, type?: 'success' | 'error' | 'warning' | 'info') => void;
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

export const LiveAuctionWorkstation: React.FC<LiveAuctionWorkstationProps> = ({
  auction,
  token,
  claims,
  onBack,
  onShowToast,
}) => {
  const [activeTab, setActiveTab] = useState<string>('leaderboard');
  const [leaderboard, setLeaderboard] = useState<Bid[]>([]);
  const [bidHistory, setBidHistory] = useState<Bid[]>([]);
  const [statements, setStatements] = useState<Statement[]>([]);

  const [loadingData, setLoadingData] = useState<boolean>(true);
  const [submittingBid, setSubmittingBid] = useState<boolean>(false);
  const [auctionState, setAuctionState] = useState<Auction>(auction);

  const isVendor = claims.role === 'Vendor';
  const isOrg = claims.role === 'Organization';
  const statusTone = toneFromStatus(auctionState.statusName);
  const isLive = statusTone === 'live' || auctionState.statusName?.toLowerCase() === 'open';
  const isClosed = statusTone === 'closed' || auctionState.statusName?.toLowerCase() === 'completed';

  // Realtime SignalR Handlers
  const handleBidAcceptedSignalR = useCallback(
    (payload: BidAcceptedPayload) => {
      onShowToast(
        `New bid of ${payload.NetAmount ? formatCurrency(payload.NetAmount) : 'amount'} accepted in this auction!`,
        'info'
      );
      loadAuctionData(false);
    },
    [onShowToast]
  );

  const handleAuctionStartedSignalR = useCallback(
    () => {
      onShowToast('This auction has started and is now open for live bidding!', 'success');
      setAuctionState((prev) => ({ ...prev, statusName: 'Open' }));
      loadAuctionData(false);
    },
    [onShowToast]
  );

  const handleAuctionCompletedSignalR = useCallback(
    () => {
      onShowToast('This auction has been officially completed by the procurement engine.', 'warning');
      setAuctionState((prev) => ({ ...prev, statusName: 'Completed' }));
      loadAuctionData(false);
    },
    [onShowToast]
  );

  // Hook SignalR
  const { status: signalRStatus } = useAuctionSignalR(
    token,
    auctionState.id,
    handleBidAcceptedSignalR,
    handleAuctionCompletedSignalR,
    handleAuctionCompletedSignalR,
    handleAuctionStartedSignalR
  );

  const loadAuctionData = async (showLoading = true) => {
    if (showLoading) setLoadingData(true);
    try {
      // 1. Load Leaderboard
      const bids = await api.getLeaderboard(token, auctionState.id);
      setLeaderboard(bids || []);

      // 2. Load Vendor Bid History if Vendor
      if (isVendor && claims.userId) {
        const history = await api.getBidHistory(token, auctionState.id, claims.userId);
        setBidHistory(history || []);
      }

      // 3. Load Statement if Org or Closed
      if (isOrg || isClosed) {
        try {
          const stmts = await api.getAuctionStatement(token, auctionState.id);
          setStatements(stmts || []);
        } catch {
          // statement might not exist until closed
        }
      }
    } catch (err: any) {
      console.warn('Failed to load auction market data', err);
    } finally {
      if (showLoading) setLoadingData(false);
    }
  };

  useEffect(() => {
    loadAuctionData(true);
  }, [auctionState.id, token, claims.userId]);

  // Derive current vendor stats
  const currentVendorBid = isVendor
    ? leaderboard.find((b) => b.vendorId === claims.userId) || (bidHistory.length > 0 ? bidHistory[0] : null)
    : null;

  const currentRank = isVendor && currentVendorBid
    ? leaderboard.findIndex((b) => b.vendorId === claims.userId) + 1 || null
    : null;

  const leadingBidAmount = leaderboard.length > 0 ? leaderboard[0].netAmount : null;

  // Submit Bid Logic
  const handleSubmitBid = async (bidData: {
    totalNet: number;
    totalBasic: number;
    totalTax: number;
    bidDetails: BidDetailSaveRequest[];
  }) => {
    setSubmittingBid(true);
    try {
      const currentRevNo = currentVendorBid ? currentVendorBid.bidRevisionNo + 1 : 1;

      await api.submitBid(token, {
        auctionId: auctionState.id,
        vendorId: claims.userId,
        basicAmount: bidData.totalBasic,
        taxAmount: bidData.totalTax,
        discountAmount: 0,
        netAmount: bidData.totalNet,
        mainBidId: currentVendorBid ? currentVendorBid.id : null,
        bidRevisionNo: currentRevNo,
        bidDetails: bidData.bidDetails,
      });

      onShowToast(`Bid of ${formatCurrency(bidData.totalNet)} successfully accepted!`, 'success');
      await loadAuctionData(false);
    } catch (err: any) {
      throw err;
    } finally {
      setSubmittingBid(false);
    }
  };

  const tabsList = [
    { id: 'leaderboard', label: 'Live Market Leaderboard', count: leaderboard.length, icon: <Trophy size={15} /> },
    { id: 'requirements', label: 'Line Requirements', count: auctionState.auctionRequirements?.length || 0, icon: <Layers size={15} /> },
  ];

  if (isVendor) {
    tabsList.push({ id: 'history', label: 'My Bid Revisions', count: bidHistory.length, icon: <Clock size={15} /> });
  }

  if (isOrg || isClosed) {
    tabsList.push({ id: 'statement', label: 'Auction Statement', count: statements.length, icon: <Award size={15} /> });
  }

  return (
    <div className="bn-live-workstation-stack">
      {/* Workstation Header */}
      <section className="bn-workstation-hero">
        <div className="bn-hero-top-row">
          <Button variant="outline" size="sm" icon={<ArrowLeft size={16} />} onClick={onBack}>
            Back to Register
          </Button>

          <div className="bn-flex-center gap-2">
            <Badge tone={auctionState.statusName}>{auctionState.statusName || 'Scheduled'}</Badge>
            <span className="bn-type-badge">
              {auctionState.isForwardAuction ? 'FORWARD AUCTION' : 'REVERSE AUCTION'}
            </span>
            <span className={`bn-signalr-pill bn-signalr-${signalRStatus.toLowerCase()}`}>
              <Wifi size={12} /> {signalRStatus}
            </span>
          </div>
        </div>

        <div className="bn-hero-main-title">
          <div className="bn-hero-left">
            <span className="bn-auction-code">DOC #{auctionState.docNoYearly} • AUCTION #{auctionState.id}</span>
            <h2 className="bn-title-text">{auctionState.auctionName || auctionState.docNoYearly}</h2>
            <p className="bn-subtitle-text">{auctionState.about || auctionState.organization?.name || 'B2B Procurement Event'}</p>
          </div>

          <div className="bn-hero-right-timer">
            <Countdown endTime={auctionState.auctionEndTime} startTime={auctionState.auctionStartTime} size="lg" />
            <div className="bn-time-bounds">
              <span>Start: {formatDate(auctionState.auctionStartTime)}</span>
              <span>End: {formatDate(auctionState.auctionEndTime)}</span>
            </div>
          </div>
        </div>
      </section>

      {/* Vendor Bidding Panel (Shown when user is Vendor & Auction is Live) */}
      {isVendor && (
        <BiddingPanel
          auction={auctionState}
          currentVendorBid={currentVendorBid}
          currentRank={currentRank}
          leadingBidAmount={leadingBidAmount}
          onSubmitBid={handleSubmitBid}
          submitting={submittingBid}
          token={token}
          disabledReason={!isLive ? `Bidding is disabled because this auction is ${auctionState.statusName || 'Closed'}.` : undefined}
        />
      )}

      {/* Interactive Tabs */}
      <div className="bn-workstation-tabs-bar">
        <Tabs tabs={tabsList} activeTab={activeTab} onChange={setActiveTab} />
        <button className="bn-icon-btn" onClick={() => loadAuctionData(false)} title="Refresh market rankings">
          <RefreshCw size={15} />
        </button>
      </div>

      {/* Tab Panels */}
      <div className="bn-tab-panel-content">
        {loadingData ? (
          <LoadingState message="Fetching live market rankings and bid logs..." />
        ) : (
          <>
            {activeTab === 'leaderboard' && (
              <LeaderboardTable
                bids={leaderboard}
                currentUserId={claims.userId}
                isBidPriceHidden={Boolean(auctionState.isBidPriceHidden)}
                isClosed={isClosed}
              />
            )}

            {activeTab === 'requirements' && (
              <RequirementListTable requirements={auctionState.auctionRequirements} />
            )}

            {activeTab === 'history' && isVendor && (
              <BidHistoryTimeline history={bidHistory} isForward={Boolean(auctionState.isForwardAuction)} />
            )}

            {activeTab === 'statement' && (
              <StatementView
                auction={auctionState}
                statements={statements}
                onRefresh={() => loadAuctionData(false)}
                token={token}
                claims={claims}
                onShowToast={onShowToast}
              />
            )}
          </>
        )}
      </div>
    </div>
  );
};
