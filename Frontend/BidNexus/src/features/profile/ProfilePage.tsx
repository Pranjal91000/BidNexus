import { useEffect, useState } from 'react';
import type { RatingSummary, TenantReputation } from '../../types';
import { api } from '../../services/api';
import { useApp } from '../../lib/appContext';
import { dateShort } from '../../lib/format';
import { Empty, Loading } from '../../components/ui/States';

export function ProfilePage() {
  const { token, claims, isOrg } = useApp();
  const [loading, setLoading] = useState(true);
  const [rep, setRep] = useState<TenantReputation | null>(null);
  const [history, setHistory] = useState<RatingSummary[]>([]);

  useEffect(() => {
    Promise.all([
      api.getMyReputation(token).then(setRep).catch(() => setRep(null)),
      api.getTenantRatingHistory(token, claims.tenantId).then((h) => setHistory(h || [])).catch(() => setHistory([])),
    ]).finally(() => setLoading(false));
  }, [token, claims.tenantId]);

  const facts = [
    ['Name', claims.name],
    ['Email', claims.email || '—'],
    ['Account type', isOrg ? 'Buyer organisation' : 'Vendor'],
  ];

  return (
    <main className="page page--narrow">
      <div className="page-header">
        <div className="page-header__text">
          <h1>Profile</h1>
        </div>
      </div>

      <section className="card card--pad">
        <dl className="facts" style={{ margin: 0 }}>
          {facts.map(([label, value]) => (
            <div key={label}>
              <dt className="fact__label">{label}</dt>
              <dd className="fact__value ellipsis" style={{ margin: 0 }}>{value}</dd>
            </div>
          ))}
        </dl>
      </section>

      <section className="section">
        <div className="section__head">
          <h2>Your reputation</h2>
          <span className="small muted">Ratings from {isOrg ? 'vendors' : 'buyers'} after auctions</span>
        </div>
        {loading ? (
          <Loading />
        ) : !rep || rep.totalRatingsReceived === 0 ? (
          <div className="card">
            <Empty title="No ratings yet">
              {isOrg ? 'Winning vendors can rate you after an auction closes.' : 'Buyers can rate you after awarding an auction to you.'}
            </Empty>
          </div>
        ) : (
          <div className="card card--pad stack-lg" style={{ gap: 24 }}>
            <div className="row" style={{ gap: 12, alignItems: 'baseline' }}>
              <span className="num" style={{ fontSize: 40, fontWeight: 600, lineHeight: 1 }}>{(rep.averageRating ?? 0).toFixed(1)}</span>
              <span className="muted">out of 5 · {rep.totalRatingsReceived} rating{rep.totalRatingsReceived === 1 ? '' : 's'}</span>
            </div>
            {(rep.parameterBreakdown ?? []).length > 0 && (
              <div className="stack" style={{ ['--gap' as string]: '10px' }}>
                {(rep.parameterBreakdown ?? []).map((p) => (
                  <div key={p.parameterId} className="bar-row">
                    <span className="small">{p.parameterName}</span>
                    <span className="bar-track" aria-hidden="true">
                      <span className="bar-fill" style={{ display: 'block', width: `${((p.averageScore ?? 0) / 5) * 100}%` }} />
                    </span>
                    <span className="small num right">{(p.averageScore ?? 0).toFixed(1)}</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </section>

      {history.length > 0 && (
        <section className="section">
          <h2>Recent feedback</h2>
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Auction</th>
                  <th>Date</th>
                  <th className="num">Score</th>
                </tr>
              </thead>
              <tbody>
                {history.slice(0, 10).map((h) => (
                  <tr key={h.id}>
                    <td>
                      <span className="strong">{h.auctionName || `Auction #${h.auctionId}`}</span>
                      {h.remarks && <span className="cell-sub">“{h.remarks}”</span>}
                    </td>
                    <td className="muted">{dateShort(h.createdAt)}</td>
                    <td className="num strong">{(h.averageScore ?? 0).toFixed(1)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      )}
    </main>
  );
}
