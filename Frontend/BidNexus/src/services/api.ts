import type {
  Claims,
  Role,
  Auction,
  AuctionCreateRequest,
  AuctionUpdateRequest,
  Bid,
  BidCreateRequest,
  Statement,
  VendorAuctionResult,
  BidActivity,
  OrganizationDashboard,
  VendorDashboard,
  Item,
  Unit,
  GlobalCategory,
  GlobalStatus,
  ChargeType,
  TaxNature,
  RatingFor,
  RatingParameter,
  RatingCreateRequest,
  TenantReputation,
  RatingSummary,
  TaxMaster,
  LoginResponse
} from '../types';

// Production is served through the same Nginx origin, which proxies API and SignalR
// traffic to the Railway backend. This avoids browser-side CORS entirely.
export const API_BASE_URL = import.meta.env.DEV
  ? (import.meta.env.VITE_API_BASE_URL || import.meta.env.VITE_API_URL || 'http://localhost:5000')
  : '';

let refreshPromise: Promise<string | null> | null = null;

export async function doRefreshToken(): Promise<string | null> {
  const currentRefreshToken = localStorage.getItem('bidnexus_refresh_token');
  if (!currentRefreshToken) {
    return null;
  }

  try {
    const res = await fetch(`${API_BASE_URL}/refresh`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ refreshToken: currentRefreshToken }),
    });

    if (!res.ok) {
      throw new Error('Refresh token invalid or expired');
    }

    const data: LoginResponse = await res.json();
    const newAccessToken = data.accessToken || data.token;
    if (newAccessToken) {
      localStorage.setItem('bidnexus_token', newAccessToken);
      if (data.refreshToken) {
        localStorage.setItem('bidnexus_refresh_token', data.refreshToken);
      }
      if (data.refreshTokenExpiresAt) {
        localStorage.setItem('bidnexus_refresh_expires_at', data.refreshTokenExpiresAt);
      }
      window.dispatchEvent(
        new CustomEvent('bidnexus:token-refreshed', {
          detail: { accessToken: newAccessToken, refreshToken: data.refreshToken }
        })
      );
      return newAccessToken;
    }
    return null;
  } catch (err) {
    localStorage.removeItem('bidnexus_token');
    localStorage.removeItem('bidnexus_refresh_token');
    localStorage.removeItem('bidnexus_refresh_expires_at');
    window.dispatchEvent(
      new CustomEvent('bidnexus:session-expired', {
        detail: 'Session expired after 4 hours. Please log in again.'
      })
    );
    return null;
  }
}

export function parseJwtClaims(token: string): Claims {
  if (!token) {
    return { role: 'Unknown', userId: 0, tenantId: 0, email: '', name: 'Guest User' };
  }
  try {
    const base64Url = token.split('.')[1];
    if (!base64Url) throw new Error('Invalid JWT');
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    const p = JSON.parse(jsonPayload);

    const rawRole =
      p.role ||
      p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      p['Role'] ||
      '';

    let role: Role = 'Unknown';
    if (typeof rawRole === 'string') {
      if (rawRole.toLowerCase().includes('vendor')) role = 'Vendor';
      else if (rawRole.toLowerCase().includes('org')) role = 'Organization';
    } else if (Array.isArray(rawRole)) {
      if (rawRole.some((r) => String(r).toLowerCase().includes('vendor'))) role = 'Vendor';
      else if (rawRole.some((r) => String(r).toLowerCase().includes('org'))) role = 'Organization';
    }

    const userId = +(
      p.userId ||
      p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
      p.sub ||
      0
    );

    const tenantId = +(p.tenantId || p.TenantId || 0);

    const email =
      p.email ||
      p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
      '';

    const name =
      p.name ||
      p.unique_name ||
      p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      email ||
      `User #${userId || 'Desk'}`;

    const exp = typeof p.exp === 'number' ? p.exp : undefined;

    return { role, userId, tenantId, email, name, exp };
  } catch (err) {
    console.warn('Failed to parse JWT token claims', err);
    return { role: 'Unknown', userId: 0, tenantId: 0, email: '', name: 'BidNexus User' };
  }
}

export async function request<T>(path: string, token?: string, options: RequestInit = {}): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string> | undefined),
  };

  const activeToken = token || localStorage.getItem('bidnexus_token') || undefined;
  if (activeToken) {
    headers['Authorization'] = `Bearer ${activeToken}`;
  }

  const url = `${API_BASE_URL}${path}`;

  try {
    let res = await fetch(url, { ...options, headers });

    // Handle 401 Unauthorized by attempting automatic refresh token flow
    if (res.status === 401 && !path.includes('/login') && !path.includes('/refresh')) {
      if (!refreshPromise) {
        refreshPromise = doRefreshToken().finally(() => {
          refreshPromise = null;
        });
      }

      const refreshedToken = await refreshPromise;
      if (refreshedToken) {
        headers['Authorization'] = `Bearer ${refreshedToken}`;
        res = await fetch(url, { ...options, headers });
      } else {
        throw new Error('Session expired. Please log in again.');
      }
    }

    if (!res.ok) {
      const errorText = await res.text();
      let errorMsg = errorText || `Request failed with status ${res.status}`;
      try {
        const parsed = JSON.parse(errorText);
        if (parsed.message) errorMsg = parsed.message;
        else if (typeof parsed === 'object') {
          const keys = Object.keys(parsed);
          if (keys.length > 0) {
            const val = parsed[keys[0]];
            if (Array.isArray(val)) errorMsg = val.join(', ');
            else if (typeof val === 'string') errorMsg = val;
          }
        }
      } catch {
        // use errorMsg fallback
      }
      throw new Error(errorMsg);
    }

    if (res.status === 204) {
      return undefined as unknown as T;
    }

    return (await res.json()) as T;
  } catch (err: any) {
    if (err.name === 'TypeError' && err.message.includes('fetch')) {
      throw new Error(`Unable to connect to BidNexus backend at ${API_BASE_URL}. Ensure the server is online.`);
    }
    throw err;
  }
}

// Api helper methods
export const api = {
  // Authentication
  login: async (Username: string, Password: string) => {
    return request<LoginResponse>(`/login`, undefined, {
      method: 'POST',
      body: JSON.stringify({ Username, Password }),
    });
  },

  refreshToken: async (refreshToken: string) => {
    return request<LoginResponse>(`/refresh`, undefined, {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    });
  },

  doRefreshToken,

  register: async (data: any) => {
    return request<any>('/Registration', undefined, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  // Auctions
  getAuctions: async (token: string, categoryId = null, pageNo = 1, pageSize = 100): Promise<Auction[]> => {
    return request<Auction[]>(`/api/auctions?${categoryId !== null ? `categoryId=${categoryId}&` : ''}pageNo=${pageNo}&pageSize=${pageSize}`, token);
  },

  getPendingAuctions: async (token: string, categoryId = null, pageNo = 1, pageSize = 100): Promise<Auction[]> => {
    return request<Auction[]>(`/api/auctions/pending?${categoryId !== null ? `categoryId=${categoryId}&` : ''}pageNo=${pageNo}&pageSize=${pageSize}`, token);
  },

  getAuctionById: async (token: string, id: number): Promise<Auction> => {
    return request<Auction>(`/api/auctions/${id}`, token);
  },

  createAuction: async (token: string, data: AuctionCreateRequest): Promise<Auction> => {
    return request<Auction>('/api/auctions', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  updateAuction: async (token: string, id: number, data: AuctionUpdateRequest): Promise<Auction> => {
    return request<Auction>(`/api/auctions/${id}`, token, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  deleteAuction: async (token: string, id: number): Promise<void> => {
    return request<void>(`/api/auctions/${id}`, token, {
      method: 'DELETE',
    });
  },

  // Statements
  getAuctionStatement: async (token: string, auctionId: number): Promise<Statement[]> => {
    return request<Statement[]>(`/api/auctions/${auctionId}/statement`, token);
  },

  getMyResult: async (token: string, auctionId: number): Promise<VendorAuctionResult> => {
    return request<VendorAuctionResult>(`/api/auctions/${auctionId}/my-result`, token);
  },

  // Dashboards
  getOrganizationDashboard: async (token: string): Promise<OrganizationDashboard> => {
    return request<OrganizationDashboard>('/api/dashboard/organization', token);
  },

  getVendorDashboard: async (token: string): Promise<VendorDashboard> => {
    return request<VendorDashboard>('/api/dashboard/vendor', token);
  },

  // Bids
  getBidActivity: async (token: string, auctionId: number): Promise<BidActivity[]> => {
    return request<BidActivity[]>(`/api/bids/auction/${auctionId}/activity`, token);
  },

  submitBid: async (token: string, data: BidCreateRequest): Promise<Bid> => {
    return request<Bid>('/api/bids', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  getAuctionBids: async (token: string, auctionId: number): Promise<Bid[]> => {
    return request<Bid[]>(`/api/bids/auction/${auctionId}`, token);
  },

  getLeaderboard: async (token: string, auctionId: number): Promise<Bid[]> => {
    return request<Bid[]>(`/api/bids/auction/${auctionId}/leaderboard`, token);
  },

  getBidHistory: async (token: string, auctionId: number, vendorId: number): Promise<Bid[]> => {
    return request<Bid[]>(`/api/bids/auction/${auctionId}/history?vendorId=${vendorId}`, token);
  },

  // Masters & Global Data
  getItems: async (token: string): Promise<Item[]> => {
    return request<Item[]>('/api/masters/items', token);
  },

  createItem: async (token: string, data: any): Promise<Item> => {
    return request<Item>('/api/masters/items', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  updateItem: async (token: string, id: number, data: any): Promise<Item> => {
    return request<Item>(`/api/masters/items/${id}`, token, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  deleteItem: async (token: string, id: number): Promise<void> => {
    return request<void>(`/api/masters/items/${id}`, token, {
      method: 'DELETE',
    });
  },

  getUnits: async (token: string): Promise<Unit[]> => {
    return request<Unit[]>('/api/masters/units', token);
  },

  createUnit: async (token: string, data: any): Promise<Unit> => {
    return request<Unit>('/api/masters/units', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  updateUnit: async (token: string, id: number, data: any): Promise<Unit> => {
    return request<Unit>(`/api/masters/units/${id}`, token, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  deleteUnit: async (token: string, id: number): Promise<void> => {
    return request<void>(`/api/masters/units/${id}`, token, {
      method: 'DELETE',
    });
  },

  getTaxMasters: async (token: string): Promise<TaxMaster[]> => {
    return request<TaxMaster[]>('/api/masters/tax-masters', token);
  },

  createTaxMaster: async (token: string, data: any): Promise<TaxMaster> => {
    return request<TaxMaster>('/api/masters/tax-masters', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  updateTaxMaster: async (token: string, id: number, data: any): Promise<TaxMaster> => {
    return request<TaxMaster>(`/api/masters/tax-masters/${id}`, token, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  },

  deleteTaxMaster: async (token: string, id: number): Promise<void> => {
    return request<void>(`/api/masters/tax-masters/${id}`, token, {
      method: 'DELETE',
    });
  },

  getCategories: async (token: string): Promise<GlobalCategory[]> => {
    return request<GlobalCategory[]>('/api/global-data/categories', token);
  },

  getStatuses: async (token: string): Promise<GlobalStatus[]> => {
    return request<GlobalStatus[]>('/api/global-data/statuses', token);
  },

  getChargeTypes: async (token: string): Promise<ChargeType[]> => {
    return request<ChargeType[]>('/api/global-data/charge-types', token);
  },

  getTaxNatures: async (token: string): Promise<TaxNature[]> => {
    return request<TaxNature[]>('/api/global-data/tax-natures', token);
  },

  getRatingFors: async (token: string): Promise<RatingFor[]> => {
    return request<RatingFor[]>('/api/global-data/rating-fors', token);
  },

  getRatingParameters: async (token: string): Promise<RatingParameter[]> => {
    return request<RatingParameter[]>('/api/global-data/rating-parameters', token);
  },

  // Ratings
  submitRating: async (token: string, data: RatingCreateRequest): Promise<any> => {
    return request<any>('/api/ratings', token, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  getTenantReputation: async (token: string, tenantId: number): Promise<TenantReputation> => {
    return request<TenantReputation>(`/api/ratings/tenant/${tenantId}/reputation`, token);
  },

  getTenantRatingHistory: async (token: string, tenantId: number): Promise<RatingSummary[]> => {
    return request<RatingSummary[]>(`/api/ratings/tenant/${tenantId}/history`, token);
  },

  getAuctionRatings: async (token: string, auctionId: number): Promise<RatingSummary[]> => {
    return request<RatingSummary[]>(`/api/ratings/auction/${auctionId}`, token);
  },

  getMyReputation: async (token: string): Promise<TenantReputation> => {
    return request<TenantReputation>('/api/ratings/my-reputation', token);
  }
};
