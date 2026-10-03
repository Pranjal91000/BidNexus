import React from 'react';
import type { Auction } from '../../types';
import { Badge } from '../ui/Badge';
import { Calendar, Layers, Eye, ShieldAlert, ArrowRight, Edit, Trash2 } from 'lucide-react';
import { Button } from '../ui/Button';

interface AuctionCardProps {
  auction: Auction;
  onOpen: (auction: Auction) => void;
  onEdit?: (auction: Auction) => void;
  onDelete?: (auction: Auction) => void;
  canManage?: boolean;
}

export const AuctionCard: React.FC<AuctionCardProps> = ({
  auction,
  onOpen,
  onEdit,
  onDelete,
  canManage = false,
}) => {
  const formatDate = (val?: string) => {
    if (!val) return '—';
    const d = new Date(val);
    return isNaN(d.getTime()) ? val : d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short' });
  };

  return (
    <div className="bn-auction-card">
      <div className="bn-card-top">
        <div className="bn-flex-center gap-2">
          <Badge tone={auction.statusName}>{auction.statusName || 'Scheduled'}</Badge>
          <span className="bn-type-badge">
            {auction.isForwardAuction ? 'FORWARD' : 'REVERSE'} AUCTION
          </span>
        </div>
        <span className="bn-card-id">#{auction.id}</span>
      </div>

      <div className="bn-card-main" onClick={() => onOpen(auction)}>
        <h3 className="bn-card-name" title={auction.auctionName || auction.docNoYearly}>
          {auction.auctionName || auction.docNoYearly}
        </h3>
        <p className="bn-card-org-name">{auction.organization?.name || auction.docNoYearly}</p>
        <p className="bn-card-about">{auction.about || 'B2B procurement auction line item requirements'}</p>
      </div>

      <div className="bn-card-specs">
        <div className="bn-spec-item">
          <Calendar size={13} />
          <span>Starts: {formatDate(auction.auctionStartTime)}</span>
        </div>
        <div className="bn-spec-item">
          <Layers size={13} />
          <span>{auction.auctionRequirements?.length || 0} Requirements</span>
        </div>
        <div className="bn-spec-item">
          {auction.isBidPriceHidden ? (
            <span className="bn-flex-center gap-1 bn-text-warning"><ShieldAlert size={13} /> Hidden Price</span>
          ) : (
            <span className="bn-flex-center gap-1"><Eye size={13} /> Transparent Price</span>
          )}
        </div>
      </div>

      <div className="bn-card-actions">
        <span className="bn-doc-no">{auction.docNoYearly}</span>
        <div className="bn-flex-center gap-2">
          {canManage && onEdit && (
            <Button
              variant="outline"
              size="sm"
              icon={<Edit size={14} />}
              onClick={(e) => { e.stopPropagation(); onEdit(auction); }}
              title="Edit auction details"
            >
              Edit
            </Button>
          )}
          {canManage && onDelete && (
            <Button
              variant="danger"
              size="sm"
              icon={<Trash2 size={14} />}
              onClick={(e) => { e.stopPropagation(); onDelete(auction); }}
              title="Delete auction"
            />
          )}
          <Button variant="primary" size="sm" onClick={() => onOpen(auction)} icon={<ArrowRight size={14} />}>
            Open Desk
          </Button>
        </div>
      </div>
    </div>
  );
};
