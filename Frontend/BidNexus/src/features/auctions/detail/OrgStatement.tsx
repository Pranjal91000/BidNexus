import { Fragment, useEffect, useState } from 'react';
import { ChevronDown, Download } from 'lucide-react';
import type { Auction, BidActivity, RatingSummary, Statement } from '../../../types';
import { api } from '../../../services/api';
import { useApp } from '../../../lib/appContext';
import { dateShort, inr, inrRate, pct, qty, time } from '../../../lib/format';
import { Button } from '../../../components/ui/Button';
import { Badge, Rank } from '../../../components/ui/Badge';
import { Empty } from '../../../components/ui/States';
import { RatingModal } from '../../ratings/RatingModal';
import { ItemsList, PriceTrend } from './Blocks';
import { improvementFrom } from '../../../lib/trend';

interface Props {
  auction: Auction;
  statement: Statement[];
  activity: BidActivity[];
}

/**
 * The result document for a closed auction: who won, by how much, how everyone ranked,
 * and each vendor's line rates on demand.
 */
export function OrgStatement({ auction, statement, activity }: Props) {
  const { token, claims } = useApp();
  const [open, setOpen] = useState<number | null>(null);
  const [ratings, setRatings] = useState<RatingSummary[]>([]);
  const [rating, setRating] = useState(false);
  const forward = !!auction.isForwardAuction;

  const loadRatings = () => api.getAuctionRatings(token, auction.id).then((r) => setRatings(r || [])).catch(() => setRatings([]));
  useEffect(() => {
    loadRatings();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auction.id]);

  if (statement.length === 0) {
    return (
      <div className="card">
        <Empty title="No bids were received">This auction closed without any bids, so there is nothing to award.</Empty>
      </div>
    );
  }

  const rows = [...statement].sort((a, b) => a.rank - b.rank);
  const winner = rows.find((s) => s.isWinner) ?? rows[0];
  const runnerUp = rows.find((s) => s !== winner);
  const l1 = winner.netAmount;
  const change = improvementFrom(activity, forward);
  const alreadyRated = ratings.some((r) => r.submittedByTenantId === claims.tenantId);

  return (
    <>
      <section className="card card--pad row row--between" style={{ alignItems: 'flex-end', gap: 24 }}>
        <div className="stack-sm" style={{ gap: 4 }}>
          <span className="small muted">Awarded to</span>
          <span style={{ fontSize: 'var(--text-xl)', fontWeight: 600 }}>{winner.vendorName || `Vendor #${winner.vendorId}`}</span>
          <span className="hero-amount num" style={{ marginTop: 8 }}>{inr(winner.netAmount)}</span>
          <span className="small muted">
            {runnerUp
              ? `${inr(Math.abs(runnerUp.netAmount - l1))} ${forward ? 'higher' : 'lower'} than the next bid (${runnerUp.vendorName})`
              : 'Only bidder'}
            {change && change.percent > 0 && ` · ${pct(change.percent)} better than the opening bid of ${inr(change.opening)}`}
          </span>
        </div>
        <div className="row no-print">
          <Button icon={<Download size={16} />} onClick={() => window.print()}>Download PDF</Button>
          {alreadyRated ? <Badge>Vendor rated</Badge> : <Button onClick={() => setRating(true)}>Rate vendor</Button>}
        </div>
      </section>

      <section className="section">
        <div className="section__head">
          <h2>Final ranking</h2>
          <span className="small muted">{forward ? 'Highest' : 'Lowest'} net amount wins</span>
        </div>
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th style={{ width: 72 }}>Rank</th>
                <th>Vendor</th>
                <th className="num">Net amount</th>
                <th className="num">vs L1</th>
                <th className="no-print" aria-label="Details" style={{ width: 48 }} />
              </tr>
            </thead>
            <tbody>
              {rows.map((s, i) => {
                const isOpen = open === s.id;
                const diff = Math.abs(s.netAmount - l1);
                return (
                  <Fragment key={s.id}>
                    <tr>
                      <td><Rank rank={s.rank} /></td>
                      <td className="strong">{s.vendorName || `Vendor #${s.vendorId}`}</td>
                      <td className="num strong">{inr(s.netAmount)}</td>
                      <td className="num muted">{i === 0 ? '—' : `${forward ? '−' : '+'}${inr(diff)} (${pct((diff / l1) * 100)})`}</td>
                      <td className="no-print">
                        {(s.lines?.length ?? 0) > 0 && (
                          <button
                            type="button"
                            className="expand-btn"
                            aria-expanded={isOpen}
                            aria-label={`${isOpen ? 'Hide' : 'Show'} line rates for ${s.vendorName}`}
                            onClick={() => setOpen(isOpen ? null : s.id)}
                          >
                            <ChevronDown size={18} />
                          </button>
                        )}
                      </td>
                    </tr>
                    {isOpen && (
                      <tr className="row-detail">
                        <td />
                        <td colSpan={4}>
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
                                {(s.lines ?? []).map((l) => (
                                  <tr key={l.auctionRequirementId}>
                                    <td>{l.itemName}</td>
                                    <td className="num">{qty(l.quantity)} {l.unitName}</td>
                                    <td className="num">{inrRate(l.rate)}</td>
                                    <td className="num">{inr(l.baseAmount)}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                            <div className="row row--between" style={{ alignItems: 'flex-end' }}>
                              <span className="small muted">
                                Bid #{s.bidId}
                                {s.bidRevisionNo ? ` · ${s.bidRevisionNo} revision${s.bidRevisionNo === 1 ? '' : 's'}` : ''}
                                {s.submittedAt ? ` · last at ${time(s.submittedAt)}` : ''}
                              </span>
                              <dl className="totals small" style={{ margin: 0 }}>
                                <dt>Basic</dt>
                                <dd className="num">{inr(s.basicAmount)}</dd>
                                <dt>Taxes</dt>
                                <dd className="num">{inr(s.taxAmount)}</dd>
                                <dt className="totals__grand">Net</dt>
                                <dd className="num totals__grand">{inr(s.netAmount)}</dd>
                              </dl>
                            </div>
                          </div>
                        </td>
                      </tr>
                    )}
                  </Fragment>
                );
              })}
            </tbody>
          </table>
        </div>
        <span className="small muted">Closed {dateShort(auction.auctionEndTime)} · {rows.length} bidder{rows.length === 1 ? '' : 's'} · {activity.length || '—'} bids</span>
      </section>

      <PriceTrend auction={auction} activity={activity} />
      <ItemsList auction={auction} collapsible />

      {rating && (
        <RatingModal
          auctionId={auction.id}
          targetName={winner.vendorName}
          targetRole="Vendor"
          onClose={() => setRating(false)}
          onSubmitted={() => {
            setRating(false);
            loadRatings();
          }}
        />
      )}
    </>
  );
}
