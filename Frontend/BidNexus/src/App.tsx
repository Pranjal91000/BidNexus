import { useCallback, useEffect, useMemo, useState } from 'react';
import type { Auction, Claims, OrganizationDashboard, VendorDashboard } from './types';
import { api, parseJwtClaims } from './services/api';
import { AppContext, errorMessage, type AppContextValue } from './lib/appContext';
import { useRoute } from './lib/router';
import { Shell } from './components/shell/Shell';
import { ErrorBoundary } from './components/shell/ErrorBoundary';
import { Toasts, type ToastMessage, type ToastTone } from './components/ui/Toast';
import { AuthScreen } from './features/auth/AuthScreen';
import { OrgOverview } from './features/overview/OrgOverview';
import { VendorOverview } from './features/overview/VendorOverview';
import { AuctionsPage } from './features/auctions/AuctionsPage';
import { AuctionDetail } from './features/auctions/detail/AuctionDetail';
import { AuctionFormModal } from './features/auctions/AuctionFormModal';
import { MastersPage } from './features/masters/MastersPage';
import { ProfilePage } from './features/profile/ProfilePage';

const TOKEN_KEY = 'bidnexus_token';
const REFRESH_KEY = 'bidnexus_refresh_token';
const REFRESH_EXPIRES_KEY = 'bidnexus_refresh_expires_at';
const GUEST: Claims = { role: 'Unknown', userId: 0, tenantId: 0, email: '', name: 'Guest' };

const sessionExpired = () => {
  const at = localStorage.getItem(REFRESH_EXPIRES_KEY);
  return Boolean(at && Date.now() >= new Date(at).getTime());
};

const clearSession = () => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_KEY);
  localStorage.removeItem(REFRESH_EXPIRES_KEY);
};

export default function App() {
  const [token, setToken] = useState<string>(() => {
    if (sessionExpired()) {
      clearSession();
      return '';
    }
    return localStorage.getItem(TOKEN_KEY) || '';
  });
  const [sessionNotice, setSessionNotice] = useState(() => (sessionExpired() ? 'Your session expired. Please sign in again.' : ''));
  const claims = useMemo(() => (token ? parseJwtClaims(token) : GUEST), [token]);
  const isOrg = claims.role === 'Organization';
  const isVendor = claims.role === 'Vendor';

  const [route, navigate] = useRoute();
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [auctionsLoading, setAuctionsLoading] = useState(false);
  const [orgDashboard, setOrgDashboard] = useState<OrganizationDashboard | null>(null);
  const [vendorDashboard, setVendorDashboard] = useState<VendorDashboard | null>(null);
  const [formAuction, setFormAuction] = useState<Auction | null | undefined>(undefined);
  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const toast = useCallback((message: string, type: ToastTone = 'info') => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, message, type }]);
    window.setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 5000);
  }, []);

  const logout = useCallback((reason?: string) => {
    clearSession();
    setToken('');
    setAuctions([]);
    setOrgDashboard(null);
    setVendorDashboard(null);
    setSessionNotice(reason || '');
    window.location.hash = '#/overview';
  }, []);

  const refresh = useCallback(async () => {
    if (!token) return;
    const role = parseJwtClaims(token).role;
    setAuctionsLoading(true);
    try {
      const list = role === 'Vendor' ? await api.getPendingAuctions(token, null, 1, 100) : await api.getAuctions(token, null, 1, 100);
      setAuctions(list || []);
    } catch (err) {
      toast(errorMessage(err, 'Could not load auctions.'), 'error');
    } finally {
      setAuctionsLoading(false);
    }
    // Dashboard numbers are a nice-to-have; a failure here must not block the app.
    try {
      if (role === 'Organization') setOrgDashboard(await api.getOrganizationDashboard(token));
      if (role === 'Vendor') setVendorDashboard(await api.getVendorDashboard(token));
    } catch {
      /* dashboard endpoint unavailable — the overview falls back to list-derived numbers */
    }
  }, [token, toast]);

  useEffect(() => {
    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
      // eslint-disable-next-line react-hooks/set-state-in-effect -- load data once signed in
      refresh();
    }
  }, [token, refresh]);

  // Session lifecycle: forced logout and silent refresh events from the API client.
  useEffect(() => {
    const onExpired = (e: Event) => logout((e as CustomEvent).detail || 'Your session expired. Please sign in again.');
    const onRefreshed = (e: Event) => {
      const next = (e as CustomEvent).detail?.accessToken;
      if (next) setToken(next);
    };
    window.addEventListener('bidnexus:session-expired', onExpired);
    window.addEventListener('bidnexus:token-refreshed', onRefreshed);
    return () => {
      window.removeEventListener('bidnexus:session-expired', onExpired);
      window.removeEventListener('bidnexus:token-refreshed', onRefreshed);
    };
  }, [logout]);

  // Proactive refresh shortly before the access token expires.
  useEffect(() => {
    if (!token) return;
    const id = window.setInterval(async () => {
      if (sessionExpired()) {
        logout('Your session expired. Please sign in again.');
        return;
      }
      const exp = parseJwtClaims(token).exp;
      if (exp && exp * 1000 - Date.now() < 2 * 60 * 1000 && localStorage.getItem(REFRESH_KEY)) {
        try {
          await api.doRefreshToken();
        } catch {
          /* the request layer retries on 401 */
        }
      }
    }, 30000);
    return () => window.clearInterval(id);
  }, [token, logout]);

  const handleSignedIn = (accessToken: string, refreshToken?: string, refreshExpiresAt?: string) => {
    localStorage.setItem(TOKEN_KEY, accessToken);
    if (refreshToken) localStorage.setItem(REFRESH_KEY, refreshToken);
    localStorage.setItem(REFRESH_EXPIRES_KEY, refreshExpiresAt || new Date(Date.now() + 4 * 3600 * 1000).toISOString());
    setSessionNotice('');
    setToken(accessToken);
  };

  const dismissToast = (id: string) => setToasts((p) => p.filter((t) => t.id !== id));

  if (!token) {
    return (
      <>
        <AuthScreen notice={sessionNotice} onSignedIn={handleSignedIn} />
        <Toasts toasts={toasts} onDismiss={dismissToast} />
      </>
    );
  }

  const ctx: AppContextValue = {
    token,
    claims,
    isOrg,
    isVendor,
    auctions,
    auctionsLoading,
    orgDashboard,
    vendorDashboard,
    refresh,
    navigate,
    openAuction: (id) => navigate({ page: 'auctions', auctionId: id }),
    openAuctionForm: (auction) => setFormAuction(auction ?? null),
    toast,
  };

  let content;
  if (route.page === 'auctions' && route.auctionId) {
    content = <AuctionDetail key={route.auctionId} auctionId={route.auctionId} />;
  } else if (route.page === 'auctions') {
    content = <AuctionsPage tab={route.tab} />;
  } else if (route.page === 'masters') {
    content = <MastersPage />;
  } else if (route.page === 'profile') {
    content = <ProfilePage />;
  } else {
    content = isOrg ? <OrgOverview /> : <VendorOverview />;
  }

  return (
    <AppContext.Provider value={ctx}>
      <Shell page={route.page} claims={claims} onNavigate={(page) => navigate({ page })} onLogout={() => logout()}>
        <ErrorBoundary resetKey={window.location.hash}>{content}</ErrorBoundary>
      </Shell>

      {formAuction !== undefined && isOrg && (
        <AuctionFormModal
          auction={formAuction}
          onClose={() => setFormAuction(undefined)}
          onSaved={async (id) => {
            setFormAuction(undefined);
            await refresh();
            if (id) navigate({ page: 'auctions', auctionId: id });
          }}
        />
      )}

      <Toasts toasts={toasts} onDismiss={dismissToast} />
    </AppContext.Provider>
  );
}
