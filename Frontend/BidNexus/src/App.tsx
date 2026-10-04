import { useEffect, useState, useCallback } from 'react';
import type { Auction, Claims, Page } from './types';
import { api, parseJwtClaims } from './services/api';
import { Sidebar } from './components/shell/Sidebar';
import { TopBar } from './components/shell/TopBar';
import { MobileNav } from './components/shell/MobileNav';
import { AuthModal } from './components/auth/AuthModal';
import { OrganizationDashboard } from './components/dashboard/OrganizationDashboard';
import { VendorDashboard } from './components/dashboard/VendorDashboard';
import { AuctionRegister } from './components/auctions/AuctionRegister';
import { LiveAuctionWorkstation } from './components/live/LiveAuctionWorkstation';
import { MyBidsView } from './components/bids/MyBidsView';
import { AuctionStatementsRegister } from './components/statements/AuctionStatementsRegister';
import { WorkspaceSettings } from './components/workspace/WorkspaceSettings';
import { MastersView } from './components/masters/MastersView';
import { ToastContainer, type ToastMessage } from './components/ui/Toast';
import { AuctionFormModal } from './components/auctions/AuctionFormModal';
import './App.css';

export function App() {
  const [token, setToken] = useState<string>(() => {
    const refreshExpiresAtStr = localStorage.getItem('bidnexus_refresh_expires_at');
    if (refreshExpiresAtStr && Date.now() >= new Date(refreshExpiresAtStr).getTime()) {
      localStorage.removeItem('bidnexus_token');
      localStorage.removeItem('bidnexus_refresh_token');
      localStorage.removeItem('bidnexus_refresh_expires_at');
      return '';
    }
    return localStorage.getItem('bidnexus_token') || '';
  });
  const [sessionNotice, setSessionNotice] = useState<string>(() => {
    const refreshExpiresAtStr = localStorage.getItem('bidnexus_refresh_expires_at');
    if (refreshExpiresAtStr && Date.now() >= new Date(refreshExpiresAtStr).getTime()) {
      return 'Your previous 4-hour authorization session has expired. Please log in again.';
    }
    return '';
  });
  const [claims, setClaims] = useState<Claims>(() => parseJwtClaims(token));
  const [page, setPage] = useState<Page>('overview');

  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [selectedAuction, setSelectedAuction] = useState<Auction | null>(null);
  const [loadingAuctions, setLoadingAuctions] = useState<boolean>(false);

  const [searchQuery, setSearchQuery] = useState<string>('');
  const [isMobileNavOpen, setIsMobileNavOpen] = useState<boolean>(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState<boolean>(false);

  // Global Toast Messages
  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const addToast = useCallback((message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info', title?: string) => {
    const id = Math.random().toString(36).substr(2, 9);
    setToasts((prev) => [...prev, { id, message, type, title }]);
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id));
    }, 5000);
  }, []);

  const dismissToast = (id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  };

  const handleLogout = useCallback((reason?: string) => {
    localStorage.removeItem('bidnexus_token');
    localStorage.removeItem('bidnexus_refresh_token');
    localStorage.removeItem('bidnexus_refresh_expires_at');
    setToken('');
    setSelectedAuction(null);
    setPage('overview');
    if (reason) {
      setSessionNotice(reason);
      addToast(reason, 'warning', 'Session Notice');
    } else {
      setSessionNotice('');
      addToast('Session disconnected successfully.', 'info');
    }
  }, [addToast]);

  const handleConnectToken = (
    newToken: string,
    newRefreshToken?: string,
    _expiresAt?: string,
    refreshExpiresAt?: string
  ) => {
    localStorage.setItem('bidnexus_token', newToken);
    if (newRefreshToken) {
      localStorage.setItem('bidnexus_refresh_token', newRefreshToken);
    }
    if (refreshExpiresAt) {
      localStorage.setItem('bidnexus_refresh_expires_at', refreshExpiresAt);
    } else {
      const fourHoursFromNow = new Date(Date.now() + 4 * 60 * 60 * 1000).toISOString();
      localStorage.setItem('bidnexus_refresh_expires_at', fourHoursFromNow);
    }
    setSessionNotice('');
    setToken(newToken);
  };

  const loadAuctions = useCallback(async () => {
    if (!token) return;
    setLoadingAuctions(true);
    try {
      const parsed = parseJwtClaims(token);
      const isVendor = parsed.role === 'Vendor';
      // Vendors access all open/pending auctions across organizations without tenant filtering
      const data = isVendor
        ? await api.getPendingAuctions(token, null, 1, 100)
        : await api.getAuctions(token, null, 1, 100);
      setAuctions(data || []);
    } catch (err: any) {
      addToast(err instanceof Error ? err.message : 'Unable to load auctions from backend.', 'error');
    } finally {
      setLoadingAuctions(false);
    }
  }, [token, addToast]);

  useEffect(() => {
    if (token) {
      localStorage.setItem('bidnexus_token', token);
      const parsed = parseJwtClaims(token);
      setClaims(parsed);
      loadAuctions();
    } else {
      localStorage.removeItem('bidnexus_token');
      setClaims({ role: 'Unknown', userId: 0, tenantId: 0, email: '', name: 'Guest User' });
    }
  }, [token, loadAuctions]);

  // Session expiry and background token refresh event listeners
  useEffect(() => {
    const onSessionExpired = (e: any) => {
      const msg = e.detail || 'Session expired after 4 hours. Please log in again.';
      handleLogout(msg);
    };

    const onTokenRefreshed = (e: any) => {
      if (e.detail?.accessToken) {
        setToken(e.detail.accessToken);
        setClaims(parseJwtClaims(e.detail.accessToken));
      }
    };

    window.addEventListener('bidnexus:session-expired', onSessionExpired);
    window.addEventListener('bidnexus:token-refreshed', onTokenRefreshed);

    return () => {
      window.removeEventListener('bidnexus:session-expired', onSessionExpired);
      window.removeEventListener('bidnexus:token-refreshed', onTokenRefreshed);
    };
  }, [handleLogout]);

  // Periodic token lifecycle monitor (4-hour maximum validity & proactive refresh)
  useEffect(() => {
    if (!token) return;

    const interval = setInterval(async () => {
      const refreshExpiresAtStr = localStorage.getItem('bidnexus_refresh_expires_at');
      if (refreshExpiresAtStr) {
        const refreshExpiresTime = new Date(refreshExpiresAtStr).getTime();
        if (Date.now() >= refreshExpiresTime) {
          handleLogout('Your 4-hour authorization session has expired. Please log in again.');
          return;
        }
      }

      // Check access token expiration (proactive refresh if <= 2 minutes remaining)
      const parsed = parseJwtClaims(token);
      if (parsed.exp) {
        const expTimeMs = parsed.exp * 1000;
        const remainingMinutes = (expTimeMs - Date.now()) / (1000 * 60);
        if (remainingMinutes <= 2) {
          const refreshToken = localStorage.getItem('bidnexus_refresh_token');
          if (refreshToken) {
            try {
              await api.doRefreshToken();
            } catch {
              // Ignore; request interceptor will handle on 401
            }
          }
        }
      }
    }, 30000);

    return () => clearInterval(interval);
  }, [token, handleLogout]);

  const handleOpenAuctionDesk = (auction: Auction) => {
    setSelectedAuction(auction);
    setPage('auctions');
  };

  // If no token, show Authentication Modal / Token Gate
  if (!token) {
    return (
      <>
        <ToastContainer toasts={toasts} onDismiss={dismissToast} />
        <AuthModal
          onConnectToken={handleConnectToken}
          sessionNotice={sessionNotice}
        />
      </>
    );
  }

  return (
    <div className="bn-app-shell">
      {/* Desktop Sidebar */}
      <Sidebar
        page={page}
        setPage={setPage}
        claims={claims}
        signalRStatus={'CONNECTED'}
        onLogout={handleLogout}
        onClearSelectedAuction={() => setSelectedAuction(null)}
      />

      {/* Mobile Drawer Navigation */}
      <MobileNav
        isOpen={isMobileNavOpen}
        onClose={() => setIsMobileNavOpen(false)}
        page={page}
        setPage={setPage}
        claims={claims}
        signalRStatus={'CONNECTED'}
        onLogout={handleLogout}
        onClearSelectedAuction={() => setSelectedAuction(null)}
      />

      {/* Main Workspace Area */}
      <main className="bn-main-workspace">
        <TopBar
          page={page}
          selectedAuctionName={selectedAuction?.auctionName || selectedAuction?.docNoYearly || null}
          claims={claims}
          searchQuery={searchQuery}
          setSearchQuery={setSearchQuery}
          onRefresh={loadAuctions}
          signalRStatus={'CONNECTED'}
          onOpenMobileNav={() => setIsMobileNavOpen(true)}
        />

        <div className="bn-page-container">
          {/* Workstation page view when an auction is explicitly selected */}
          {selectedAuction ? (
            <LiveAuctionWorkstation
              auction={selectedAuction}
              token={token}
              claims={claims}
              onBack={() => setSelectedAuction(null)}
              onShowToast={addToast}
            />
          ) : (
            <>
              {/* Overview Page (Role Adapted) */}
              {page === 'overview' && (
                claims.role === 'Organization' ? (
                  <OrganizationDashboard
                    auctions={auctions}
                    loading={loadingAuctions}
                    onOpenAuction={handleOpenAuctionDesk}
                    onCreateAuction={() => { setPage('auctions'); setIsCreateModalOpen(true); }}
                    onGoToRegister={() => setPage('auctions')}
                  />
                ) : (
                  <VendorDashboard
                    auctions={auctions}
                    loading={loadingAuctions}
                    onOpenAuction={handleOpenAuctionDesk}
                    onGoToRegister={() => setPage('auctions')}
                    onGoToMyBids={() => setPage('bids')}
                  />
                )
              )}

              {/* Auction Register Page */}
              {page === 'auctions' && (
                <AuctionRegister
                  auctions={auctions}
                  loading={loadingAuctions}
                  claims={claims}
                  token={token}
                  onOpenAuction={handleOpenAuctionDesk}
                  onRefresh={loadAuctions}
                  searchQuery={searchQuery}
                  setSearchQuery={setSearchQuery}
                />
              )}

              {/* My Bids Page (Vendor) */}
              {page === 'bids' && (
                <MyBidsView
                  auctions={auctions}
                  token={token}
                  claims={claims}
                  onOpenAuction={handleOpenAuctionDesk}
                  onShowToast={addToast}
                />
              )}

              {/* Statements Page (Organization) */}
              {page === 'statements' && (
                <AuctionStatementsRegister
                  auctions={auctions}
                  token={token}
                  onOpenAuction={handleOpenAuctionDesk}
                  onShowToast={addToast}
                />
              )}

              {/* Master Catalog Page */}
              {page === 'masters' && (
                <MastersView claims={claims} token={token} onShowToast={addToast} />
              )}

              {/* Workspace Settings Page */}
              {page === 'workspace' && (
                <WorkspaceSettings claims={claims} token={token} />
              )}
            </>
          )}
        </div>
      </main>

      {/* Global Toast Container */}
      <ToastContainer toasts={toasts} onDismiss={dismissToast} />

      {/* Global Create Auction Modal Triggered from Dashboard */}
      {isCreateModalOpen && (
        <AuctionFormModal
          isOpen={isCreateModalOpen}
          onClose={() => setIsCreateModalOpen(false)}
          onSubmit={async (data) => {
            await api.createAuction(token, data);
            loadAuctions();
            addToast('Auction created successfully', 'success');
          }}
          token={token}
          userOrgId={claims.userId}
        />
      )}
    </div>
  );
}

export default App;
