import { ChevronRight } from 'lucide-react';
import type { Auction, BidActivity } from '../../../types';
import { dateTime, inrCompact, qty } from '../../../lib/format';
import { itemNameOf, unitNameOf } from '../../../lib/auction';
import { TrendChart, type TrendSeries } from '../../../components/charts/TrendChart';
import { bestOverTime } from '../../../lib/trend';
import { useNow } from '../../../lib/hooks';

/** Key facts for an auction that has not started yet. */
export function Facts({ auction }: { auction: Auction }) {
  const facts = [
    ['Type', auction.isForwardAuction ? 'Forward — highest bid wins' : 'Reverse — lowest bid wins'],
    ['Starts', dateTime(auction.auctionStartTime)],
    ['Ends', dateTime(auction.auctionEndTime)],
    ['Who can bid', auction.openToAll ? 'All vendors' : 'Approved vendors only'],
    ['Bid amounts', auction.isBidPriceHidden ? 'Hidden from vendors' : 'Visible to vendors'],
  ];
  return (
    <section className="card card--pad">
      <dl className="facts" style={{ margin: 0 }}>
        {facts.map(([label, value]) => (
          <div key={label}>
            <dt className="fact__label">{label}</dt>
            <dd className="fact__value" style={{ margin: 0 }}>{value}</dd>
          </div>
        ))}
      </dl>
    </section>
  );
}

/** Requirement lines. Collapsible once the auction is running, because then the numbers matter more. */
export function ItemsList({ auction, collapsible }: { auction: Auction; collapsible?: boolean }) {
  const reqs = [...(auction.auctionRequirements ?? [])].sort((a, b) => (a.lineNo ?? 0) - (b.lineNo ?? 0));
  const table = (
    <div className="table-wrap">
      <table className="table">
        <thead>
          <tr>
            <th style={{ width: 56 }}>Line</th>
            <th>Item</th>
            <th className="num">Quantity</th>
          </tr>
        </thead>
        <tbody>
          {reqs.map((r, i) => (
            <tr key={r.id}>
              <td className="muted">{r.lineNo || i + 1}</td>
              <td>
                <span className="strong">{itemNameOf(r)}</span>
                {r.technicalSpecification && <span className="cell-sub">{r.technicalSpecification}</span>}
              </td>
              <td className="num">{qty(r.quantity)} {unitNameOf(r)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );

  if (!collapsible) {
    return (
      <section className="section">
        <h2>Items</h2>
        {table}
      </section>
    );
  }
  return (
    <details className="disclosure">
      <summary>
        <ChevronRight size={18} />
        Items ({reqs.length})
      </summary>
      {table}
    </details>
  );
}

/** Price trend: best bid in the market over time, plus the caller's own bids for vendors. */
export function PriceTrend({ auction, activity, showMine }: { auction: Auction; activity: BidActivity[]; showMine?: boolean }) {
  const now = useNow(5000);
  const forward = !!auction.isForwardAuction;
  const events = activity.map((e) => ({ t: new Date(e.at).getTime(), v: e.netAmount, bidder: e.bidder, mine: e.isMine }));
  const othersVisible = events.some((e) => !e.mine && e.v != null);

  const series: TrendSeries[] = [];
  if (othersVisible || !showMine) {
    series.push({ name: forward ? 'Highest bid' : 'Lowest bid', points: bestOverTime(events, forward), variant: 'market' });
  }
  if (showMine) {
    series.push({
      name: 'Your bid',
      points: events.filter((e) => e.mine && e.v != null).map((e) => ({ t: e.t, v: e.v as number })),
      variant: 'mine',
    });
  }
  if (!series.some((s) => s.points.length)) return null;

  const start = new Date(auction.auctionStartTime).getTime();
  const end = Math.min(now, new Date(auction.auctionEndTime).getTime());
  return (
    <section className="card card--pad stack">
      <div className="section__head">
        <h2>Price trend</h2>
        <span className="small muted">{events.length} bid{events.length === 1 ? '' : 's'}</span>
      </div>
      <TrendChart
        series={series}
        start={start}
        end={end}
        format={inrCompact}
        summary={`Price trend with ${events.length} bids`}
      />
    </section>
  );
}
