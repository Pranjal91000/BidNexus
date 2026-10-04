import { createContext, useContext } from 'react';
import type { Auction, Claims, OrganizationDashboard, VendorDashboard } from '../types';
import type { Route } from './router';
import type { ToastTone } from '../components/ui/Toast';

export interface AppContextValue {
  token: string;
  claims: Claims;
  isOrg: boolean;
  isVendor: boolean;
  auctions: Auction[];
  auctionsLoading: boolean;
  orgDashboard: OrganizationDashboard | null;
  vendorDashboard: VendorDashboard | null;
  /** Reloads the auction list and the dashboard numbers. */
  refresh: () => Promise<void>;
  navigate: (r: Partial<Route> & { page: Route['page'] }) => void;
  openAuction: (id: number) => void;
  /** Opens the create (no argument) or edit auction form. */
  openAuctionForm: (auction?: Auction) => void;
  toast: (message: string, type?: ToastTone) => void;
}

export const AppContext = createContext<AppContextValue | null>(null);

export function useApp(): AppContextValue {
  const ctx = useContext(AppContext);
  if (!ctx) throw new Error('useApp must be used inside <AppContext.Provider>');
  return ctx;
}

export const errorMessage = (err: unknown, fallback: string) => (err instanceof Error && err.message ? err.message : fallback);
