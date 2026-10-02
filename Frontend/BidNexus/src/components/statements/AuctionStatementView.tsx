import React from 'react';
import type { Auction, Statement } from '../../types';
import { Award, CheckCircle2, FileText, RefreshCw, Trophy } from 'lucide-react';
import { Button } from '../ui/Button';

interface StatementViewProps {
  auction: Auction;
  statements: Statement[];
  onRefresh: () => void;
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2,
  }).format(amount);
};

export const StatementView: React.FC<StatementViewProps> = ({ auction, statements, onRefresh }) => {
  const winner = statements.find((s) => s.isWinner || s.rank === 1);

  if (statements.length === 0) {
    return (
      <div className="bn-statement-empty">
        <FileText size={36} className="bn-text-muted" />
        <h4>Auction Statement Pending</h4>
        <p>
          The authoritative statement and winner award breakdown will be issued by the auction engine upon official auction closure.
        </p>
        <Button variant="outline" size="sm" icon={<RefreshCw size={14} />} onClick={onRefresh} className="bn-mt-3">
          Check Statement Status
        </Button>
      </div>
    );
  }

  return (
    <div className="bn-statement-container">
      {/* Winner Highlight Banner */}
      {winner && (
        <section className="bn-winner-banner">
          <div className="bn-winner-badge-icon">
            <Trophy size={28} />
          </div>
          <div className="bn-winner-info">
            <span className="bn-eyebrow bn-text-success">OFFICIAL AWARD WINNER</span>
            <h3 className="bn-winner-name">{winner.vendorName || `Vendor #${winner.vendorId}`}</h3>
            <p className="bn-winner-details">
              Winning Bid Amount: <strong className="bn-font-mono">{formatCurrency(winner.netAmount)}</strong> • Reference Bid #{winner.bidId}
            </p>
          </div>
          <div className="bn-winner-award-chip">
            <CheckCircle2 size={16} /> Verified & Awarded
          </div>
        </section>
      )}

      {/* Statement Table */}
      <div className="bn-table-header-kicker">
        <span className="bn-eyebrow">AUTHORITATIVE STATEMENT</span>
        <h4 className="bn-table-title">Final Auction Rankings & Award Breakdown</h4>
      </div>

      <div className="bn-table-responsive">
        <table className="bn-table bn-statement-table">
          <thead>
            <tr>
              <th style={{ width: '80px' }}>Rank</th>
              <th>Vendor Name</th>
              <th>Winning Net Amount</th>
              <th>Bid Reference ID</th>
              <th>Award Outcome</th>
            </tr>
          </thead>
          <tbody>
            {statements.map((stmt) => (
              <tr key={stmt.id || stmt.bidId} className={stmt.isWinner ? 'bn-row-winner' : ''}>
                <td>
                  <span className={`bn-rank-pill ${stmt.rank === 1 ? 'bn-rank-first' : ''}`}>
                    #{stmt.rank}
                  </span>
                </td>
                <td>
                  <strong>{stmt.vendorName || `Vendor #${stmt.vendorId}`}</strong>
                </td>
                <td>
                  <span className="bn-font-mono bn-font-bold">{formatCurrency(stmt.netAmount)}</span>
                </td>
                <td>
                  <span className="bn-text-mono bn-text-muted">#BID-{stmt.bidId}</span>
                </td>
                <td>
                  {stmt.isWinner || stmt.rank === 1 ? (
                    <span className="bn-badge bn-badge-winner">
                      <Award size={13} /> WINNER AWARDED
                    </span>
                  ) : (
                    <span className="bn-badge bn-badge-scheduled">Unsuccessful Bid</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Requirement Line Items Summary */}
      <div className="bn-statement-summary-box bn-mt-6">
        <h5 className="bn-summary-title">Scope & Compliance Certification</h5>
        <p className="bn-summary-desc">
          This statement reflects the final authoritative evaluations rendered by the BidNexus procurement engine for Auction #{auction.id} ({auction.docNoYearly}). All bids were evaluated under strict multi-tenant isolation.
        </p>
      </div>
    </div>
  );
};
