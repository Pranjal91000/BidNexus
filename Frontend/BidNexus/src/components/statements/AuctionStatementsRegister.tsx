import React, { useState, useEffect } from 'react';
import type { Auction, Statement } from '../../types';
import { Award, RefreshCw } from 'lucide-react';
import { Button } from '../ui/Button';
import { Badge } from '../ui/Badge';
import { LoadingState } from '../ui/LoadingState';
import { EmptyState } from '../ui/EmptyState';
import { api } from '../../services/api';

interface AuctionStatementsRegisterProps {
  auctions: Auction[];
  token: string;
  onOpenAuction: (auction: Auction) => void;
  onShowToast: (message: string, type?: 'success' | 'error' | 'warning' | 'info') => void;
}

export const AuctionStatementsRegister: React.FC<AuctionStatementsRegisterProps> = ({
  auctions,
  token,
  onOpenAuction,
  onShowToast,
}) => {
  const [statementRows, setStatementRows] = useState<{ auction: Auction; statements: Statement[] }[]>([]);
  const [loading, setLoading] = useState(true);

  const closedAuctions = auctions.filter(
    (a) => (a.statusName || '').toLowerCase().includes('close') || (a.statusName || '').toLowerCase().includes('complete')
  );

  const loadAllStatements = async () => {
    setLoading(true);
    try {
      const targetAuctions = closedAuctions.length > 0 ? closedAuctions : auctions.slice(0, 15);
      const results = await Promise.all(
        targetAuctions.map(async (a) => {
          try {
            const stmts = await api.getAuctionStatement(token, a.id);
            return { auction: a, statements: stmts || [] };
          } catch {
            return { auction: a, statements: [] };
          }
        })
      );
      setStatementRows(results.filter((r) => r.statements.length > 0));
    } catch (err: any) {
      onShowToast(err instanceof Error ? err.message : 'Unable to load auction statements', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAllStatements();
  }, [auctions, token]);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 2,
    }).format(amount);
  };

  return (
    <div className="bn-statements-register-stack">
      <div className="bn-section-header">
        <div>
          <span className="bn-eyebrow">ORGANIZATION RESULTS</span>
          <h2 className="bn-section-title">Procurement Statements & Winner Awards</h2>
        </div>
        <Button variant="outline" size="sm" icon={<RefreshCw size={14} />} onClick={loadAllStatements}>
          Refresh Statements
        </Button>
      </div>

      {loading ? (
        <LoadingState message="Fetching closed auction statements..." />
      ) : statementRows.length > 0 ? (
        <div className="bn-statements-cards-list">
          {statementRows.map(({ auction, statements }) => {
            const winner = statements.find((s) => s.isWinner || s.rank === 1);
            return (
              <div key={auction.id} className="bn-statement-overview-card" onClick={() => onOpenAuction(auction)}>
                <div className="bn-card-header">
                  <Badge tone="closed">Closed & Awarded</Badge>
                  <span className="bn-auction-id">Doc #{auction.docNoYearly}</span>
                </div>

                <h3 className="bn-card-title">{auction.auctionName || auction.docNoYearly}</h3>

                {winner ? (
                  <div className="bn-statement-winner-summary">
                    <Award size={18} className="bn-text-success" />
                    <div>
                      <strong>Winner: {winner.vendorName || `Vendor #${winner.vendorId}`}</strong>
                      <span className="bn-block bn-font-mono bn-text-sm">
                        Winning Amount: {formatCurrency(winner.netAmount)}
                      </span>
                    </div>
                  </div>
                ) : (
                  <p className="bn-text-muted bn-text-sm">Statement generated with {statements.length} vendor rankings.</p>
                )}

                <div className="bn-card-footer bn-mt-3">
                  <span className="bn-text-xs bn-text-muted">{statements.length} evaluated vendor bids</span>
                  <Button variant="primary" size="sm">
                    View Complete Statement →
                  </Button>
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        <EmptyState
          title="No statements generated yet"
          description="Statements are generated automatically when procurement auctions are closed and awarded."
        />
      )}
    </div>
  );
};
