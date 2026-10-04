import { useMemo, useState } from 'react';
import { ChevronRight, Pencil, Plus, Search, Trash2 } from 'lucide-react';
import type { Auction } from '../../types';
import { api } from '../../services/api';
import { errorMessage, useApp } from '../../lib/appContext';
import { useNow } from '../../lib/hooks';
import { auctionTitle, phaseOf, sortForPhase, type Phase } from '../../lib/auction';
import { dateShort, inr } from '../../lib/format';
import { Button } from '../../components/ui/Button';
import { Countdown } from '../../components/ui/Countdown';
import { PhaseBadge, Rank } from '../../components/ui/Badge';
import { Tabs } from '../../components/ui/Tabs';
import { Empty, Loading } from '../../components/ui/States';
import { ConfirmDialog } from '../../components/ui/Modal';

type TabId = 'live' | 'upcoming' | 'drafts' | 'closed';

const TAB_PHASES: Record<TabId, Phase[]> = {
  live: ['live'],
  upcoming: ['upcoming'],
  drafts: ['draft'],
  closed: ['closing', 'closed'],
};

const EMPTY_TEXT: Record<TabId, string> = {
  live: 'No auction is running right now.',
  upcoming: 'Nothing is scheduled.',
  drafts: 'No drafts. Drafts stay private until you publish them.',
  closed: 'Closed auctions and their results appear here.',
};

export function AuctionsPage({ tab: routeTab }: { tab: string | null }) {
  const { auctions, auctionsLoading, isOrg, token, orgDashboard, vendorDashboard, openAuction, openAuctionForm, navigate, refresh, toast } = useApp();
  const now = useNow(15000);
  const [query, setQuery] = useState('');
  const [toDelete, setToDelete] = useState<Auction | null>(null);
  const [deleting, setDeleting] = useState(false);

  const byTab = useMemo(() => {
    const groups: Record<TabId, Auction[]> = { live: [], upcoming: [], drafts: [], closed: [] };
    for (const a of auctions) {
      const p = phaseOf(a, now);
      const tab = (Object.keys(TAB_PHASES) as TabId[]).find((t) => TAB_PHASES[t].includes(p));
      if (tab) groups[tab].push(a);
    }
    return groups;
  }, [auctions, now]);

  const tabIds: TabId[] = isOrg ? ['live', 'upcoming', 'drafts', 'closed'] : ['live', 'upcoming', 'closed'];
  const fallback: TabId = byTab.live.length ? 'live' : 'upcoming';
  const active: TabId = tabIds.includes(routeTab as TabId) ? (routeTab as TabId) : fallback;

  const q = query.trim().toLowerCase();
  const rows = sortForPhase(
    byTab[active].filter((a) => !q || `${a.auctionName} ${a.docNoYearly} ${a.organization?.name ?? ''}`.toLowerCase().includes(q)),
    active === 'live' ? 'live' : active === 'upcoming' ? 'upcoming' : 'closed',
  );

  const results = new Map((orgDashboard?.recentResults ?? []).map((r) => [r.auctionId, r]));
  const positions = new Map((vendorDashboard?.livePositions ?? []).map((p) => [p.auctionId, p]));

  const confirmDelete = async () => {
    if (!toDelete) return;
    setDeleting(true);
    try {
      await api.deleteAuction(token, toDelete.id);
      toast('Draft deleted.', 'success');
      setToDelete(null);
      await refresh();
    } catch (err) {
      toast(errorMessage(err, 'Could not delete the auction.'), 'error');
    } finally {
      setDeleting(false);
    }
  };

  const whenCell = (a: Auction) => {
    const p = phaseOf(a, now);
    if (p === 'live') return <Countdown to={a.auctionEndTime} />;
    if (p === 'upcoming') return <span>Starts {dateShort(a.auctionStartTime)}</span>;
    if (p === 'draft') return <span className="muted">Starts {dateShort(a.auctionStartTime)}</span>;
    if (p === 'closing') return <span className="muted">Preparing results</span>;
    return <span>{dateShort(a.auctionEndTime)}</span>;
  };

  const thirdHeader = isOrg ? (active === 'closed' ? 'Winner' : 'Items') : active === 'live' ? 'Your position' : active === 'closed' ? 'Result' : 'Items';

  const thirdCell = (a: Auction) => {
    const items = a.auctionRequirements?.length ?? 0;
    if (isOrg && active === 'closed') {
      const r = results.get(a.id);
      if (!r) return <span className="muted">View statement</span>;
      return (
        <span>
          <span className="ellipsis" style={{ display: 'block' }}>{r.winnerName ?? 'No bids'}</span>
          {r.winningAmount != null && <span className="cell-sub">{inr(r.winningAmount)}</span>}
        </span>
      );
    }
    if (!isOrg && active === 'live') {
      const p = positions.get(a.id);
      if (!p) return <span className="muted">Not bid yet</span>;
      return (
        <span className="row" style={{ gap: 8 }}>
          <Rank rank={p.rank} />
          <span className={`small ${p.rank === 1 ? 'accent' : 'warn'}`}>{p.rank === 1 ? 'Leading' : `of ${p.bidders}`}</span>
        </span>
      );
    }
    if (!isOrg && active === 'closed') return <span className="muted">View result</span>;
    return <span className="num">{items} item{items === 1 ? '' : 's'}</span>;
  };

  return (
    <main className="page">
      <div className="page-header">
        <div className="page-header__text">
          <h1>Auctions</h1>
        </div>
        {isOrg && (
          <Button variant="primary" icon={<Plus size={18} />} onClick={() => openAuctionForm()}>
            New auction
          </Button>
        )}
      </div>

      <div className="stack">
        <div className="toolbar">
          <Tabs
            label="Auction status"
            tabs={tabIds.map((id) => ({ id, label: id[0].toUpperCase() + id.slice(1), count: byTab[id].length }))}
            active={active}
            onChange={(id) => navigate({ page: 'auctions', tab: id })}
          />
          <div className="search">
            <Search size={16} />
            <label className="sr-only" htmlFor="auction-search">Search auctions</label>
            <input id="auction-search" className="input" placeholder="Search name or doc no." value={query} onChange={(e) => setQuery(e.target.value)} />
          </div>
        </div>

        {auctionsLoading && auctions.length === 0 ? (
          <Loading />
        ) : rows.length === 0 ? (
          <div className="card">
            <Empty title={q ? 'No matches' : 'Nothing here'}>{q ? `No auction matches “${query}”.` : EMPTY_TEXT[active]}</Empty>
          </div>
        ) : (
          <div className="table-wrap">
            <table className="table table--clickable">
              <thead>
                <tr>
                  <th>Auction</th>
                  <th>{active === 'live' ? 'Closes' : active === 'closed' ? 'Closed' : 'Schedule'}</th>
                  <th>{thirdHeader}</th>
                  <th aria-label="Actions" />
                </tr>
              </thead>
              <tbody>
                {rows.map((a) => {
                  const phase = phaseOf(a, now);
                  const editable = isOrg && (phase === 'draft' || phase === 'upcoming');
                  return (
                    <tr key={a.id} onClick={() => openAuction(a.id)}>
                      <td style={{ maxWidth: 420 }}>
                        <span className="row" style={{ gap: 8, flexWrap: 'nowrap' }}>
                          <button type="button" className="link-btn ellipsis" onClick={() => openAuction(a.id)}>
                            {auctionTitle(a)}
                          </button>
                          {phase === 'closing' && <PhaseBadge phase={phase} />}
                        </span>
                        <span className="cell-sub ellipsis">
                          {a.docNoYearly} · {a.isForwardAuction ? 'Forward' : 'Reverse'}
                          {!isOrg && a.organization?.name ? ` · ${a.organization.name}` : ''}
                        </span>
                      </td>
                      <td>{whenCell(a)}</td>
                      <td>{thirdCell(a)}</td>
                      <td style={{ width: 96 }} onClick={(e) => e.stopPropagation()}>
                        <span className="row row--end" style={{ gap: 0, flexWrap: 'nowrap' }}>
                          {editable && (
                            <button type="button" className="icon-btn" aria-label={`Edit ${auctionTitle(a)}`} onClick={() => openAuctionForm(a)}>
                              <Pencil size={16} />
                            </button>
                          )}
                          {isOrg && phase === 'draft' && (
                            <button type="button" className="icon-btn icon-btn--danger" aria-label={`Delete ${auctionTitle(a)}`} onClick={() => setToDelete(a)}>
                              <Trash2 size={16} />
                            </button>
                          )}
                          {!editable && <ChevronRight size={18} className="muted" aria-hidden="true" />}
                        </span>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <ConfirmDialog
        open={!!toDelete}
        title="Delete draft?"
        message={`“${toDelete ? auctionTitle(toDelete) : ''}” will be removed. This can't be undone.`}
        confirmLabel="Delete draft"
        danger
        busy={deleting}
        onConfirm={confirmDelete}
        onCancel={() => setToDelete(null)}
      />
    </main>
  );
}
