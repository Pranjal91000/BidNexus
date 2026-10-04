export type Role = 'Vendor' | 'Organization' | 'Unknown';

export type Page = 'overview' | 'auctions' | 'masters' | 'profile' | 'bids' | 'statements' | 'workspace';

export interface Claims {
  role: Role;
  userId: number;
  tenantId: number;
  email: string;
  name: string;
  exp?: number;
}

export interface Unit {
  id: number;
  unitName?: string;
  name?: string;
  code?: string;
  alias?: string;
  statusId?: number;
  statusRemarks?: string;
}

export interface Item {
  id: number;
  itemName?: string;
  name?: string;
  code?: string;
  about?: string;
  itemDescription?: string;
  categoryId?: number;
  statusId?: number;
  applicableUnits?: any[];
  unitIds?: number[];
  docAttachmentId?: number | null;
}

export interface TaxMaster {
  id: number;
  name: string;
  code: string;
  taxNatureId: number;
  chargeTypeId: number;
  taxValue: number;
  statusId: number;
  statusRemarks?: string;
  createdDateTime?: string;
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
  taxId?: number | null;
  taxName: string;
  taxCode: string;
  taxNatureId: number;
  chargeTypeId: number;
  taxValue: number;
  taxAmount: number;
}

export interface AppliedTaxItem {
  id?: number | null;
  tempKey: string;
  name: string;
  code: string;
  taxNatureId: number;
  chargeTypeId: number;
  taxValue: number;
  isCustom?: boolean;
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
  /** True when the server withheld this competitor's amount (hidden-price auction). */
  amountHidden?: boolean;
  vendor?: { id?: number; name?: string; userName?: string };
  bidDetails?: any[];
}

export interface StatementLine {
  auctionRequirementId: number;
  lineNo: number;
  itemName: string;
  quantity: number;
  unitName: string;
  rate: number;
  baseAmount: number;
  netAmount: number;
}

export interface Statement {
  id: number;
  auctionId: number;
  bidId: number;
  vendorId: number;
  vendorName: string;
  basicAmount?: number;
  taxAmount?: number;
  netAmount: number;
  rank: number;
  isWinner: boolean;
  bidRevisionNo?: number;
  submittedAt?: string;
  lines?: StatementLine[];
}

export interface VendorAuctionResult {
  auctionId: number;
  participated: boolean;
  rank: number | null;
  bidders: number;
  isWinner: boolean;
  pricesHidden: boolean;
  myNetAmount: number | null;
  myBasicAmount: number | null;
  myTaxAmount: number | null;
  winningAmount: number | null;
  bidRevisionNo: number | null;
  lines: StatementLine[];
}

export interface BidActivity {
  at: string;
  netAmount: number | null;
  bidder: string;
  isMine: boolean;
  bidRevisionNo: number;
}

export interface MonthlyPoint {
  month: string;
  auctions: number;
  won: number;
  value: number;
}

export interface AuctionResultSummary {
  auctionId: number;
  auctionName: string;
  docNoYearly: string;
  closedAt: string;
  winnerName: string | null;
  winningAmount: number | null;
  bidders: number;
  priceImprovementPercent: number | null;
}

export interface OrganizationDashboard {
  live: number;
  upcoming: number;
  drafts: number;
  closedLast90Days: number;
  awardedValueLast90Days: number;
  averagePriceImprovementPercent: number | null;
  averageBiddersPerAuction: number;
  monthly: MonthlyPoint[];
  recentResults: AuctionResultSummary[];
}

export interface VendorLivePosition {
  auctionId: number;
  auctionName: string;
  endsAt: string;
  rank: number;
  bidders: number;
}

export interface VendorDashboard {
  liveParticipating: number;
  leading: number;
  upcomingOpen: number;
  participatedLast12Months: number;
  wonLast12Months: number;
  wonValueLast12Months: number;
  monthly: MonthlyPoint[];
  livePositions: VendorLivePosition[];
}

export interface LoginResponse {
  token?: string;
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
  refreshTokenExpiresAt?: string;
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

export const ChargeTypeEnum = {
  Fixed: 1,
  Percentage: 2,
  PerUnit: 3,
} as const;
export type ChargeTypeEnum = (typeof ChargeTypeEnum)[keyof typeof ChargeTypeEnum];

export const TaxNatureEnum = {
  Additive: 1,
  Deductive: 2,
} as const;
export type TaxNatureEnum = (typeof TaxNatureEnum)[keyof typeof TaxNatureEnum];

export interface ChargeType {
  id: number;
  name: string;
  code: string;
  isActive?: boolean;
}

export interface TaxNature {
  id: number;
  name: string;
  code: string;
  isActive?: boolean;
}

export interface RatingFor {
  id: number;
  name: string;
  code: string;
}

export interface RatingParameter {
  id: number;
  name: string;
  code: string;
  ratingForId?: number;
}

export interface RatingValueSubmit {
  ratingParameterId: number;
  score: number;
}

export interface RatingCreateRequest {
  auctionId: number;
  ratingForId: number;
  ratingValues: RatingValueSubmit[];
  remarks?: string;
}

export interface RatingParameterAverage {
  parameterId: number;
  parameterName: string;
  averageScore: number;
  ratingCount: number;
}

export interface TenantReputation {
  tenantId: number;
  averageRating: number;
  totalRatingsReceived: number;
  parameterBreakdown: RatingParameterAverage[];
}

export interface RatingSummary {
  id: number;
  auctionId: number;
  auctionName?: string;
  submittedByTenantId: number;
  ratingForId: number;
  ratingForName?: string;
  averageScore: number;
  remarks?: string;
  createdAt: string;
}

export type SignalRStatus = 'CONNECTED' | 'CONNECTING' | 'RECONNECTING' | 'DISCONNECTED';

