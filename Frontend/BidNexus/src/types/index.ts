export type Role = 'Vendor' | 'Organization' | 'Unknown';

export type Page = 'overview' | 'auctions' | 'bids' | 'statements' | 'workspace';

export interface Claims {
  role: Role;
  userId: number;
  tenantId: number;
  email: string;
  name: string;
}

export interface Unit {
  id: number;
  unitName?: string;
  name?: string;
  alias?: string;
}

export interface Item {
  id: number;
  itemName?: string;
  name?: string;
  code?: string;
  about?: string;
}

export interface Requirement {
  id: number;
  lineNo: number;
  itemId?: number;
  item?: Item;
  unitId?: number;
  unit?: Unit;
  quantity: number;
  technicalSpecification?: string;
  documentAttachmentId?: number | null;
}

export interface AuctionRequirementSaveRequest {
  lineNo: number;
  itemId: number;
  technicalSpecification?: string;
  quantity: number;
  unitId: number;
  documentAttachmentId?: number | null;
}

export interface Auction {
  id: number;
  auctionName?: string;
  about?: string;
  docNoYearly: string;
  docDate: string;
  organizationId?: number;
  organization?: { id?: number; name?: string };
  auctionRequirements?: Requirement[];
  isForwardAuction: boolean;
  auctionStartTime: string;
  auctionEndTime: string;
  statusId?: number;
  statusName?: string;
  openToAll: boolean;
  isBidPriceHidden: boolean;
  docAttachmentId?: string | null;
}

export interface AuctionCreateRequest {
  auctionName: string;
  about: string;
  docNoYearly: string;
  docDate: string;
  isForwardAuction: boolean;
  auctionStartTime: string;
  auctionEndTime: string;
  docAttachmentId?: string | null;
  openToAll: boolean;
  isBidPriceHidden: boolean;
  organizationId: number;
  statusId: number;
  auctionRequirements: AuctionRequirementSaveRequest[];
}

export interface AuctionUpdateRequest extends AuctionCreateRequest {}

export interface BidTaxDetailSaveRequest {
  taxId?: number;
  taxName: string;
  taxCode: string;
  taxNatureId: number;
  chargeTypeId: number;
  taxValue: number;
  taxAmount: number;
}

export interface BidDetailSaveRequest {
  auctionRequirementId: number;
  rate: number;
  baseAmount: number;
  netAmount: number;
  taxes?: BidTaxDetailSaveRequest[];
}

export interface BidCreateRequest {
  auctionId: number;
  vendorId: number;
  basicAmount: number;
  taxAmount: number;
  discountAmount: number;
  netAmount: number;
  mainBidId?: number | null;
  bidRevisionNo: number;
  bidDetails: BidDetailSaveRequest[];
}

export interface Bid {
  id: number;
  auctionId?: number;
  vendorId: number;
  basicAmount: number;
  taxAmount: number;
  discountAmount: number;
  netAmount: number;
  createdAt: string;
  isCurrent: boolean;
  bidRevisionNo: number;
  vendor?: { id?: number; name?: string; userName?: string };
  bidDetails?: any[];
}

export interface Statement {
  id: number;
  auctionId: number;
  bidId: number;
  vendorId: number;
  vendorName: string;
  netAmount: number;
  rank: number;
  isWinner: boolean;
}

export interface LoginResponse {
  token?: string;
  accessToken?: string;
  message?: string;
  userId?: number;
  tenantId?: number;
  role?: string;
  name?: string;
}

export interface GlobalCategory {
  id: number;
  name?: string;
  code?: string;
}

export interface GlobalStatus {
  id: number;
  name?: string;
  code?: string;
}

export type SignalRStatus = 'CONNECTED' | 'CONNECTING' | 'RECONNECTING' | 'DISCONNECTED';
