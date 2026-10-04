import { ChevronRight, Plus } from 'lucide-react';
import { useApp } from '../../lib/appContext';
import { useNow } from '../../lib/hooks';
import { auctionTitle, phaseOf, sortForPhase } from '../../lib/auction';
import { dateShort, inr, inrCompact, monthLabel, pct } from '../../lib/format';
import { Button } from '../../components/ui/Button';
import { Countdown } from '../../components/ui/Countdown';
import { Callout, Empty, Loading, Stat } from '../../components/ui/States';
import { BarChart } from '../../components/charts/BarChart';

export function OrgOverview() {
  const { auctions, auctionsLoading, orgDashboard: d, openAuction, openAuctionForm, navigate } = useApp();
  const now = useNow(15000);

  const live = sortForPhase(auctions.filter((a) => phaseOf(a, now) === 'live'), 'live');
  const upcoming = sortForPhase(auctions.filter((a) => phaseOf(a, now) === 'upcoming'), 'upcoming');
  const drafts = auctions.filter((a) => phaseOf(a, now) === 'draft').length;

  const header = (
    <div className="page-header">
      <div className="page-header__text">
        <h1>Overview</h1>
      </div>
      <Button variant="primary" icon={<Plus size={18} />} onClick={() => openAuctionForm()}>
        New auction
      </Button>
    </div>
  );

  if (auctionsLoading && auctions.length === 0) {
    return <main className="page">{header}<Loading /></main>;
  }

  if (!auctionsLoading && auctions.length === 0) {
    return (
      <main className="page">
        {header}
        <div className="card">
          <Empty
            title="No auctions yet"
            action={<Button variant="primary" onClick={() => openAuctionForm()}>Create your first auction</Button>}
          >
            Add the items you want to buy in Masters, then create an auction and invite vendors to bid.
          </Empty>
        </div>
      </main>
    );
  }

  return (
    <main className="page">
      {header}

      <div className="grid-4">
        <Stat
          label="Live now"
          value={live.length}
          sub={live[0] ? <Countdown to={live[0].auctionEndTime} prefix="Next closes in" /> : 'Nothing running'}
        />
        <Stat
          label="Upcoming"
          value={upcoming.length}
          sub={upcoming[0] ? `Next starts ${dateShort(upcoming[0].auctionStartTime)}` : 'None scheduled'}
        />
        <Stat
          label="Awarded · last 90 days"
          value={d ? inrCompact(d.awardedValueLast90Days) : '—'}
          sub={d ? `${d.closedLast90Days} auction${d.closedLast90Days === 1 ? '' : 's'} closed` : undefined}
        />
        <Stat
          label="Average saving"
          value={d?.averagePriceImprovementPercent != null ? pct(d.averagePriceImprovementPercent) : '—'}
          sub={d ? `Opening bid → winning bid · ${d.averageBiddersPerAuction} bidders on average` : undefined}
        />
      </div>

      {drafts > 0 && (
        <Callout>
          <span className="grow">
            {drafts} draft{drafts === 1 ? ' is' : 's are'} not published yet. Vendors can't see drafts.
          </span>
          <button type="button" className="link-btn accent" onClick={() => navigate({ page: 'auctions', tab: 'drafts' })}>
            Review drafts
          </button>
        </Callout>
      )}

      {live.length > 0 && (
        <section className="section">
          <div className="section__head">
            <h2>Live now</h2>
          </div>
          <div className="table-wrap">
            <table className="table table--clickable">
              <tbody>
                {live.slice(0, 5).map((a) => (
                  <tr key={a.id} onClick={() => openAuction(a.id)}>
                    <td>
                      <button type="button" className="link-btn" onClick={() => openAuction(a.id)}>{auctionTitle(a)}</button>
                      <span className="cell-sub">{a.docNoYearly}</span>
                    </td>
                    <td className="num"><Countdown to={a.auctionEndTime} /></td>
                    <td style={{ width: 40 }}><ChevronRight size={18} className="muted" /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      )}

      <div className="grid-split">
        <section className="card card--pad stack">
          <div className="section__head">
            <h2>Awarded value</h2>
            <span className="small muted">Last 6 months</span>
          </div>
          {d && d.monthly.some((m) => m.value > 0) ? (
            <BarChart
              data={d.monthly.map((m) => ({ label: monthLabel(m.month), values: [m.value] }))}
              series={[{ name: 'Awarded value' }]}
              format={inrCompact}
              summary={`Awarded value by month: ${d.monthly.map((m) => `${monthLabel(m.month)} ${inrCompact(m.value)}`).join(', ')}`}
            />
          ) : (
            <Empty title="No awards yet">Closed auctions will show here by month.</Empty>
          )}
        </section>

        <section className="card card--pad stack">
          <div className="section__head">
            <h2>Recent results</h2>
            <button type="button" className="link-btn small accent" onClick={() => navigate({ page: 'auctions', tab: 'closed' })}>
              All closed
            </button>
          </div>
          {d && d.recentResults.length > 0 ? (
            <div className="stack" style={{ ['--gap' as string]: '0px' }}>
              {d.recentResults.slice(0, 5).map((r, i) => (
                <button
                  key={r.auctionId}
                  type="button"
                  className="link-btn"
                  onClick={() => openAuction(r.auctionId)}
                  style={{ padding: '12px 0', borderTop: i ? '1px solid var(--line-soft)' : 0, fontWeight: 400 }}
                >
                  <span className="row row--between" style={{ flexWrap: 'nowrap' }}>
                    <span className="grow">
                      <span className="strong ellipsis" style={{ display: 'block' }}>{r.auctionName || r.docNoYearly}</span>
                      <span className="small muted ellipsis" style={{ display: 'block' }}>{r.winnerName ?? 'No bids received'}</span>
                    </span>
                    <span className="right">
                      <span className="num strong" style={{ display: 'block' }}>{inr(r.winningAmount)}</span>
                      {r.priceImprovementPercent != null && (
                        <span className="small accent num" style={{ display: 'block' }}>{pct(r.priceImprovementPercent)} better than opening</span>
                      )}
                    </span>
                  </span>
                </button>
              ))}
            </div>
          ) : (
            <Empty title="No results yet" />
          )}
        </section>
      </div>
    </main>
  );
}
