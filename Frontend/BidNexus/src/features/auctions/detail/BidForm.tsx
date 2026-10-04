import { useEffect, useMemo, useState } from 'react';
import { X } from 'lucide-react';
import type { AppliedTaxItem, Auction, Bid, BidDetailSaveRequest, TaxMaster } from '../../../types';
import { ChargeTypeEnum, TaxNatureEnum } from '../../../types';
import { api } from '../../../services/api';
import { errorMessage, useApp } from '../../../lib/appContext';
import { isBetter, itemNameOf, unitNameOf } from '../../../lib/auction';
import { inr, qty } from '../../../lib/format';
import { buildBidTaxDetails, getTaxDisplayRate } from '../../../utils/taxUtils';
import { Button } from '../../../components/ui/Button';
import { Modal } from '../../../components/ui/Modal';
import { SelectField, TextField } from '../../../components/ui/Field';
import { Callout } from '../../../components/ui/States';

interface Line {
  requirementId: number;
  itemName: string;
  spec: string;
  quantity: number;
  unit: string;
  rate: string;
  taxes: AppliedTaxItem[];
}

interface Props {
  auction: Auction;
  /** The caller's current bid, if any (with line details). */
  currentBid: Bid | null;
  /** Other bidders' current amounts; empty when the auction hides prices. */
  otherAmounts: number[];
  disabled: boolean;
  onSubmitted: () => void;
}

const round2 = (n: number) => Math.round(n * 100) / 100;

/** "GST 18%" — avoids repeating the rate when the name already contains it. */
const taxLabel = (t: AppliedTaxItem) => {
  const rate = getTaxDisplayRate(t);
  const name = t.name || t.code;
  return name.includes(rate) ? name : `${name} ${rate}`;
};

export function BidForm({ auction, currentBid, otherAmounts, disabled, onSubmitted }: Props) {
  const { token, claims, toast } = useApp();
  const forward = !!auction.isForwardAuction;
  const [catalog, setCatalog] = useState<TaxMaster[]>([]);
  const [lines, setLines] = useState<Line[]>([]);
  const [confirming, setConfirming] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [customFor, setCustomFor] = useState<number | null>(null);

  useEffect(() => {
    api.getTaxMasters(token).then((t) => setCatalog(t || [])).catch(() => setCatalog([]));
  }, [token]);

  // Start from the vendor's current bid so a revision only needs the changes.
  // (Re-seeding local form state when the server's bid changes is intentional.)
  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    const details = currentBid?.bidDetails ?? [];
    const reqs = [...(auction.auctionRequirements ?? [])].sort((a, b) => (a.lineNo ?? 0) - (b.lineNo ?? 0));
    setLines(
      reqs.map((r) => {
        const d = details.find((x: { auctionRequirementId: number }) => x.auctionRequirementId === r.id);
        return {
          requirementId: r.id,
          itemName: itemNameOf(r),
          spec: r.technicalSpecification ?? '',
          quantity: Number(r.quantity) || 1,
          unit: unitNameOf(r),
          rate: d?.rate ? String(d.rate) : '',
          taxes: (d?.taxes ?? []).map((t: Record<string, unknown>, i: number) => ({
            id: (t.taxId as number) ?? null,
            tempKey: t.taxId ? `m-${t.taxId}` : `c-${i}-${t.taxCode}`,
            name: String(t.taxName ?? 'Tax'),
            code: String(t.taxCode ?? ''),
            taxNatureId: Number(t.taxNatureId) || TaxNatureEnum.Additive,
            chargeTypeId: Number(t.chargeTypeId) || ChargeTypeEnum.Percentage,
            taxValue: Number(t.taxValue) || 0,
            isCustom: !t.taxId,
          })),
        };
      }),
    );
    setConfirming(false);
  }, [auction.auctionRequirements, currentBid]);

  const computed = useMemo(
    () =>
      lines.map((l) => {
        const rate = Math.max(0, parseFloat(l.rate) || 0);
        const base = round2(rate * l.quantity);
        const { taxes, totalTaxAmount } = buildBidTaxDetails(l.taxes, base, l.quantity);
        return { ...l, rateNum: rate, base, taxDetails: taxes, tax: totalTaxAmount, net: round2(base + totalTaxAmount) };
      }),
    [lines],
  );
  const basic = round2(computed.reduce((s, l) => s + l.base, 0));
  const taxTotal = round2(computed.reduce((s, l) => s + l.tax, 0));
  const total = round2(computed.reduce((s, l) => s + l.net, 0));

  const allRated = computed.length > 0 && computed.every((l) => l.rateNum > 0);
  const improves = !currentBid || isBetter(total, currentBid.netAmount, forward);
  const bestOther = otherAmounts.length ? (forward ? Math.max(...otherAmounts) : Math.min(...otherAmounts)) : null;
  const projectedRank = otherAmounts.filter((o) => isBetter(o, total, forward)).length + 1;

  let hint: { text: string; tone?: 'warn' | 'good' };
  if (!allRated) hint = { text: 'Enter a rate for every item.' };
  else if (!improves) hint = { text: `Your new total must be ${forward ? 'higher' : 'lower'} than your current bid of ${inr(currentBid!.netAmount)}.`, tone: 'warn' };
  else if (bestOther == null) hint = { text: currentBid ? `This changes your bid by ${inr(Math.abs(currentBid.netAmount - total))}.` : 'Your first bid in this auction.' };
  else if (projectedRank === 1) hint = { text: 'This bid would put you in the lead.', tone: 'good' };
  else hint = { text: `This would place you L${projectedRank}. ${forward ? 'Go above' : 'Go below'} ${inr(bestOther)} to lead.` };

  const setLine = (id: number, patch: Partial<Line>) => {
    setConfirming(false);
    setError('');
    setLines((prev) => prev.map((l) => (l.requirementId === id ? { ...l, ...patch } : l)));
  };

  const addCatalogTax = (id: number, taxId: number) => {
    const m = catalog.find((t) => t.id === taxId);
    const line = lines.find((l) => l.requirementId === id);
    if (!m || !line || line.taxes.some((t) => t.id === m.id)) return;
    setLine(id, {
      taxes: [...line.taxes, { id: m.id, tempKey: `m-${m.id}`, name: m.name, code: m.code, taxNatureId: m.taxNatureId, chargeTypeId: m.chargeTypeId, taxValue: Number(m.taxValue) || 0 }],
    });
  };

  const adjustAll = (percent: number) => {
    setConfirming(false);
    setLines((prev) =>
      prev.map((l) => {
        const r = parseFloat(l.rate) || 0;
        if (r <= 0) return l;
        const next = forward ? r * (1 + percent / 100) : r * (1 - percent / 100);
        return { ...l, rate: String(round2(Math.max(0.01, next))) };
      }),
    );
  };

  const submit = async () => {
    setSubmitting(true);
    setError('');
    try {
      const bidDetails: BidDetailSaveRequest[] = computed.map((l) => ({
        auctionRequirementId: l.requirementId,
        rate: l.rateNum,
        baseAmount: l.base,
        netAmount: l.net,
        taxes: l.taxDetails,
      }));
      await api.submitBid(token, {
        auctionId: auction.id,
        vendorId: claims.userId,
        basicAmount: basic,
        taxAmount: taxTotal,
        discountAmount: 0,
        netAmount: total,
        mainBidId: currentBid ? currentBid.id : null,
        bidRevisionNo: currentBid ? currentBid.bidRevisionNo + 1 : 1,
        bidDetails,
      });
      toast(`Bid of ${inr(total)} accepted.`, 'success');
      setConfirming(false);
      onSubmitted();
    } catch (err) {
      setError(errorMessage(err, 'The bid was not accepted.'));
      setConfirming(false);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <section className="card">
      <div className="row row--between" style={{ padding: '20px 24px 4px' }}>
        <h2>{currentBid ? 'Revise your bid' : 'Place your bid'}</h2>
        <div className="row" style={{ gap: 6 }}>
          <span className="small muted">{forward ? 'Raise' : 'Lower'} all rates</span>
          {[0.5, 1, 2].map((p) => (
            <Button key={p} size="sm" variant="ghost" onClick={() => adjustAll(p)} disabled={disabled || !allRated}>
              {forward ? '+' : '−'}{p}%
            </Button>
          ))}
        </div>
      </div>

      <div style={{ overflowX: 'auto', padding: '0 24px' }}>
        <table className="table table--compact bid-table">
          <thead>
            <tr>
              <th>Item</th>
              <th className="num">Qty</th>
              <th className="num" style={{ width: 170 }}>Rate per unit</th>
              <th>Taxes</th>
              <th className="num">Amount</th>
            </tr>
          </thead>
          <tbody>
            {computed.map((l) => {
              const available = catalog.filter((t) => !l.taxes.some((x) => x.id === t.id));
              return (
                <tr key={l.requirementId}>
                  <td style={{ paddingTop: 14, paddingBottom: 14 }}>
                    <label htmlFor={`rate-${l.requirementId}`} className="strong">{l.itemName}</label>
                    {l.spec && <span className="cell-sub">{l.spec}</span>}
                  </td>
                  <td className="num muted" data-label="Quantity">{qty(l.quantity)} {l.unit}</td>
                  <td data-label="Rate per unit">
                    <div className="input-affix">
                      <span className="input-affix__prefix">₹</span>
                      <input
                        id={`rate-${l.requirementId}`}
                        inputMode="decimal"
                        value={l.rate}
                        disabled={disabled}
                        onChange={(e) => setLine(l.requirementId, { rate: e.target.value.replace(/[^0-9.]/g, '') })}
                      />
                    </div>
                  </td>
                  <td data-label="Taxes">
                    <div className="row" style={{ gap: 6 }}>
                      {l.taxes.map((t) => (
                        <span key={t.tempKey} className={`tax-chip${t.taxNatureId === TaxNatureEnum.Deductive ? ' tax-chip--minus' : ''}`}>
                          {taxLabel(t)}
                          <button
                            type="button"
                            aria-label={`Remove ${t.name}`}
                            disabled={disabled}
                            onClick={() => setLine(l.requirementId, { taxes: l.taxes.filter((x) => x.tempKey !== t.tempKey) })}
                          >
                            <X size={14} />
                          </button>
                        </span>
                      ))}
                      <select
                        className="select"
                        style={{ height: 30, width: 'auto', fontSize: 'var(--text-sm)', paddingRight: 28 }}
                        value=""
                        disabled={disabled}
                        aria-label={`Add tax to ${l.itemName}`}
                        onChange={(e) => {
                          const v = e.target.value;
                          if (v === 'custom') setCustomFor(l.requirementId);
                          else if (v) addCatalogTax(l.requirementId, Number(v));
                        }}
                      >
                        <option value="">Add tax</option>
                        {available.map((t) => (
                          <option key={t.id} value={t.id}>{t.name} ({getTaxDisplayRate(t)})</option>
                        ))}
                        <option value="custom">Other tax…</option>
                      </select>
                    </div>
                  </td>
                  <td className="num strong" data-label="Amount">{inr(l.net)}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      <div style={{ padding: '16px 24px 0' }}>
        <dl className="totals small" style={{ margin: 0 }}>
          <dt>Basic</dt>
          <dd className="num">{inr(basic)}</dd>
          <dt>Taxes</dt>
          <dd className="num">{inr(taxTotal)}</dd>
          <dt className="totals__grand">New total</dt>
          <dd className="num totals__grand">{inr(total)}</dd>
        </dl>
      </div>

      {error && (
        <div style={{ padding: '16px 24px 0' }}>
          <Callout tone="error">{error}</Callout>
        </div>
      )}

      <div className="row row--between card__divider" style={{ marginTop: 20, padding: '16px 24px' }}>
        <span className={`small grow ${hint.tone === 'warn' ? 'warn' : hint.tone === 'good' ? 'accent' : 'muted'}`} aria-live="polite" style={{ flexBasis: 280 }}>
          {disabled ? 'Bidding is not open.' : hint.text}
        </span>
        {!confirming ? (
          <Button variant="primary" size="lg" disabled={disabled || !allRated || !improves} onClick={() => setConfirming(true)}>
            Place bid · {inr(total)}
          </Button>
        ) : (
          <div className="row">
            <span className="strong">Submit {inr(total)}? Bids can't be withdrawn.</span>
            <Button onClick={() => setConfirming(false)}>Cancel</Button>
            <Button variant="primary" size="lg" loading={submitting} onClick={submit}>
              Confirm bid
            </Button>
          </div>
        )}
      </div>

      {customFor != null && (
        <CustomTaxModal
          onClose={() => setCustomFor(null)}
          onAdd={(tax) => {
            const line = lines.find((l) => l.requirementId === customFor);
            if (line) setLine(customFor, { taxes: [...line.taxes, tax] });
            setCustomFor(null);
          }}
        />
      )}
    </section>
  );
}

function CustomTaxModal({ onClose, onAdd }: { onClose: () => void; onAdd: (t: AppliedTaxItem) => void }) {
  const [name, setName] = useState('');
  const [code, setCode] = useState('');
  const [nature, setNature] = useState<number>(TaxNatureEnum.Additive);
  const [charge, setCharge] = useState<number>(ChargeTypeEnum.Percentage);
  const [value, setValue] = useState('');
  const [error, setError] = useState('');

  const add = () => {
    const v = parseFloat(value);
    if (!name.trim()) return setError('Enter a name.');
    if (isNaN(v) || v < 0) return setError('Enter a value of 0 or more.');
    onAdd({
      id: null,
      tempKey: `c-${Date.now()}`,
      name: name.trim(),
      code: (code.trim() || name.trim().replace(/[^a-z0-9]/gi, '_')).toUpperCase(),
      taxNatureId: nature,
      chargeTypeId: charge,
      taxValue: v,
      isCustom: true,
    });
  };

  return (
    <Modal
      open
      title="Other tax or charge"
      onClose={onClose}
      footer={
        <>
          <Button variant="ghost" onClick={onClose}>Cancel</Button>
          <Button variant="primary" onClick={add}>Add</Button>
        </>
      }
    >
      {error && <Callout tone="error">{error}</Callout>}
      <div className="form-grid">
        <TextField label="Name" value={name} onChange={(e) => setName(e.target.value)} />
        <TextField label="Code (optional)" value={code} onChange={(e) => setCode(e.target.value)} />
        <SelectField
          label="Adds or deducts"
          value={nature}
          onChange={(e) => setNature(Number(e.target.value))}
          options={[
            { value: TaxNatureEnum.Additive, label: 'Adds to amount' },
            { value: TaxNatureEnum.Deductive, label: 'Deducts from amount' },
          ]}
        />
        <SelectField
          label="Charged as"
          value={charge}
          onChange={(e) => setCharge(Number(e.target.value))}
          options={[
            { value: ChargeTypeEnum.Percentage, label: 'Percent of amount' },
            { value: ChargeTypeEnum.Fixed, label: 'Fixed amount' },
            { value: ChargeTypeEnum.PerUnit, label: 'Per unit' },
          ]}
        />
        <TextField label={charge === ChargeTypeEnum.Percentage ? 'Rate (%)' : 'Amount (₹)'} inputMode="decimal" value={value} onChange={(e) => setValue(e.target.value)} />
      </div>
    </Modal>
  );
}
