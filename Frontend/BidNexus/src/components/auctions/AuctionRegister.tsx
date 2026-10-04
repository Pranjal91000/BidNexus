import React, { useState, useMemo } from 'react';
import type { Auction, Claims, AuctionCreateRequest } from '../../types';
import { AuctionCard } from './AuctionCard';
import { Button } from '../ui/Button';
import { Select } from '../ui/Input';
import { Badge, toneFromStatus } from '../ui/Badge';
import { Countdown } from '../ui/Countdown';
import { EmptyState } from '../ui/EmptyState';
import { LoadingState } from '../ui/LoadingState';
import { AuctionFormModal } from './AuctionFormModal';
import { ConfirmDialog } from '../ui/Modal';
import { Plus, LayoutGrid, List, Search, RefreshCw, Edit, Trash2 } from 'lucide-react';
import { api } from '../../services/api';

interface AuctionRegisterProps {
  auctions: Auction[];
  loading: boolean;
  claims: Claims;
  token: string;
  onOpenAuction: (auction: Auction) => void;
  onRefresh: () => void;
  searchQuery: string;
  setSearchQuery: (q: string) => void;
}

export const AuctionRegister: React.FC<AuctionRegisterProps> = ({
  auctions,
  loading,
  claims,
  token,
  onOpenAuction,
  onRefresh,
  searchQuery,
  setSearchQuery,
}) => {
  // Filters & Layout
  const [statusFilter, setStatusFilter] = useState<string>('ALL');
  const [typeFilter, setTypeFilter] = useState<string>('ALL');
  const [viewMode, setViewMode] = useState<'grid' | 'table'>('grid');
  const sortBy = 'newest';

  // Modal states
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingAuction, setEditingAuction] = useState<Auction | null>(null);

  const [deletingAuction, setDeletingAuction] = useState<Auction | null>(null);
  const [deleting, setDeleting] = useState(false);

  const isOrg = claims.role === 'Organization';

  // Filter & Sort logic
  const filteredAuctions = useMemo(() => {
    return auctions
      .filter((a) => {
        // Search filter
        const text = `${a.docNoYearly} ${a.auctionName || ''} ${a.organization?.name || ''} ${a.about || ''}`.toLowerCase();
        if (searchQuery && !text.includes(searchQuery.toLowerCase())) {
          return false;
        }

        // Status filter
        const tone = toneFromStatus(a.statusName);
        if (statusFilter === 'LIVE' && tone !== 'live') return false;
        if (statusFilter === 'SCHEDULED' && tone !== 'scheduled') return false;
        if ((statusFilter === 'CLOSED' || statusFilter === 'COMPLETED') && tone !== 'closed') return false;

        // Type filter
        if (typeFilter === 'FORWARD' && !a.isForwardAuction) return false;
        if (typeFilter === 'REVERSE' && a.isForwardAuction) return false;

        return true;
      })
      .sort((a, b) => {
        if (sortBy === 'newest') return b.id - a.id;
        if (sortBy === 'oldest') return a.id - b.id;
        return a.id - b.id;
      });
  }, [auctions, searchQuery, statusFilter, typeFilter, sortBy]);

  const handleCreateSubmit = async (data: AuctionCreateRequest) => {
    if (editingAuction) {
      await api.updateAuction(token, editingAuction.id, data);
    } else {
      await api.createAuction(token, data);
    }
    onRefresh();
  };

  const handleDeleteConfirm = async () => {
    if (!deletingAuction) return;
    setDeleting(true);
    try {
      await api.deleteAuction(token, deletingAuction.id);
      setDeletingAuction(null);
      onRefresh();
    } catch (err: any) {
      alert(err instanceof Error ? err.message : 'Failed to delete auction');
    } finally {
      setDeleting(false);
    }
  };

  const formatDate = (val?: string) => {
    if (!val) return '—';
    const d = new Date(val);
    return isNaN(d.getTime()) ? val : d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' });
  };

  return (
    <div className="bn-register-stack">
      {/* Top Register Toolbar */}
      <div className="bn-toolbar">
        <div className="bn-toolbar-left">
          <div className="bn-search-wrapper">
            <Search size={16} className="bn-search-icon" />
            <input
              type="text"
              className="bn-input bn-search-input"
              placeholder="Search by Title, Doc #, Organization..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          <div className="bn-flex-center gap-2">
            <Select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              options={[
                { value: 'ALL', label: 'All Statuses' },
                { value: 'LIVE', label: '● Live Market Only' },
                { value: 'SCHEDULED', label: 'Scheduled' },
                { value: 'COMPLETED', label: 'Completed / Awarded' },
              ]}
            />

            <Select
              value={typeFilter}
              onChange={(e) => setTypeFilter(e.target.value)}
              options={[
                { value: 'ALL', label: 'All Auction Types' },
                { value: 'REVERSE', label: 'Reverse Auctions (Lowest Wins)' },
                { value: 'FORWARD', label: 'Forward Auctions (Highest Wins)' },
              ]}
            />
          </div>
        </div>

        <div className="bn-toolbar-right">
          <div className="bn-view-toggle">
            <button
              className={`bn-toggle-btn ${viewMode === 'grid' ? 'active' : ''}`}
              onClick={() => setViewMode('grid')}
              title="Grid view"
            >
              <LayoutGrid size={16} />
            </button>
            <button
              className={`bn-toggle-btn ${viewMode === 'table' ? 'active' : ''}`}
              onClick={() => setViewMode('table')}
              title="Table view"
            >
              <List size={16} />
            </button>
          </div>

          <button className="bn-icon-btn" onClick={onRefresh} title="Refresh auction list">
            <RefreshCw size={16} />
          </button>

          {isOrg && (
            <Button
              variant="primary"
              icon={<Plus size={16} />}
              onClick={() => { setEditingAuction(null); setIsFormOpen(true); }}
            >
              Create Auction
            </Button>
          )}
        </div>
      </div>

      {/* Main Content View */}
      {loading ? (
        <LoadingState message="Loading procurement auction register..." />
      ) : filteredAuctions.length > 0 ? (
        viewMode === 'grid' ? (
          <div className="bn-grid-auctions">
            {filteredAuctions.map((auction) => (
              <AuctionCard
                key={auction.id}
                auction={auction}
                onOpen={onOpenAuction}
                onEdit={isOrg ? (a) => { setEditingAuction(a); setIsFormOpen(true); } : undefined}
                onDelete={isOrg ? (a) => setDeletingAuction(a) : undefined}
                canManage={isOrg}
              />
            ))}
          </div>
        ) : (
          <div className="bn-table-container">
            <table className="bn-table">
              <thead>
                <tr>
                  <th>Doc #</th>
                  <th>Auction Title</th>
                  <th>Type</th>
                  <th>Status</th>
                  <th>Start Time</th>
                  <th>End Time</th>
                  <th>Requirements</th>
                  <th>Visibility</th>
                  <th style={{ textAlign: 'right' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredAuctions.map((auction) => (
                  <tr key={auction.id} className="bn-clickable-row" onClick={() => onOpenAuction(auction)}>
                    <td>
                      <span className="bn-font-mono bn-text-accent">#{auction.id}</span>
                      <small className="bn-block bn-text-muted">{auction.docNoYearly}</small>
                    </td>
                    <td>
                      <strong>{auction.auctionName || auction.docNoYearly}</strong>
                      <small className="bn-block bn-text-muted">{auction.organization?.name || 'Organization'}</small>
                    </td>
                    <td>
                      <span className="bn-type-badge">
                        {auction.isForwardAuction ? 'FORWARD' : 'REVERSE'}
                      </span>
                    </td>
                    <td>
                      <Badge tone={auction.statusName}>{auction.statusName || 'Scheduled'}</Badge>
                    </td>
                    <td>
                      <span className="bn-text-xs">{formatDate(auction.auctionStartTime)}</span>
                    </td>
                    <td>
                      <div className="bn-flex-col gap-1">
                        <span className="bn-text-xs">{formatDate(auction.auctionEndTime)}</span>
                        <Countdown endTime={auction.auctionEndTime} startTime={auction.auctionStartTime} compact />
                      </div>
                    </td>
                    <td>
                      <span className="bn-req-pill">
                        {auction.auctionRequirements?.length || 0} Lines
                      </span>
                    </td>
                    <td>
                      {auction.isBidPriceHidden ? (
                        <span className="bn-badge bn-badge-warning" title="Rank visible only">Hidden Price</span>
                      ) : (
                        <span className="bn-badge bn-badge-info" title="All bid prices visible">Transparent</span>
                      )}
                    </td>
                    <td style={{ textAlign: 'right' }} onClick={(e) => e.stopPropagation()}>
                      <div className="bn-flex-end gap-1">
                        {isOrg && (
                          <>
                            <button
                              className="bn-icon-btn"
                              onClick={() => { setEditingAuction(auction); setIsFormOpen(true); }}
                              title="Edit auction"
                            >
                              <Edit size={14} />
                            </button>
                            <button
                              className="bn-icon-btn bn-text-danger"
                              onClick={() => setDeletingAuction(auction)}
                              title="Delete auction"
                            >
                              <Trash2 size={14} />
                            </button>
                          </>
                        )}
                        <Button variant="primary" size="sm" onClick={() => onOpenAuction(auction)}>
                          Desk →
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )
      ) : (
        <EmptyState
          title="No auctions found"
          description="No procurement auctions match your active search or filter criteria."
          actionText={isOrg ? 'Create New Auction' : 'Clear Filters'}
          onAction={isOrg ? () => { setEditingAuction(null); setIsFormOpen(true); } : () => { setSearchQuery(''); setStatusFilter('ALL'); setTypeFilter('ALL'); }}
        />
      )}

      {/* Form Modal for Create / Edit */}
      <AuctionFormModal
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        onSubmit={handleCreateSubmit}
        initialData={editingAuction}
        token={token}
        userOrgId={claims.userId}
      />

      {/* Delete Confirmation Modal */}
      <ConfirmDialog
        isOpen={Boolean(deletingAuction)}
        onClose={() => setDeletingAuction(null)}
        onConfirm={handleDeleteConfirm}
        title={`Delete Auction #${deletingAuction?.id}`}
        message={`Are you sure you want to delete auction "${deletingAuction?.auctionName || deletingAuction?.docNoYearly}"? This operation cannot be undone.`}
        confirmText="Delete Auction"
        variant="danger"
        loading={deleting}
      />
    </div>
  );
};
