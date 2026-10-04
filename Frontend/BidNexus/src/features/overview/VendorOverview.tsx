import { ChevronRight } from 'lucide-react';
import { useApp } from '../../lib/appContext';
import { useNow } from '../../lib/hooks';
import { auctionTitle, phaseOf, sortForPhase } from '../../lib/auction';
import { dateShort, inrCompact, monthLabel, pct } from '../../lib/format';
import { Countdown } from '../../components/ui/Countdown';
import { Rank } from '../../components/ui/Badge';
import { Empty, Loading, Stat } from '../../components/ui/States';
import { BarChart } from '../../components/charts/BarChart';

export function VendorOverview() {
  const { auctions, auctionsLoading, vendorDashboard: d, openAuction, navigate } = useApp();
  const now = useNow(15000);

  const live = sortForPhase(auctions.filter((a) => phaseOf(a, now) === 'live'), 'live');
  const upcoming = sortForPhase(auctions.filter((a) => phaseOf(a, now) === 'upcoming'), 'upcoming');
  const positions = d?.livePositions ?? [];
  const positionIds = new Set(positions.map((p) => p.auctionId));
  const liveNotBid = live.filter((a) => !positionIds.has(a.id));
  const winRate = d && d.participatedLast12Months > 0 ? (d.wonLast12Months / d.participatedLast12Months) * 100 : null;

  if (auctionsLoading && auctions.length === 0) {
    return <main className="page"><h1>Overview</h1><Loading /></main>;
  }

  return (
    <main className="page">
      <div className="page-header">
        <div className="page-header__text">
          <h1>Overview</h1>
        </div>
      </div>

      <div className="grid-4">
        <Stat
          label="Your live auctions"
          value={d ? d.liveParticipating : '—'}
          sub={d ? (d.liveParticipating ? `Leading in ${d.leading}` : 'Not bidding right now') : undefined}
        />
        <Stat
          label="Open to bid"
          value={live.length + upcoming.length}
          sub={upcoming[0] ? `Next starts ${dateShort(upcoming[0].auctionStartTime)}` : `${live.length} live now`}
        />
        <Stat
          label="Won · last 12 months"
          value={d ? `${d.wonLast12Months} of ${d.participatedLast12Months}` : '—'}
          sub={winRate != null ? `${pct(winRate, 0)} win rate` : undefined}
        />
        <Stat label="Value won · last 12 months" value={d ? inrCompact(d.wonValueLast12Months) : '—'} />
      </div>

      <section className="section">
        <div className="section__head">
          <h2>Where you stand</h2>
          <span className="small muted">Live auctions you have bid in</span>
        </div>
        {positions.length > 0 ? (
          <div className="table-wrap">
            <table className="table table--clickable">
              <thead>
                <tr>
                  <th>Auction</th>
                  <th>Closes</th>
                  <th>Your position</th>
                  <th aria-label="Open" />
                </tr>
              </thead>
              <tbody>
                {positions.map((p) => (
                  <tr key={p.auctionId} onClick={() => openAuction(p.auctionId)}>
                    <td>
                      <button type="button" className="link-btn" onClick={() => openAuction(p.auctionId)}>{p.auctionName}</button>
                    </td>
                    <td className="num"><Countdown to={p.endsAt} /></td>
                    <td>
                      <span className="row" style={{ gap: 8 }}>
                        <Rank rank={p.rank} />
                        <span className={`small ${p.rank === 1 ? 'accent' : 'warn'}`}>
                          {p.rank === 1 ? 'Leading' : `of ${p.bidders}`}
                        </span>
                      </span>
                    </td>
                    <td style={{ width: 40 }}><ChevronRight size={18} className="muted" /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <div className="card">
            <Empty title="You're not in any live auction">
              {liveNotBid.length ? `${liveNotBid.length} auction${liveNotBid.length === 1 ? ' is' : 's are'} live and open for bids.` : 'Upcoming auctions are listed below.'}
            </Empty>
          </div>
        )}
      </section>

      <div className="grid-split">
        <section className="card card--pad stack">
          <div className="section__head">
            <h2>Participation</h2>
            <span className="small muted">Closed auctions, last 6 months</span>
          </div>
          {d && d.monthly.some((m) => m.auctions > 0) ? (
            <BarChart
              data={d.monthly.map((m) => ({ label: monthLabel(m.month), values: [m.auctions, m.won] }))}
              series={[{ name: 'Took part', variant: 'secondary' }, { name: 'Won' }]}
              format={(n) => String(Math.round(n))}
              height={200}
              summary={`Auctions taken part in and won by month: ${d.monthly.map((m) => `${monthLabel(m.month)} ${m.auctions} / ${m.won}`).join(', ')}`}
            />
          ) : (
            <Empty title="No closed auctions yet" />
          )}
        </section>

        <section className="card card--pad stack">
          <div className="section__head">
            <h2>Open for bids</h2>
            <button type="button" className="link-btn small accent" onClick={() => navigate({ page: 'auctions' })}>
              All auctions
            </button>
          </div>
          {[...liveNotBid, ...upcoming].length ? (
            <div className="stack" style={{ ['--gap' as string]: '0px' }}>
              {[...liveNotBid, ...upcoming].slice(0, 5).map((a, i) => {
                const isLive = phaseOf(a, now) === 'live';
                return (
                  <button
                    key={a.id}
                    type="button"
                    className="link-btn"
                    onClick={() => openAuction(a.id)}
                    style={{ padding: '12px 0', borderTop: i ? '1px solid var(--line-soft)' : 0, fontWeight: 400 }}
                  >
                    <span className="row row--between" style={{ flexWrap: 'nowrap' }}>
                      <span className="grow">
                        <span className="strong ellipsis" style={{ display: 'block' }}>{auctionTitle(a)}</span>
                        <span className="small muted ellipsis" style={{ display: 'block' }}>{a.organization?.name ?? a.docNoYearly}</span>
                      </span>
                      <span className="small num">
                        {isLive ? <Countdown to={a.auctionEndTime} /> : `Starts ${dateShort(a.auctionStartTime)}`}
                      </span>
                    </span>
                  </button>
                );
              })}
            </div>
          ) : (
            <Empty title="Nothing open right now" />
          )}
        </section>
      </div>
    </main>
  );
}
