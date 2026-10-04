import { useEffect, useRef } from 'react';
import { ArrowLeft, Pencil, RefreshCw } from 'lucide-react';
import { useApp } from '../../../lib/appContext';
import { useNow } from '../../../lib/hooks';
import { acceptsBids, auctionTitle, phaseOf, typeLabel } from '../../../lib/auction';
import { dateTime } from '../../../lib/format';
import { Button } from '../../../components/ui/Button';
import { PhaseBadge } from '../../../components/ui/Badge';
import { BigTimer } from '../../../components/ui/Countdown';
import { Callout, Loading } from '../../../components/ui/States';
import { useAuctionData } from './useAuctionData';
import { Facts, ItemsList } from './Blocks';
import { OrgLive } from './OrgLive';
import { OrgStatement } from './OrgStatement';
import { VendorLive, VendorResult } from './VendorViews';

export function AuctionDetail({ auctionId }: { auctionId: number }) {
  const { isOrg, auctions, navigate, openAuctionForm } = useApp();
  const now = useNow(1000);
  const data = useAuctionData(auctionId);
  const { auction, loading, error, reload } = data;

  // When the list reloads (e.g. after an edit), refresh this page too.
  const first = useRef(true);
  useEffect(() => {
    if (first.current) {
      first.current = false;
      return;
    }
    reload();
  }, [auctions, reload]);

  const back = (
    <button type="button" className="back-link no-print" onClick={() => navigate({ page: 'auctions' })}>
      <ArrowLeft size={18} />
      Auctions
    </button>
  );

  if (loading && !auction) {
    return <main className="page">{back}<Loading /></main>;
  }
  if (!auction) {
    return (
      <main className="page">
        {back}
        <Callout tone="error">{error || 'This auction could not be found.'}</Callout>
      </main>
    );
  }

  const phase = phaseOf(auction, now);
  const canEdit = isOrg && (phase === 'draft' || phase === 'upcoming');

  let body;
  if (phase === 'draft' || phase === 'upcoming') {
    body = (
      <>
        {phase === 'draft' && <Callout>This is a draft. Vendors can't see it until you publish it.</Callout>}
        {phase === 'upcoming' && !isOrg && <Callout tone="info">Bidding opens {dateTime(auction.auctionStartTime)}. You can review the items now.</Callout>}
        <Facts auction={auction} />
        <ItemsList auction={auction} />
      </>
    );
  } else if (phase === 'closing') {
    body = (
      <>
        <Callout>
          <span className="grow">Bidding has ended. Results are being prepared and will appear here automatically.</span>
          <button type="button" className="link-btn" onClick={() => reload()}>Check now</button>
        </Callout>
        {isOrg && <OrgLive auction={auction} leaderboard={data.leaderboard} activity={data.activity} />}
      </>
    );
  } else if (phase === 'live') {
    body = isOrg ? (
      <OrgLive auction={auction} leaderboard={data.leaderboard} activity={data.activity} />
    ) : (
      <VendorLive
        auction={auction}
        leaderboard={data.leaderboard}
        history={data.history}
        activity={data.activity}
        canBid={acceptsBids(auction, now)}
        onChanged={reload}
      />
    );
  } else {
    body = isOrg ? (
      <OrgStatement auction={auction} statement={data.statement} activity={data.activity} />
    ) : (
      <VendorResult auction={auction} result={data.myResult} />
    );
  }

  const meta = [auction.docNoYearly, typeLabel(auction), !isOrg ? auction.organization?.name : null].filter(Boolean).join(' · ');

  return (
    <main className="page">
      {back}

      <div className="page-header">
        <div className="page-header__text">
          <div className="row" style={{ gap: 8 }}>
            <PhaseBadge phase={phase} />
            <span className="small muted">{meta}</span>
          </div>
          <h1>{auctionTitle(auction)}</h1>
          {auction.about && <p className="muted" style={{ maxWidth: 720 }}>{auction.about}</p>}
        </div>

        <div className="row no-print">
          {phase === 'live' && <BigTimer to={auction.auctionEndTime} />}
          {phase === 'closed' && <span className="small muted">Closed {dateTime(auction.auctionEndTime)}</span>}
          {canEdit && (
            <Button icon={<Pencil size={16} />} onClick={() => openAuctionForm(auction)}>
              Edit
            </Button>
          )}
          {phase !== 'live' && (
            <button type="button" className="icon-btn" aria-label="Refresh" onClick={() => reload()}>
              <RefreshCw size={16} />
            </button>
          )}
        </div>
      </div>

      {phase === 'live' && (data.connection === 'RECONNECTING' || data.connection === 'DISCONNECTED') && (
        <Callout tone="warn">
          <span className="grow">Live updates are reconnecting. Numbers may be a few seconds behind.</span>
          <button type="button" className="link-btn" onClick={() => reload()}>Refresh now</button>
        </Callout>
      )}

      {body}
    </main>
  );
}
