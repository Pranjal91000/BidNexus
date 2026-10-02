import type {
  Claims,
  Role,
  Auction,
  AuctionCreateRequest,
  AuctionUpdateRequest,
  Bid,
  BidCreateRequest,
  Statement,
  Item,
  Unit,
  GlobalCategory,
  GlobalStatus
} from '../types';

export const API_BASE_URL =
  import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'http://localhost:5000' : '');

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

    return { role, userId, tenantId, email, name };
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

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const url = `${API_BASE_URL}${path}`;

  try {
    const res = await fetch(url, { ...options, headers });
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
  login: async (username: string, password: string) => {
    return request<{ token?: string; accessToken?: string;[key: string]: any }>(`/login?Username=${username}&Password=${password}`, undefined, {
      method: 'POST'
    });
  },

  register: async (data: any) => {
    return request<any>('/Registration', undefined, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  // Auctions
  getAuctions: async (token: string, categoryId = 0, pageNo = 1, pageSize = 100): Promise<Auction[]> => {
    return request<Auction[]>(`/api/auctions?categoryId=${categoryId}&pageNo=${pageNo}&pageSize=${pageSize}`, token);
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

  // Bids
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

  // Masters
  getItems: async (token: string): Promise<Item[]> => {
    try {
      return await request<Item[]>('/api/masters/items', token);
    } catch {
      return [];
    }
  },

  getUnits: async (token: string): Promise<Unit[]> => {
    try {
      return await request<Unit[]>('/api/masters/units', token);
    } catch {
      return [];
    }
  },

  getCategories: async (token: string): Promise<GlobalCategory[]> => {
    try {
      return await request<GlobalCategory[]>('/api/global-data/categories', token);
    } catch {
      return [];
    }
  },

  getStatuses: async (token: string): Promise<GlobalStatus[]> => {
    try {
      return await request<GlobalStatus[]>('/api/global-data/statuses', token);
    } catch {
      return [];
    }
  }
};
