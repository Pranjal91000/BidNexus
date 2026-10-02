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
import { ToastContainer, type ToastMessage } from './components/ui/Toast';
import { AuctionFormModal } from './components/auctions/AuctionFormModal';
import './App.css';

export function App() {
  const [token, setToken] = useState<string>(() => localStorage.getItem('bidnexus_token') || '');
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

  const loadAuctions = useCallback(async () => {
    if (!token) return;
    setLoadingAuctions(true);
    try {
      const data = await api.getAuctions(token, 0, 1, 100);
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

  const handleLogout = () => {
    localStorage.removeItem('bidnexus_token');
    setToken('');
    setSelectedAuction(null);
    setPage('overview');
    addToast('Session disconnected successfully.', 'info');
  };

  const handleOpenAuctionDesk = (auction: Auction) => {
    setSelectedAuction(auction);
    setPage('auctions');
  };

  // If no token, show Authentication Modal / Token Gate
  if (!token) {
    return <AuthModal onConnectToken={(newToken) => setToken(newToken)} />;
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
