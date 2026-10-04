import React, { useState, useEffect, useMemo } from 'react';
import type {
  Auction,
  Bid,
  BidDetailSaveRequest,
  TaxMaster,
  AppliedTaxItem,
  TaxNature,
  ChargeType,
} from '../../types';
import { ChargeTypeEnum, TaxNatureEnum } from '../../types';
import { Button } from '../ui/Button';
import { Modal } from '../ui/Modal';
import { Input, Select } from '../ui/Input';
import {
  TrendingDown,
  TrendingUp,
  AlertTriangle,
  CheckCircle2,
  ShieldCheck,
  Zap,
  Calculator,
} from 'lucide-react';
import { api } from '../../services/api';
import { buildBidTaxDetails, getTaxDisplayRate, calculateTaxAmount } from '../../utils/taxUtils';

interface BiddingPanelProps {
  auction: Auction;
  currentVendorBid: Bid | null;
  currentRank: number | null;
  leadingBidAmount: number | null;
  onSubmitBid: (data: {
    totalNet: number;
    totalBasic: number;
    totalTax: number;
    bidDetails: BidDetailSaveRequest[];
  }) => Promise<void>;
  submitting: boolean;
  disabledReason?: string | null;
  token: string;
}

interface LineItemInput {
  requirementId: number;
  lineNo: number;
  itemName: string;
  spec: string;
  quantity: number;
  unit: string;
  rate: string;
  appliedTaxes: AppliedTaxItem[];
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2,
  }).format(amount);
};

export const BiddingPanel: React.FC<BiddingPanelProps> = ({
  auction,
  currentVendorBid,
  currentRank,
  leadingBidAmount,
  onSubmitBid,
  submitting,
  disabledReason,
  token,
}) => {
  const isForward = Boolean(auction.isForwardAuction);
  const [masterTaxes, setMasterTaxes] = useState<TaxMaster[]>([]);
  const [taxNatures, setTaxNatures] = useState<TaxNature[]>([]);
  const [chargeTypes, setChargeTypes] = useState<ChargeType[]>([]);
  const [lineItems, setLineItems] = useState<LineItemInput[]>([]);
  const [validationError, setValidationError] = useState<string>('');
  const [bidStatusNotice, setBidStatusNotice] = useState<{ type: 'success' | 'error'; message: string } | null>(null);

  // Custom Tax Modal State
  const [customTaxModalReqId, setCustomTaxModalReqId] = useState<number | null>(null);
  const [customTaxName, setCustomTaxName] = useState('');
  const [customTaxCode, setCustomTaxCode] = useState('');
  const [customTaxNatureId, setCustomTaxNatureId] = useState<number>(TaxNatureEnum.Additive);
  const [customTaxChargeTypeId, setCustomTaxChargeTypeId] = useState<number>(ChargeTypeEnum.Percentage);
  const [customTaxValue, setCustomTaxValue] = useState<string>('5');
  const [customTaxError, setCustomTaxError] = useState('');

  // Load Tax Master catalog, natures, and charge types
  useEffect(() => {
    if (token) {
      Promise.all([
        api.getTaxMasters(token).catch(() => []),
        api.getTaxNatures(token).catch(() => []),
        api.getChargeTypes(token).catch(() => []),
      ]).then(([taxList, natureList, chargeList]) => {
        setMasterTaxes(taxList || []);
        setTaxNatures(natureList || []);
        setChargeTypes(chargeList || []);
      });
    }
  }, [token]);

  // Synchronize requirement line items
  useEffect(() => {
    const reqs = auction.auctionRequirements || [];
    const existingDetails = currentVendorBid?.bidDetails || [];

    const initialized: LineItemInput[] = reqs.map((r, idx) => {
      const matchedDetail = existingDetails.find((d: any) => d.auctionRequirementId === r.id);

      let initialRate = '';
      let initialAppliedTaxes: AppliedTaxItem[] = [];

      if (matchedDetail) {
        initialRate = matchedDetail.rate ? String(matchedDetail.rate) : '';
        if (matchedDetail.taxes && matchedDetail.taxes.length > 0) {
          initialAppliedTaxes = matchedDetail.taxes.map((t: any, tIdx: number) => {
            const master = t.taxId ? masterTaxes.find((m) => m.id === t.taxId) : null;
            return {
              id: t.taxId || null,
              tempKey: t.taxId ? `master-${t.taxId}` : `custom-${tIdx}-${t.taxCode || Date.now()}`,
              name: t.taxName || master?.name || (t.taxId ? `Tax #${t.taxId}` : 'Custom Tax'),
              code: t.taxCode || master?.code || '',
              taxNatureId: Number(t.taxNatureId) || master?.taxNatureId || TaxNatureEnum.Additive,
              chargeTypeId: Number(t.chargeTypeId) || master?.chargeTypeId || ChargeTypeEnum.Percentage,
              taxValue: Number(t.taxValue) ?? master?.taxValue ?? 0,
              isCustom: !t.taxId,
            };
          });
        }
      }

      return {
        requirementId: r.id,
        lineNo: r.lineNo || idx + 1,
        itemName: r.item?.itemName || r.item?.name || `Item #${r.itemId || r.id}`,
        spec: r.technicalSpecification || '',
        quantity: r.quantity || 1,
        unit: r.unit?.alias || r.unit?.name || r.unit?.unitName || 'unit',
        rate: initialRate,
        appliedTaxes: initialAppliedTaxes,
      };
    });

    setLineItems(initialized);
  }, [auction.auctionRequirements, currentVendorBid, masterTaxes]);

  // Handle individual line rate change
  const handleRateChange = (reqId: number, val: string) => {
    setValidationError('');
    setLineItems((prev) =>
      prev.map((item) => (item.requirementId === reqId ? { ...item, rate: val } : item))
    );
  };

  // Add catalog master tax to line item
  const handleAddMasterTax = (reqId: number, taxId: number) => {
    const master = masterTaxes.find((t) => t.id === taxId);
    if (!master) return;
    setLineItems((prev) =>
      prev.map((item) => {
        if (item.requirementId === reqId) {
          if (item.appliedTaxes.some((t) => t.id === taxId)) return item;
          const newTax: AppliedTaxItem = {
            id: master.id,
            tempKey: `master-${master.id}`,
            name: master.name,
            code: master.code,
            taxNatureId: master.taxNatureId,
            chargeTypeId: master.chargeTypeId,
            taxValue: Number(master.taxValue) || 0,
            isCustom: false,
          };
          return { ...item, appliedTaxes: [...item.appliedTaxes, newTax] };
        }
        return item;
      })
    );
  };

  // Remove tax from line item (works for master or custom tax)
  const handleRemoveTax = (reqId: number, tempKey: string) => {
    setLineItems((prev) =>
      prev.map((item) => {
        if (item.requirementId === reqId) {
          return {
            ...item,
            appliedTaxes: item.appliedTaxes.filter((t) => t.tempKey !== tempKey),
          };
        }
        return item;
      })
    );
  };

  // Open Custom Tax Modal for a specific requirement
  const openCustomTaxModal = (reqId: number, defaultName = '') => {
    setCustomTaxModalReqId(reqId);
    setCustomTaxName(defaultName);
    setCustomTaxCode(defaultName ? defaultName.replace(/[^a-zA-Z0-9]/g, '_').toUpperCase() : '');
    setCustomTaxNatureId(TaxNatureEnum.Additive);
    setCustomTaxChargeTypeId(ChargeTypeEnum.Percentage);
    setCustomTaxValue('5');
    setCustomTaxError('');
  };

  // Apply custom typed tax to line item
  const handleApplyCustomTax = (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    if (!customTaxName.trim()) {
      setCustomTaxError('Tax Name is required.');
      return;
    }
    if (!customTaxCode.trim()) {
      setCustomTaxError('Tax Code is required.');
      return;
    }
    const val = parseFloat(customTaxValue);
    if (isNaN(val) || val < 0) {
      setCustomTaxError('Tax value/rate must be a valid number >= 0.');
      return;
    }
    if (!customTaxModalReqId) return;

    const newTax: AppliedTaxItem = {
      id: null,
      tempKey: `custom-${Date.now()}-${Math.random().toString(36).substring(2, 6)}`,
      name: customTaxName.trim(),
      code: customTaxCode.trim().toUpperCase(),
      taxNatureId: customTaxNatureId,
      chargeTypeId: customTaxChargeTypeId,
      taxValue: val,
      isCustom: true,
    };

    setLineItems((prev) =>
      prev.map((item) => {
        if (item.requirementId === customTaxModalReqId) {
          return { ...item, appliedTaxes: [...item.appliedTaxes, newTax] };
        }
        return item;
      })
    );

    setCustomTaxModalReqId(null);
  };

  // Quick Adjustment across all lines proportionally
  const handleQuickDelta = (pct: number) => {
    setValidationError('');
    setLineItems((prev) =>
      prev.map((item) => {
        const currentRate = parseFloat(item.rate) || 0;
        if (currentRate <= 0) return item;
        const delta = (currentRate * pct) / 100;
        const newRate = isForward ? currentRate + delta : Math.max(0.01, currentRate - delta);
        return {
          ...item,
          rate: Number(newRate.toFixed(2)).toString(),
        };
      })
    );
  };

  // Item computations & aggregation
  const lineComputations = useMemo(() => {
    return lineItems.map((item) => {
      const rateNum = Math.max(0, parseFloat(item.rate) || 0);
      const baseAmount = Math.round(rateNum * item.quantity * 100) / 100;
      const { taxes: bidTaxDetails, totalTaxAmount: lineTaxAmount } = buildBidTaxDetails(
        item.appliedTaxes,
        baseAmount,
        item.quantity
      );
      const lineNetAmount = Math.round((baseAmount + lineTaxAmount) * 100) / 100;

      return {
        ...item,
        rateNum,
        baseAmount,
        bidTaxDetails,
        lineTaxAmount,
        lineNetAmount,
      };
    });
  }, [lineItems]);

  const totalBasic = useMemo(() => {
    return Math.round(lineComputations.reduce((sum, item) => sum + item.baseAmount, 0) * 100) / 100;
  }, [lineComputations]);

  const totalTax = useMemo(() => {
    return Math.round(lineComputations.reduce((sum, item) => sum + item.lineTaxAmount, 0) * 100) / 100;
  }, [lineComputations]);

  const totalNet = useMemo(() => {
    return Math.round(lineComputations.reduce((sum, item) => sum + item.lineNetAmount, 0) * 100) / 100;
  }, [lineComputations]);

  const validate = (): boolean => {
    if (lineItems.length === 0) {
      setValidationError('No requirement line items found in this auction.');
      return false;
    }

    for (const item of lineComputations) {
      if (item.rateNum <= 0) {
        setValidationError(`Line #${item.lineNo} (${item.itemName}): Enter a valid unit rate > 0.`);
        return false;
      }
      if (item.lineNetAmount <= 0) {
        setValidationError(`Line #${item.lineNo}: Net amount must be greater than zero.`);
        return false;
      }
    }

    if (totalNet <= 0) {
      setValidationError('Total Net Bid Amount must be greater than zero.');
      return false;
    }

    if (currentVendorBid) {
      if (!isForward && totalNet >= currentVendorBid.netAmount) {
        setValidationError(
          `In a Reverse Auction, your new bid (${formatCurrency(totalNet)}) must be LOWER than your current bid (${formatCurrency(currentVendorBid.netAmount)}).`
        );
        return false;
      }
      if (isForward && totalNet <= currentVendorBid.netAmount) {
        setValidationError(
          `In a Forward Auction, your new bid (${formatCurrency(totalNet)}) must be HIGHER than your current bid (${formatCurrency(currentVendorBid.netAmount)}).`
        );
        return false;
      }
    }

    setValidationError('');
    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setBidStatusNotice(null);

    if (!validate()) return;

    try {
      const bidDetails: BidDetailSaveRequest[] = lineComputations.map((item) => ({
        auctionRequirementId: item.requirementId,
        rate: item.rateNum,
        baseAmount: item.baseAmount,
        netAmount: item.lineNetAmount,
        taxes: item.bidTaxDetails,
      }));

      await onSubmitBid({
        totalNet,
        totalBasic,
        totalTax,
        bidDetails,
      });

      setBidStatusNotice({ type: 'success', message: 'Item-level bid calculation verified and accepted by procurement server!' });
    } catch (err: any) {
      setBidStatusNotice({
        type: 'error',
        message: err instanceof Error ? err.message : 'Authoritative bidding server rejected bid submission.',
      });
    }
  };

  // Preview computations for Custom Tax Modal
  const targetReqLine = lineItems.find((l) => l.requirementId === customTaxModalReqId);
  const targetRateNum = targetReqLine ? Math.max(0, parseFloat(targetReqLine.rate) || 0) : 0;
  const targetBase = targetReqLine ? Math.round(targetRateNum * targetReqLine.quantity * 100) / 100 : 0;
  const previewVal = parseFloat(customTaxValue) || 0;
  const previewTaxAmt = customTaxModalReqId && targetReqLine
    ? calculateTaxAmount(
        { chargeTypeId: customTaxChargeTypeId, taxNatureId: customTaxNatureId, taxValue: previewVal },
        targetBase,
        targetReqLine.quantity
      ).amount
    : 0;
  const isPreviewDeductive = customTaxNatureId === TaxNatureEnum.Deductive;

  return (
    <div className="bn-bidding-panel-container">
      {/* Bid status banner */}
      {bidStatusNotice && (
        <div className={`bn-bid-notice bn-bid-notice-${bidStatusNotice.type}`}>
          {bidStatusNotice.type === 'success' ? <CheckCircle2 size={16} /> : <AlertTriangle size={16} />}
          <span>{bidStatusNotice.message}</span>
        </div>
      )}

      {disabledReason ? (
        <div className="bn-bidding-disabled-banner">
          <AlertTriangle size={20} />
          <div>
            <strong>Bidding Locked</strong>
            <p>{disabledReason}</p>
          </div>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="bn-bidding-form">
          {/* Workstation Header */}
          <div className="bn-bidding-header-row">
            <div>
              <span className="bn-eyebrow">ACTIVE VENDOR TERMINAL</span>
              <h3 className="bn-bidding-title">
                {isForward ? 'Forward Multi-Item Bid Entry' : 'Reverse Multi-Item Procurement Bid'}
              </h3>
            </div>

            <div className="bn-market-context-chips">
              <div className="bn-context-chip">
                <span className="bn-context-chip-label">Your Current Rank</span>
                <span className={`bn-context-chip-value ${currentRank === 1 ? 'bn-text-success' : ''}`}>
                  {currentRank ? `#${currentRank}` : 'Unranked'}
                </span>
              </div>
              <div className="bn-context-chip">
                <span className="bn-context-chip-label">Market Leading Net</span>
                <span className="bn-context-chip-value bn-text-mono">
                  {leadingBidAmount !== null ? formatCurrency(leadingBidAmount) : '—'}
                </span>
              </div>
            </div>
          </div>

          {/* Quick Adjustment Delta Buttons */}
          <div className="bn-quick-deltas-bar">
            <span className="bn-quick-deltas-label">Quick Auto-Step All Lines:</span>
            <div className="bn-deltas-group">
              {[1, 2, 5, 10].map((pct) => (
                <button
                  key={pct}
                  type="button"
                  className="bn-delta-btn"
                  onClick={() => handleQuickDelta(pct)}
                  disabled={submitting}
                >
                  {isForward ? <TrendingUp size={12} /> : <TrendingDown size={12} />}
                  <span>{isForward ? `+${pct}%` : `-${pct}%`}</span>
                </button>
              ))}
            </div>
          </div>

          {/* Line Items Table */}
          <div className="bn-item-bidding-section">
            <div className="bn-item-bidding-table-header">
              <div className="bn-flex-center gap-2">
                <Calculator size={16} className="bn-text-cyan" />
                <span className="bn-font-bold bn-text-sm">Item Pricing & Applicable Taxes Computation Matrix</span>
              </div>
              <span className="bn-text-xs bn-text-muted">Enter unit rates and assign catalog or custom tax rules</span>
            </div>

            <div className="bn-item-bidding-table-wrapper">
              <table className="bn-table bn-item-bidding-table">
                <thead>
                  <tr>
                    <th style={{ width: '50px' }}>Line</th>
                    <th style={{ minWidth: '180px' }}>Procurement Material</th>
                    <th style={{ width: '80px' }}>Qty</th>
                    <th style={{ width: '80px' }}>Unit</th>
                    <th style={{ width: '130px' }}>Unit Rate (₹)</th>
                    <th style={{ width: '130px' }}>Base Amount</th>
                    <th style={{ minWidth: '220px' }}>Applicable Taxes</th>
                    <th style={{ width: '140px' }}>Line Net (₹)</th>
                  </tr>
                </thead>
                <tbody>
                  {lineComputations.map((item) => (
                    <tr key={item.requirementId}>
                      <td className="bn-text-center">
                        <span className="bn-badge bn-badge-line">#{item.lineNo}</span>
                      </td>
                      <td>
                        <div className="bn-item-name-cell">
                          <strong className="bn-item-title">{item.itemName}</strong>
                          {item.spec && <span className="bn-item-spec">{item.spec}</span>}
                        </div>
                      </td>
                      <td className="bn-text-right bn-font-mono">{item.quantity}</td>
                      <td>
                        <span className="bn-unit-chip">{item.unit}</span>
                      </td>
                      <td>
                        <input
                          type="number"
                          step="any"
                          min="0.01"
                          placeholder="0.00"
                          className="bn-input bn-input-sm bn-item-rate-input"
                          value={item.rate}
                          onChange={(e) => handleRateChange(item.requirementId, e.target.value)}
                          disabled={submitting}
                        />
                      </td>
                      <td className="bn-money-cell">
                        {formatCurrency(item.baseAmount)}
                      </td>
                      <td>
                        <div className="bn-tax-selector-cell">
                          <div className="bn-tax-pills-list">
                            {item.appliedTaxes.map((tax) => {
                              const isDeductive = tax.taxNatureId === TaxNatureEnum.Deductive;
                              return (
                                <span
                                  key={tax.tempKey}
                                  className={`bn-tax-pill ${isDeductive ? 'bn-tax-pill-deductive' : ''} ${
                                    tax.isCustom ? 'bn-tax-pill-custom' : ''
                                  }`}
                                >
                                  <span>
                                    {tax.code || tax.name} ({getTaxDisplayRate(tax)})
                                    {tax.isCustom && <span className="bn-pill-subtag">custom</span>}
                                  </span>
                                  <button
                                    type="button"
                                    className="bn-tax-pill-remove"
                                    onClick={() => handleRemoveTax(item.requirementId, tax.tempKey)}
                                    title="Remove tax"
                                  >
                                    ×
                                  </button>
                                </span>
                              );
                            })}
                          </div>

                          <div className="bn-tax-input-group">
                            <select
                              className="bn-select bn-input-xs bn-tax-dropdown"
                              value=""
                              onChange={(e) => {
                                const val = e.target.value;
                                if (!val) return;
                                if (val === '__custom__') {
                                  openCustomTaxModal(item.requirementId);
                                } else {
                                  handleAddMasterTax(item.requirementId, Number(val));
                                }
                              }}
                              disabled={submitting}
                            >
                              <option value="">+ Add Tax from Catalog...</option>
                              {masterTaxes
                                .filter((t) => !item.appliedTaxes.some((at) => at.id === t.id))
                                .map((t) => (
                                  <option key={t.id} value={t.id}>
                                    {t.name} ({t.code ? `${t.code} - ` : ''}{getTaxDisplayRate(t)})
                                  </option>
                                ))}
                              <option value="__custom__">➕ Type / Custom Tax...</option>
                            </select>

                            <button
                              type="button"
                              className="bn-btn-custom-tax"
                              onClick={() => openCustomTaxModal(item.requirementId)}
                              title="Type custom tax name, nature, and charge type"
                            >
                              + Custom
                            </button>
                          </div>

                          {item.lineTaxAmount !== 0 && (
                            <span className="bn-tax-breakdown-subtext">
                              Net Tax: {formatCurrency(item.lineTaxAmount)}
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="bn-money-cell bn-money-cell-net">
                        {formatCurrency(item.lineNetAmount)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Real-time Bid Totals Summary Cards */}
          <div className="bn-bidding-totals-grid">
            <div className="bn-total-card">
              <span className="bn-total-label">AGGREGATE BASIC AMOUNT</span>
              <span className="bn-total-val">{formatCurrency(totalBasic)}</span>
              <span className="bn-text-xs bn-text-muted">Sum of Base Amounts (Rate × Qty)</span>
            </div>

            <div className="bn-total-card">
              <span className="bn-total-label">APPLICABLE TAXES (+/-)</span>
              <span className={`bn-total-val ${totalTax < 0 ? 'bn-text-danger' : totalTax > 0 ? 'bn-text-cyan' : ''}`}>
                {totalTax > 0 ? `+${formatCurrency(totalTax)}` : formatCurrency(totalTax)}
              </span>
              <span className="bn-text-xs bn-text-muted">Net Additive & Deductive Line Taxes</span>
            </div>

            <div className="bn-total-card bn-total-card-highlight">
              <span className="bn-total-label">TOTAL AUTHORITATIVE NET BID</span>
              <span className="bn-total-val bn-text-glow">{formatCurrency(totalNet)}</span>
              <span className="bn-text-xs bn-text-muted">Final binding offer submitted to auction ledger</span>
            </div>
          </div>

          {validationError && <p className="bn-error-msg bn-mb-2">{validationError}</p>}

          {/* Primary CTA Button */}
          <Button
            type="submit"
            variant="primary"
            size="lg"
            className="bn-w-full bn-bid-submit-btn"
            loading={submitting}
            icon={<Zap size={20} />}
          >
            {submitting ? 'VALIDATING & SUBMITTING ITEM-LEVEL BID...' : `PLACE REVISED BID (${formatCurrency(totalNet)}) →`}
          </Button>

          <div className="bn-security-footer">
            <ShieldCheck size={13} />
            <span>Server-side verification re-computes item rates and taxes in real time to ensure strict compliance.</span>
          </div>
        </form>
      )}

      {/* Custom Tax Modal */}
      {customTaxModalReqId !== null && (
        <Modal
          isOpen={customTaxModalReqId !== null}
          onClose={() => setCustomTaxModalReqId(null)}
          title={`Add Custom Tax — Line #${targetReqLine?.lineNo || ''}`}
          subtitle={`Assign an ad-hoc tax rule for "${targetReqLine?.itemName || 'Item'}"`}
          maxWidth="md"
          footer={
            <div className="bn-flex-end gap-2">
              <Button
                variant="outline"
                type="button"
                onClick={() => setCustomTaxModalReqId(null)}
              >
                Cancel
              </Button>
              <Button
                variant="primary"
                type="button"
                onClick={() => handleApplyCustomTax()}
              >
                Apply Tax to Line
              </Button>
            </div>
          }
        >
          <div className="bn-form-stack">
            {customTaxError && (
              <div className="bn-auth-error">
                <AlertTriangle size={16} /> {customTaxError}
              </div>
            )}

            <div className="bn-grid-2">
              <Input
                label="Tax Name"
                placeholder="e.g. Municipal Octroi, Toll Cess"
                value={customTaxName}
                onChange={(e) => {
                  setCustomTaxName(e.target.value);
                  if (!customTaxCode || customTaxCode === customTaxName.replace(/[^a-zA-Z0-9]/g, '_').toUpperCase()) {
                    setCustomTaxCode(e.target.value.replace(/[^a-zA-Z0-9]/g, '_').toUpperCase());
                  }
                }}
                required
              />
              <Input
                label="Tax Code"
                placeholder="e.g. OCTROI_2, TOLL_CESS"
                value={customTaxCode}
                onChange={(e) => setCustomTaxCode(e.target.value.toUpperCase())}
                required
              />
            </div>

            <div className="bn-grid-2">
              <Select
                label="Tax Nature"
                value={String(customTaxNatureId)}
                onChange={(e) => setCustomTaxNatureId(Number(e.target.value))}
                options={
                  taxNatures.length > 0
                    ? taxNatures.map((n) => ({ value: String(n.id), label: n.name || `Nature #${n.id}` }))
                    : [
                        { value: String(TaxNatureEnum.Additive), label: 'Additive (+ Added to Basic)' },
                        { value: String(TaxNatureEnum.Deductive), label: 'Deductive (- Withheld/Deducted)' },
                      ]
                }
              />

              <Select
                label="Charge Type"
                value={String(customTaxChargeTypeId)}
                onChange={(e) => setCustomTaxChargeTypeId(Number(e.target.value))}
                options={
                  chargeTypes.length > 0
                    ? chargeTypes.map((c) => ({ value: String(c.id), label: c.name || `Type #${c.id}` }))
                    : [
                        { value: String(ChargeTypeEnum.Percentage), label: 'Percentage (%)' },
                        { value: String(ChargeTypeEnum.PerUnit), label: 'Per Unit (₹/unit)' },
                        { value: String(ChargeTypeEnum.Fixed), label: 'Fixed (₹ flat)' },
                      ]
                }
              />
            </div>

            <Input
              label={
                customTaxChargeTypeId === ChargeTypeEnum.PerUnit
                  ? 'Tax Amount (₹ per unit)'
                  : customTaxChargeTypeId === ChargeTypeEnum.Fixed
                  ? 'Fixed Tax Amount (₹ flat)'
                  : 'Tax Rate (%)'
              }
              type="number"
              step="any"
              min="0"
              placeholder={
                customTaxChargeTypeId === ChargeTypeEnum.PerUnit
                  ? '5.00'
                  : customTaxChargeTypeId === ChargeTypeEnum.Fixed
                  ? '100.00'
                  : '18'
              }
              value={customTaxValue}
              onChange={(e) => setCustomTaxValue(e.target.value)}
              required
            />

            <div className="bn-custom-tax-preview-box">
              <div className="bn-custom-tax-preview-row">
                <span className="bn-text-muted">Target Requirement:</span>
                <strong>
                  Line #{targetReqLine?.lineNo}: {targetReqLine?.itemName} ({targetReqLine?.quantity} {targetReqLine?.unit})
                </strong>
              </div>
              <div className="bn-custom-tax-preview-row">
                <span className="bn-text-muted">Current Base Amount:</span>
                <span>{formatCurrency(targetBase)}</span>
              </div>
              <div className="bn-custom-tax-preview-row">
                <span className="bn-text-muted">
                  Computed Tax Impact ({isPreviewDeductive ? 'Deductive -' : 'Additive +'}):
                </span>
                <strong style={{ color: isPreviewDeductive ? '#f87171' : '#34d399' }}>
                  {isPreviewDeductive ? '-' : '+'}
                  {formatCurrency(previewTaxAmt)}
                </strong>
              </div>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
};
