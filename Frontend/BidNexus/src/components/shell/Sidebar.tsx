import React from 'react';
import {
  LayoutDashboard,
  Gavel,
  TrendingUp,
  FileSpreadsheet,
  Settings,
  LogOut,
  Wifi,
  ShieldCheck,
  Building2,
  Store,
  Database
} from 'lucide-react';
import type { Claims, Page, SignalRStatus } from '../../types';

interface SidebarProps {
  page: Page;
  setPage: (page: Page) => void;
  claims: Claims;
  signalRStatus: SignalRStatus;
  onLogout: () => void;
  onClearSelectedAuction?: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({
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
  };

  return (
    <aside className="bn-sidebar">
      <div className="bn-sidebar-brand" onClick={() => navigate('overview')}>
        <div className="bn-brand-logo">
          <span>B</span>
        </div>
        <div className="bn-brand-text">
          <span className="bn-brand-name">Bid<span className="bn-brand-accent">Nexus</span></span>
          <span className="bn-brand-tagline">Procurement Command Center</span>
        </div>
      </div>

      <div className="bn-user-card">
        <div className="bn-avatar">
          {claims.role === 'Organization' ? <Building2 size={16} /> : <Store size={16} />}
        </div>
        <div className="bn-user-info">
          <span className="bn-user-name" title={claims.name}>{claims.name}</span>
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
          className={`bn-nav-item ${page === 'masters' ? 'active' : ''}`}
          onClick={() => navigate('masters')}
        >
          <Database size={18} />
          <span>Master Catalog</span>
        </button>

        <button
          className={`bn-nav-item ${page === 'workspace' ? 'active' : ''}`}
          onClick={() => navigate('workspace')}
        >
          <Settings size={18} />
          <span>Workspace</span>
        </button>
      </nav>

      <div className="bn-sidebar-footer">
        <div className="bn-connection-status">
          <span className={`bn-status-indicator bn-status-${signalRStatus.toLowerCase()}`}>
            <Wifi size={13} />
          </span>
          <div className="bn-connection-info">
            <span className="bn-connection-text">
              {signalRStatus === 'CONNECTED'
                ? 'SignalR Live'
                : signalRStatus === 'CONNECTING'
                ? 'Connecting Hub...'
                : signalRStatus === 'RECONNECTING'
                ? 'Reconnecting...'
                : 'API Connected'}
            </span>
            <span className="bn-security-tag">
              <ShieldCheck size={11} /> Tenant Isolated
            </span>
          </div>
        </div>

        <button className="bn-logout-btn" onClick={onLogout} title="Disconnect session">
          <LogOut size={16} />
          <span>Disconnect</span>
        </button>
      </div>
    </aside>
  );
};
