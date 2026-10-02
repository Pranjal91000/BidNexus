import React from 'react';
import { Drawer } from '../ui/Drawer';
import {
  LayoutDashboard,
  Gavel,
  TrendingUp,
  FileSpreadsheet,
  Settings,
  LogOut,
  Building2,
  Store,
  Wifi
} from 'lucide-react';
import type { Claims, Page, SignalRStatus } from '../../types';

interface MobileNavProps {
  isOpen: boolean;
  onClose: () => void;
  page: Page;
  setPage: (page: Page) => void;
  claims: Claims;
  signalRStatus: SignalRStatus;
  onLogout: () => void;
  onClearSelectedAuction?: () => void;
}

export const MobileNav: React.FC<MobileNavProps> = ({
  isOpen,
  onClose,
  page,
  setPage,
  claims,
  signalRStatus,
  onLogout,
  onClearSelectedAuction,
}) => {
  const navigate = (targetPage: Page) => {
    if (onClearSelectedAuction) onClearSelectedAuction();
    setPage(targetPage);
    onClose();
  };

  return (
    <Drawer isOpen={isOpen} onClose={onClose} title="BidNexus Navigation">
      <div className="bn-mobile-nav">
        <div className="bn-user-card bn-mb-4">
          <div className="bn-avatar">
            {claims.role === 'Organization' ? <Building2 size={16} /> : <Store size={16} />}
          </div>
          <div className="bn-user-info">
            <span className="bn-user-name">{claims.name}</span>
            <span className={`bn-role-badge bn-role-${claims.role.toLowerCase()}`}>
              {claims.role} Workspace
            </span>
          </div>
        </div>

        <nav className="bn-nav-menu">
          <button
            className={`bn-nav-item ${page === 'overview' ? 'active' : ''}`}
            onClick={() => navigate('overview')}
          >
            <LayoutDashboard size={18} />
            <span>Overview</span>
          </button>

          <button
            className={`bn-nav-item ${page === 'auctions' ? 'active' : ''}`}
            onClick={() => navigate('auctions')}
          >
            <Gavel size={18} />
            <span>Auction Register</span>
          </button>

          {claims.role === 'Vendor' && (
            <button
              className={`bn-nav-item ${page === 'bids' ? 'active' : ''}`}
              onClick={() => navigate('bids')}
            >
              <TrendingUp size={18} />
              <span>My Bids</span>
            </button>
          )}

          {claims.role === 'Organization' && (
            <button
              className={`bn-nav-item ${page === 'statements' ? 'active' : ''}`}
              onClick={() => navigate('statements')}
            >
              <FileSpreadsheet size={18} />
              <span>Statements</span>
            </button>
          )}

          <button
            className={`bn-nav-item ${page === 'workspace' ? 'active' : ''}`}
            onClick={() => navigate('workspace')}
          >
            <Settings size={18} />
            <span>Workspace</span>
          </button>
        </nav>

        <div className="bn-mobile-nav-footer bn-mt-6">
          <div className="bn-connection-status bn-mb-4">
            <span className={`bn-status-indicator bn-status-${signalRStatus.toLowerCase()}`}>
              <Wifi size={13} />
            </span>
            <span className="bn-connection-text">{signalRStatus}</span>
          </div>

          <button className="bn-btn bn-btn-danger bn-w-full" onClick={onLogout}>
            <LogOut size={16} />
            <span>Disconnect Session</span>
          </button>
        </div>
      </div>
    </Drawer>
  );
};
