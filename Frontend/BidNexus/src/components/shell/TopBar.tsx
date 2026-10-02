import React from 'react';
import { Search, RefreshCw, Bell, Shield, Menu } from 'lucide-react';
import type { Claims, Page, SignalRStatus } from '../../types';

interface TopBarProps {
  page: Page;
  selectedAuctionName?: string | null;
  claims: Claims;
  searchQuery: string;
  setSearchQuery: (query: string) => void;
  onRefresh: () => void;
  signalRStatus: SignalRStatus;
  onOpenMobileNav: () => void;
}

export const TopBar: React.FC<TopBarProps> = ({
  page,
  selectedAuctionName,
  claims,
  searchQuery,
  setSearchQuery,
  onRefresh,
  signalRStatus,
  onOpenMobileNav,
}) => {
  const getBreadcrumbTitle = () => {
    if (selectedAuctionName) {
      return (
        <span className="bn-breadcrumb">
          <span className="bn-breadcrumb-item">Auctions</span>
          <span className="bn-breadcrumb-separator">/</span>
          <span className="bn-breadcrumb-current">{selectedAuctionName}</span>
        </span>
      );
    }

    switch (page) {
      case 'overview':
        return 'Operations Overview';
      case 'auctions':
        return 'Auction Register';
      case 'bids':
        return 'My Bidding Activity';
      case 'statements':
        return 'Auction Statements & Awards';
      case 'workspace':
        return 'Workspace & Tenant Settings';
      default:
        return 'Procurement Desk';
    }
  };

  return (
    <header className="bn-topbar">
      <div className="bn-topbar-left">
        <button
          className="bn-mobile-menu-trigger"
          onClick={onOpenMobileNav}
          aria-label="Open navigation menu"
        >
          <Menu size={20} />
        </button>

        <div className="bn-title-group">
          <span className="bn-eyebrow">PROCUREMENT COMMAND CENTER</span>
          <h1 className="bn-page-title">{getBreadcrumbTitle()}</h1>
        </div>
      </div>

      <div className="bn-topbar-right">
        {page === 'auctions' && !selectedAuctionName && (
          <div className="bn-search-box">
            <Search size={16} className="bn-search-icon" />
            <input
              type="text"
              className="bn-search-input"
              placeholder="Search auctions, doc #, orgs..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>
        )}

        <div className="bn-topbar-badge" title="Tenant Session">
          <Shield size={13} />
          <span>Tenant #{claims.tenantId || 'Scope'}</span>
        </div>

        <div className={`bn-signalr-pill bn-signalr-${signalRStatus.toLowerCase()}`}>
          <span className="bn-dot" />
          <span className="bn-pill-text">{signalRStatus}</span>
        </div>

        <button className="bn-icon-btn" onClick={onRefresh} title="Refresh workspace data">
          <RefreshCw size={16} />
        </button>

        <div className="bn-notification-btn" title="System notifications">
          <Bell size={16} />
          <span className="bn-dot-indicator" />
        </div>

        <div className="bn-user-avatar-pill">
          <span className="bn-avatar-initial">{claims.name[0]?.toUpperCase() || 'U'}</span>
        </div>
      </div>
    </header>
  );
};
