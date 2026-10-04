import type { Auction, Bid, BidActivity } from '../../../types';
import { inr, pct, time } from '../../../lib/format';
import { Rank } from '../../../components/ui/Badge';
import { Empty, Stat } from '../../../components/ui/States';
import { ItemsList, PriceTrend } from './Blocks';
import { improvementFrom } from '../../../lib/trend';

interface Props {
  auction: Auction;
  leaderboard: Bid[];
  activity: BidActivity[];
}

/** What a buyer watches while an auction runs: best price, competition, trend, ranking. */
export function OrgLive({ auction, leaderboard, activity }: Props) {
  const forward = !!auction.isForwardAuction;
  const best = leaderboard[0]?.netAmount ?? null;
  const change = improvementFrom(activity, forward);

  return (
    <>
      <div className="grid-4">
        <Stat label={forward ? 'Highest bid' : 'Lowest bid'} value={inr(best)} sub={leaderboard[0]?.vendor?.name} />
        <Stat label="Bidders" value={leaderboard.length} />
        <Stat label="Bids placed" value={activity.length} sub={activity.length ? `Last at ${time(activity[activity.length - 1].at)}` : undefined} />
        <Stat
          label={forward ? 'Gain vs opening' : 'Saving vs opening'}
          value={change ? pct(change.percent) : '—'}
          sub={change ? `Opened at ${inr(change.opening)}` : undefined}
        />
      </div>

      <PriceTrend auction={auction} activity={activity} />

      <section className="section">
        <div className="section__head">
          <h2>Ranking</h2>
          <span className="small muted">Updates as bids arrive</span>
        </div>
        {leaderboard.length === 0 ? (
          <div className="card">
            <Empty title="No bids yet">The ranking appears as soon as the first vendor bids.</Empty>
          </div>
        ) : (
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th style={{ width: 72 }}>Rank</th>
                  <th>Vendor</th>
                  <th className="num">Net amount</th>
                  <th className="num">vs L1</th>
                  <th className="num">Revisions</th>
                  <th className="num">Last bid</th>
                </tr>
              </thead>
              <tbody>
                {leaderboard.map((b, i) => {
                  const diff = best != null ? Math.abs(b.netAmount - best) : 0;
                  return (
                    <tr key={b.id}>
                      <td><Rank rank={i + 1} /></td>
                      <td className="strong">{b.vendor?.name || `Vendor #${b.vendorId}`}</td>
                      <td className="num strong">{inr(b.netAmount)}</td>
                      <td className="num muted">{i === 0 || best == null ? '—' : `${forward ? '−' : '+'}${inr(diff)} (${pct((diff / best) * 100)})`}</td>
                      <td className="num">{b.bidRevisionNo}</td>
                      <td className="num muted">{time(b.createdAt)}</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </section>

      <ItemsList auction={auction} collapsible />
    </>
  );
}
