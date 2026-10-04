import { useCallback, useState, type ReactNode } from 'react';
import { Gavel, LayoutGrid, LogOut, Menu, Package, UserRound, X } from 'lucide-react';
import type { Claims, Page } from '../../types';
import { useEscape } from '../../lib/hooks';

const NAV: { id: Page; label: string; icon: ReactNode }[] = [
  { id: 'overview', label: 'Overview', icon: <LayoutGrid size={18} /> },
  { id: 'auctions', label: 'Auctions', icon: <Gavel size={18} /> },
  { id: 'masters', label: 'Masters', icon: <Package size={18} /> },
  { id: 'profile', label: 'Profile', icon: <UserRound size={18} /> },
];

interface ShellProps {
  page: Page;
  claims: Claims;
  onNavigate: (page: Page) => void;
  onLogout: () => void;
  children: ReactNode;
}

function Brand() {
  return (
    <a className="brand" href="#/overview">
      <span className="brand__mark" aria-hidden="true">B</span>
      BidNexus
    </a>
  );
}

function NavList({ page, onNavigate }: { page: Page; onNavigate: (p: Page) => void }) {
  return (
    <nav className="nav" aria-label="Main">
      {NAV.map((item) => (
        <button
          key={item.id}
          type="button"
          className={`nav__item${page === item.id ? ' nav__item--active' : ''}`}
          aria-current={page === item.id ? 'page' : undefined}
          onClick={() => onNavigate(item.id)}
        >
          {item.icon}
          {item.label}
        </button>
      ))}
    </nav>
  );
}

function Account({ claims, onLogout }: { claims: Claims; onLogout: () => void }) {
  return (
    <div className="sidebar__footer">
      <div className="stack-sm" style={{ gap: 0 }}>
        <span className="strong ellipsis">{claims.name}</span>
        <span className="small muted">{claims.role === 'Organization' ? 'Buyer organisation' : 'Vendor'}</span>
      </div>
      <button type="button" className="nav__item" onClick={onLogout} style={{ paddingLeft: 0 }}>
        <LogOut size={18} />
        Sign out
      </button>
    </div>
  );
}

export function Shell({ page, claims, onNavigate, onLogout, children }: ShellProps) {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const close = useCallback(() => setDrawerOpen(false), []);
  useEscape(drawerOpen, close);

  const go = (p: Page) => {
    setDrawerOpen(false);
    onNavigate(p);
  };

  return (
    <div className="app">
      <aside className="sidebar">
        <Brand />
        <NavList page={page} onNavigate={go} />
        <Account claims={claims} onLogout={onLogout} />
      </aside>

      <div className="main">
        <header className="topbar">
          <Brand />
          <button type="button" className="icon-btn" onClick={() => setDrawerOpen(true)} aria-label="Open menu">
            <Menu size={20} />
          </button>
        </header>
        {children}
      </div>

      {drawerOpen && (
        <>
          <div className="drawer-backdrop" onClick={close} />
          <div className="drawer" role="dialog" aria-modal="true" aria-label="Menu">
            <div className="row row--between" style={{ paddingRight: 4 }}>
              <Brand />
              <button type="button" className="icon-btn" onClick={close} aria-label="Close menu">
                <X size={20} />
              </button>
            </div>
            <NavList page={page} onNavigate={go} />
            <Account claims={claims} onLogout={onLogout} />
          </div>
        </>
      )}
    </div>
  );
}
