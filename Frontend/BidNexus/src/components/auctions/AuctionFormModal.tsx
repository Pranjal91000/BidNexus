import React, { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Input, Select, Textarea } from '../ui/Input';
import { Button } from '../ui/Button';
import type { Auction, AuctionCreateRequest, AuctionRequirementSaveRequest, Item, Unit } from '../../types';
import { Plus, Trash2, Layers, AlertCircle, RefreshCw } from 'lucide-react';
import { api } from '../../services/api';

const toLocalDateInput = (val?: string | null): string => {
  if (!val) return new Date().toISOString().slice(0, 10);
  if (val.length === 10 && val.includes('-')) return val;
  const d = new Date(val);
  if (isNaN(d.getTime())) return new Date().toISOString().slice(0, 10);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
};

const toLocalDatetimeInput = (val?: string | null): string => {
  if (!val) return '';
  const d = new Date(val);
  if (isNaN(d.getTime())) return '';
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
};

interface AuctionFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: AuctionCreateRequest) => Promise<void>;
  initialData?: Auction | null;
  token: string;
  userOrgId?: number;
}

export const AuctionFormModal: React.FC<AuctionFormModalProps> = ({
  isOpen,
  onClose,
  onSubmit,
  initialData,
  token,
  userOrgId = 1,
}) => {
  const isEdit = Boolean(initialData);

  // Form states
  const [auctionName, setAuctionName] = useState('');
  const [about, setAbout] = useState('');
  const [docNoYearly, setDocNoYearly] = useState('');
  const [docDate, setDocDate] = useState(() => new Date().toISOString().split('T')[0]);
  const [isForwardAuction, setIsForwardAuction] = useState(false);
  const [auctionStartTime, setAuctionStartTime] = useState(() => new Date().toISOString().slice(0, 16));
  const [auctionEndTime, setAuctionEndTime] = useState(() => {
    const d = new Date();
    d.setHours(d.getHours() + 24);
    return d.toISOString().slice(0, 16);
  });
  const [openToAll, setOpenToAll] = useState(true);
  const [isBidPriceHidden, setIsBidPriceHidden] = useState(false);
  const [organizationId, setOrganizationId] = useState(userOrgId || 1);
  const [statusId, setStatusId] = useState(2); // 1: Draft, 2: Authorized

  // Requirements list
  const [requirements, setRequirements] = useState<AuctionRequirementSaveRequest[]>([
    { lineNo: 1, itemId: 1, technicalSpecification: 'Grade A Procurement Spec', quantity: 100, unitId: 1 },
  ]);

  // Master options
  const [items, setItems] = useState<Item[]>([]);
  const [units, setUnits] = useState<Unit[]>([]);
  const [loadingMasters, setLoadingMasters] = useState(false);
  const [masterNotice, setMasterNotice] = useState('');

  // Validation & Submit state
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState('');
  const [loadingDetails, setLoadingDetails] = useState(false);

  useEffect(() => {
    if (!isOpen) return;

    loadMasters();

    const populateAuctionFields = (data: Auction) => {
      setAuctionName(data.auctionName || '');
      setAbout(data.about || '');
      setDocNoYearly(data.docNoYearly || '');
      setDocDate(toLocalDateInput(data.docDate));
      setIsForwardAuction(Boolean(data.isForwardAuction));
      setAuctionStartTime(toLocalDatetimeInput(data.auctionStartTime));
      setAuctionEndTime(toLocalDatetimeInput(data.auctionEndTime));
      setOpenToAll(Boolean(data.openToAll));
      setIsBidPriceHidden(Boolean(data.isBidPriceHidden));
      setOrganizationId(data.organizationId || data.organization?.id || userOrgId || 1);
      setStatusId(data.statusId === 1 ? 1 : 2);

      if (data.auctionRequirements && data.auctionRequirements.length > 0) {
        setRequirements(
          data.auctionRequirements.map((r, i) => ({
            lineNo: r.lineNo || i + 1,
            itemId: r.itemId || r.item?.id || 1,
            technicalSpecification: r.technicalSpecification || '',
            quantity: Number(r.quantity) || 1,
            unitId: r.unitId || r.unit?.id || 1,
            documentAttachmentId: r.documentAttachmentId || null,
          }))
        );
      }
    };

    if (initialData?.id) {
      // Pre-fill immediately with initialData so user doesn't see blank inputs
      populateAuctionFields(initialData);

      // Extract complete authoritative details directly from GetById
      setLoadingDetails(true);
      api
        .getAuctionById(token, initialData.id)
        .then((fullAuction) => {
          if (fullAuction) {
            populateAuctionFields(fullAuction);
          }
        })
        .catch((err) => {
          console.error('Failed to extract full auction details via GetById:', err);
        })
        .finally(() => {
          setLoadingDetails(false);
        });
    } else if (initialData) {
      populateAuctionFields(initialData);
    } else {
      // Reset form defaults for create
      const now = new Date();
      const tomorrow = new Date(now.valueOf() + 86400000);
      const docNo = `AUC/${now.getFullYear()}/${Math.floor(100 + Math.random() * 900)}`;

      setAuctionName('');
      setAbout('');
      setDocNoYearly(docNo);
      setDocDate(toLocalDateInput(now.toISOString()));
      setIsForwardAuction(false);
      setAuctionStartTime(toLocalDatetimeInput(now.toISOString()));
      setAuctionEndTime(toLocalDatetimeInput(tomorrow.toISOString()));
      setOpenToAll(true);
      setIsBidPriceHidden(false);
      setOrganizationId(userOrgId || 1);
      setStatusId(2); // Default to Authorized (2) or Draft (1)
      setRequirements([]);
    }

    setErrors({});
    setFormError('');
  }, [isOpen, initialData?.id, token, userOrgId]);

  const loadMasters = async () => {
    setLoadingMasters(true);
    try {
      const [itemList, unitList] = await Promise.all([
        api.getItems(token),
        api.getUnits(token),
      ]);
      setItems(itemList || []);
      setUnits(unitList || []);

      if (!itemList || itemList.length === 0 || !unitList || unitList.length === 0) {
        setMasterNotice(
          'Notice: Master Items or Measurement Units are not yet configured for your tenant. Please ensure items and units are registered in your Master Catalog.'
        );
      } else {
        setMasterNotice('');
      }

      if (!initialData) {
        if (itemList && itemList.length > 0 && unitList && unitList.length > 0) {
          setRequirements([
            {
              lineNo: 1,
              itemId: itemList[0].id,
              technicalSpecification: 'Standard Specification',
              quantity: 100,
              unitId: unitList[0].id,
            },
          ]);
        }
      }
    } catch (err: any) {
      setMasterNotice('Could not load master catalog: ' + (err.message || 'Unknown error'));
    } finally {
      setLoadingMasters(false);
    }
  };

  const addRequirement = () => {
    if (items.length === 0 || units.length === 0) {
      setFormError('Cannot add requirements without active items and units defined in master data.');
      return;
    }
    const nextLineNo = requirements.length + 1;
    const defaultItemId = items[0].id;
    const defaultUnitId = units[0].id;
    setRequirements([
      ...requirements,
      { lineNo: nextLineNo, itemId: defaultItemId, technicalSpecification: '', quantity: 1, unitId: defaultUnitId },
    ]);
  };

  const removeRequirement = (index: number) => {
    if (requirements.length <= 1) return;
    const updated = requirements
      .filter((_, i) => i !== index)
      .map((req, i) => ({ ...req, lineNo: i + 1 }));
    setRequirements(updated);
  };

  const updateRequirement = (index: number, field: keyof AuctionRequirementSaveRequest, value: any) => {
    const updated = [...requirements];
    updated[index] = { ...updated[index], [field]: value };
    setRequirements(updated);
  };

  const validate = (): boolean => {
    const errs: Record<string, string> = {};
    if (!auctionName.trim()) errs.auctionName = 'Auction name is required.';
    if (!docNoYearly.trim()) errs.docNoYearly = 'Document Number is required.';
    if (!docDate) errs.docDate = 'Document Date is required.';
    if (!auctionStartTime) errs.auctionStartTime = 'Start time is required.';
    if (!auctionEndTime) errs.auctionEndTime = 'End time is required.';

    if (auctionStartTime && auctionEndTime) {
      if (new Date(auctionEndTime).getTime() <= new Date(auctionStartTime).getTime()) {
        errs.auctionEndTime = 'End time must be later than start time.';
      }
    }

    if (requirements.length === 0) {
      errs.requirements = 'At least one requirement line item is required.';
    } else {
      requirements.forEach((req, i) => {
        if (!req.quantity || req.quantity <= 0) {
          errs[`req_qty_${i}`] = 'Quantity must be > 0';
        }
      });
    }

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    if (!validate()) return;

    setSubmitting(true);
    try {
      const payload: AuctionCreateRequest = {
        auctionName: auctionName.trim(),
        about: about.trim(),
        docNoYearly: docNoYearly.trim(),
        docDate: docDate,
        isForwardAuction,
        auctionStartTime: new Date(auctionStartTime).toISOString(),
        auctionEndTime: new Date(auctionEndTime).toISOString(),
        openToAll,
        isBidPriceHidden,
        organizationId,
        statusId,
        auctionRequirements: requirements.map((r) => ({
          lineNo: r.lineNo,
          itemId: Number(r.itemId) || 1,
          technicalSpecification: r.technicalSpecification || '',
          quantity: Number(r.quantity) || 1,
          unitId: Number(r.unitId) || 1,
          documentAttachmentId: r.documentAttachmentId || null,
        })),
      };

      await onSubmit(payload);
      onClose();
    } catch (err: any) {
      setFormError(err instanceof Error ? err.message : 'Failed to save auction.');
    } finally {
      setSubmitting(false);
    }
  };

  const itemOptions = items.length
    ? items.map((i) => ({ value: i.id, label: i.itemName || i.name || `Item #${i.id}` }))
    : [{ value: 0, label: '-- No Items Found in Tenant Master --' }];

  const unitOptions = units.length
    ? units.map((u) => ({ value: u.id, label: u.unitName || u.name || u.alias || `Unit #${u.id}` }))
    : [{ value: 0, label: '-- No Units Found in Tenant Master --' }];

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEdit ? `Edit Auction #${initialData?.id}` : 'Create Procurement Auction'}
      subtitle="Define auction parameters, bidding rules, and required material/service line items"
      maxWidth="lg"
      footer={
        <div className="bn-flex-end gap-2">
          <Button variant="outline" onClick={onClose} disabled={submitting}>
            Cancel
          </Button>
          <Button variant="primary" onClick={handleSubmit} loading={submitting || loadingMasters || loadingDetails} disabled={items.length === 0 || units.length === 0 || loadingMasters || loadingDetails}>
            {isEdit ? 'Update Auction' : 'Create & Schedule Auction'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="bn-form-stack">
        {loadingDetails && (
          <div className="bn-alert bn-alert-info bn-mb-3" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', padding: '0.6rem 0.8rem', borderRadius: '6px', background: 'rgba(6, 182, 212, 0.12)', border: '1px solid rgba(6, 182, 212, 0.3)', color: '#06b6d4', fontSize: '0.82rem' }}>
            <RefreshCw size={15} className="bn-spin" />
            <span>Extracting complete auction specifications and requirements from server...</span>
          </div>
        )}

        {masterNotice && (
          <div className="bn-auth-error" style={{ background: 'rgba(234, 179, 8, 0.1)', borderColor: 'rgba(234, 179, 8, 0.4)', color: '#eab308' }}>
            <AlertCircle size={16} /> {masterNotice}
          </div>
        )}

        {formError && (
          <div className="bn-auth-error">
            <AlertCircle size={16} /> {formError}
          </div>
        )}

        <div className="bn-grid-2">
          <Input
            label="Auction Title / Name"
            placeholder="e.g. Q4 Steel Plate Procurement Auction"
            value={auctionName}
            onChange={(e) => setAuctionName(e.target.value)}
            error={errors.auctionName}
            required
          />
          <Input
            label="Document Number (Yearly)"
            placeholder="e.g. AUC/2026/042"
            value={docNoYearly}
            onChange={(e) => setDocNoYearly(e.target.value)}
            error={errors.docNoYearly}
            required
          />
        </div>

        <Textarea
          label="About / Description"
          placeholder="Provide context, delivery terms, compliance requirements, or scope summary"
          rows={2}
          value={about}
          onChange={(e) => setAbout(e.target.value)}
        />

        <div className="bn-grid-3">
          <Select
            label="Auction Mechanism"
            value={isForwardAuction ? 'true' : 'false'}
            onChange={(e) => setIsForwardAuction(e.target.value === 'true')}
            options={[
              { value: 'false', label: 'Reverse Auction (Lowest Price Wins)' },
              { value: 'true', label: 'Forward Auction (Highest Bid Wins)' },
            ]}
          />

          <Input
            label="Auction Start Time"
            type="datetime-local"
            value={auctionStartTime}
            onChange={(e) => setAuctionStartTime(e.target.value)}
            error={errors.auctionStartTime}
            required
          />

          <Input
            label="Auction End Time"
            type="datetime-local"
            value={auctionEndTime}
            onChange={(e) => setAuctionEndTime(e.target.value)}
            error={errors.auctionEndTime}
            required
          />
        </div>

        <div className="bn-grid-3">
          <Select
            label="Auction Status"
            value={String(statusId)}
            onChange={(e) => setStatusId(Number(e.target.value))}
            options={[
              { value: '1', label: 'Draft (Working draft, hidden from vendors)' },
              { value: '2', label: 'Authorized (Authorized for procurement / bidding)' },
            ]}
          />

          <Select
            label="Vendor Access Rules"
            value={openToAll ? 'true' : 'false'}
            onChange={(e) => setOpenToAll(e.target.value === 'true')}
            options={[
              { value: 'true', label: 'Open to All Vendors' },
              { value: 'false', label: 'Restricted / Invited Vendors Only' },
            ]}
          />

          <Select
            label="Bid Price Visibility"
            value={isBidPriceHidden ? 'true' : 'false'}
            onChange={(e) => setIsBidPriceHidden(e.target.value === 'true')}
            options={[
              { value: 'false', label: 'Transparent (Bids Visible on Leaderboard)' },
              { value: 'true', label: 'Hidden Price (Rank Only Visible)' },
            ]}
          />
        </div>

        {/* Dynamic Requirements Manager */}
        <div className="bn-requirements-section bn-mt-4">
          <div className="bn-flex-between bn-mb-2">
            <div>
              <h4 className="bn-section-subheading">
                <Layers size={16} /> Auction Line Item Requirements
              </h4>
              <p className="bn-text-muted bn-text-xs">Specify material items, quantities, units, and technical specs.</p>
            </div>
            <Button
              type="button"
              variant="outline"
              size="sm"
              icon={<Plus size={14} />}
              onClick={addRequirement}
            >
              Add Requirement Line
            </Button>
          </div>

          {errors.requirements && <p className="bn-error-msg bn-mb-2">{errors.requirements}</p>}

          <div className="bn-req-table-wrapper">
            <table className="bn-table bn-req-table">
              <thead>
                <tr>
                  <th style={{ width: '45px' }}>Line</th>
                  <th style={{ width: '180px' }}>Item</th>
                  <th>Technical Specification</th>
                  <th style={{ width: '90px' }}>Quantity</th>
                  <th style={{ width: '130px' }}>Unit</th>
                  <th style={{ width: '45px' }}></th>
                </tr>
              </thead>
              <tbody>
                {requirements.map((req, idx) => (
                  <tr key={idx}>
                    <td>
                      <strong>#{req.lineNo}</strong>
                    </td>
                    <td>
                      <select
                        className="bn-select bn-input-sm"
                        value={req.itemId}
                        onChange={(e) => updateRequirement(idx, 'itemId', Number(e.target.value))}
                      >
                        {itemOptions.map((opt) => (
                          <option key={opt.value} value={opt.value}>
                            {opt.label}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td>
                      <input
                        type="text"
                        className="bn-input bn-input-sm"
                        placeholder="e.g. Standard 12mm thickness, ISO certified"
                        value={req.technicalSpecification || ''}
                        onChange={(e) => updateRequirement(idx, 'technicalSpecification', e.target.value)}
                      />
                    </td>
                    <td>
                      <input
                        type="number"
                        min="1"
                        step="any"
                        className="bn-input bn-input-sm"
                        value={req.quantity}
                        onChange={(e) => updateRequirement(idx, 'quantity', Number(e.target.value))}
                      />
                      {errors[`req_qty_${idx}`] && (
                        <small className="bn-error-msg">{errors[`req_qty_${idx}`]}</small>
                      )}
                    </td>
                    <td>
                      <select
                        className="bn-select bn-input-sm"
                        value={req.unitId}
                        onChange={(e) => updateRequirement(idx, 'unitId', Number(e.target.value))}
                      >
                        {unitOptions.map((opt) => (
                          <option key={opt.value} value={opt.value}>
                            {opt.label}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td>
                      <button
                        type="button"
                        className="bn-icon-btn bn-text-danger"
                        onClick={() => removeRequirement(idx)}
                        disabled={requirements.length <= 1}
                        title="Remove requirement"
                      >
                        <Trash2 size={14} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </form>
    </Modal>
  );
};
