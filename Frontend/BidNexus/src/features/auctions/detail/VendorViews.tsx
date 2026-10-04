import { useState } from 'react';
import { ChevronRight } from 'lucide-react';
import type { Auction, Bid, BidActivity, VendorAuctionResult } from '../../../types';
import { useApp } from '../../../lib/appContext';
import { inr, inrRate, qty, time } from '../../../lib/format';
import { Button } from '../../../components/ui/Button';
import { Callout, Empty } from '../../../components/ui/States';
import { RatingModal } from '../../ratings/RatingModal';
import { BidForm } from './BidForm';
import { ItemsList, PriceTrend } from './Blocks';

interface LiveProps {
  auction: Auction;
  leaderboard: Bid[];
  history: Bid[];
  activity: BidActivity[];
  canBid: boolean;
  onChanged: () => void;
}

/** A vendor's live screen: where you stand, the trend, and the bid form. */
export function VendorLive({ auction, leaderboard, history, activity, canBid, onChanged }: LiveProps) {
  const { claims } = useApp();
  const forward = !!auction.isForwardAuction;
  const myIndex = leaderboard.findIndex((b) => b.vendorId === claims.userId);
  const rank = myIndex >= 0 ? myIndex + 1 : null;
  const current = history.find((h) => h.isCurrent) ?? history[0] ?? null;
  const others = leaderboard.filter((b) => b.vendorId !== claims.userId);
  const pricesHidden = others.some((b) => b.amountHidden) || !!auction.isBidPriceHidden;
  const otherAmounts = pricesHidden ? [] : others.map((b) => b.netAmount);
  const leader = leaderboard[0];

  let title = 'You have not bid yet';
  let sub = `${leaderboard.length} vendor${leaderboard.length === 1 ? ' has' : 's have'} bid so far.`;
  if (rank === 1) {
    title = 'You are leading';
    const next = otherAmounts.length ? (forward ? Math.max(...otherAmounts) : Math.min(...otherAmounts)) : null;
    sub = next != null && current ? `Next bid is ${inr(Math.abs(next - current.netAmount))} ${forward ? 'lower' : 'higher'}.` : `${leaderboard.length} bidder${leaderboard.length === 1 ? '' : 's'}.`;
  } else if (rank) {
    title = `You are ${rank} of ${leaderboard.length}`;
    sub = pricesHidden || !leader || !current
      ? 'Bid amounts are hidden in this auction.'
      : `${forward ? 'Highest' : 'Lowest'} bid ${inr(leader.netAmount)} · you are ${inr(Math.abs(current.netAmount - leader.netAmount))} ${forward ? 'lower' : 'higher'}.`;
  } else if (!pricesHidden && leader) {
    sub = `${forward ? 'Highest' : 'Lowest'} bid so far is ${inr(leader.netAmount)}.`;
  }

  const tone = rank === 1 ? 'position--leading' : rank ? 'position--behind' : '';

  return (
    <>
      <section className={`card position ${tone}`}>
        <span className="position__rank num">{rank ? `L${rank}` : '—'}</span>
        <div className="stack-sm grow" style={{ gap: 2, flexBasis: 260 }}>
          <span style={{ fontSize: 'var(--text-lg)', fontWeight: 600 }}>{title}</span>
          <span className="muted" aria-live="polite">{sub}</span>
        </div>
        {current && (
          <div className="stack-sm right" style={{ gap: 2 }}>
            <span className="small muted">Your current bid</span>
            <span className="num" style={{ fontSize: 'var(--text-lg)', fontWeight: 600 }}>{inr(current.netAmount)}</span>
          </div>
        )}
      </section>

      {!canBid && <Callout tone="info">Bidding opens as soon as the auction is marked live. This page updates by itself.</Callout>}

      <BidForm auction={auction} currentBid={current} otherAmounts={otherAmounts} disabled={!canBid} onSubmitted={onChanged} />

      <PriceTrend auction={auction} activity={activity} showMine />

      {history.length > 0 && (
        <details className="disclosure">
          <summary>
            <ChevronRight size={18} />
            Your bids ({history.length})
          </summary>
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Revision</th>
                  <th>Time</th>
                  <th className="num">Net amount</th>
                </tr>
              </thead>
              <tbody>
                {history.map((h) => (
                  <tr key={h.id}>
                    <td>R{h.bidRevisionNo}{h.isCurrent && <span className="small accent"> · current</span>}</td>
                    <td className="muted">{time(h.createdAt)}</td>
                    <td className="num strong">{inr(h.netAmount)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </details>
      )}

      <ItemsList auction={auction} collapsible />
    </>
  );
}

/** A vendor's own outcome once the auction has closed. Never shows competitors. */
export function VendorResult({ auction, result }: { auction: Auction; result: VendorAuctionResult | null }) {
  const [showLines, setShowLines] = useState(false);
  const [rating, setRating] = useState(false);
  const [rated, setRated] = useState(false);
  const forward = !!auction.isForwardAuction;

  if (!result) {
    return (
      <div className="card">
        <Empty title="Result not available">We couldn't load your result. Try again in a moment.</Empty>
      </div>
    );
  }

  if (!result.participated) {
    return (
      <div className="card">
        <Empty title="You didn't bid in this auction">{result.bidders} vendor{result.bidders === 1 ? '' : 's'} took part.</Empty>
      </div>
    );
  }

  const gap = result.winningAmount != null && result.myNetAmount != null ? Math.abs(result.myNetAmount - result.winningAmount) : null;

  return (
    <>
      <section className="card card--pad stack-lg" style={{ gap: 24 }}>
        <div className="stack-sm">
          <span className={`small strong ${result.isWinner ? 'accent' : 'muted'}`}>{result.isWinner ? 'Awarded to you' : 'Not awarded'}</span>
          <span style={{ fontSize: 'var(--text-xl)', fontWeight: 600 }}>
            {result.isWinner ? 'You won this auction' : `You finished L${result.rank} of ${result.bidders}`}
          </span>
          <span className="muted">
            {result.isWinner
              ? `${auction.organization?.name ?? 'The buyer'} will contact you about the order.`
              : gap != null
                ? `Your final bid was ${inr(gap)} ${forward ? 'below' : 'above'} the winning bid.`
                : 'Bid amounts are hidden in this auction.'}
          </span>
        </div>
        <div className="row row--between card__divider" style={{ paddingTop: 20, alignItems: 'flex-end' }}>
          <div className="stack-sm" style={{ gap: 2 }}>
            <span className="small muted">Your final bid</span>
            <span className="num" style={{ fontSize: 'var(--text-xl)', fontWeight: 600 }}>{inr(result.myNetAmount)}</span>
          </div>
          <div className="row">
            <Button onClick={() => setShowLines((v) => !v)} aria-expanded={showLines}>
              {showLines ? 'Hide bid details' : 'View bid details'}
            </Button>
            {result.isWinner && !rated && <Button variant="primary" onClick={() => setRating(true)}>Rate the buyer</Button>}
          </div>
        </div>
        {showLines && (
          <div className="stack">
            <table className="table table--compact table--bare">
              <thead>
                <tr>
                  <th>Item</th>
                  <th className="num">Qty</th>
                  <th className="num">Rate</th>
                  <th className="num">Amount</th>
                </tr>
              </thead>
              <tbody>
                {result.lines.map((l) => (
                  <tr key={l.auctionRequirementId}>
                    <td>{l.itemName}</td>
                    <td className="num">{qty(l.quantity)} {l.unitName}</td>
                    <td className="num">{inrRate(l.rate)}</td>
                    <td className="num">{inr(l.baseAmount)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <dl className="totals small" style={{ margin: 0 }}>
              <dt>Basic</dt>
              <dd className="num">{inr(result.myBasicAmount)}</dd>
              <dt>Taxes</dt>
              <dd className="num">{inr(result.myTaxAmount)}</dd>
              <dt className="totals__grand">Net</dt>
              <dd className="num totals__grand">{inr(result.myNetAmount)}</dd>
            </dl>
          </div>
        )}
      </section>

      <ItemsList auction={auction} collapsible />

      {rating && (
        <RatingModal
          auctionId={auction.id}
          targetName={auction.organization?.name ?? 'the buyer'}
          targetRole="Organization"
          onClose={() => setRating(false)}
          onSubmitted={() => {
            setRating(false);
            setRated(true);
          }}
        />
      )}
    </>
  );
}
