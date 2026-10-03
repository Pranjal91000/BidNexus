import React, { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Input, Select, Textarea } from '../ui/Input';
import { Button } from '../ui/Button';
import type { Auction, AuctionCreateRequest, AuctionRequirementSaveRequest, Item, Unit } from '../../types';
import { Plus, Trash2, Layers, AlertCircle } from 'lucide-react';
import { api } from '../../services/api';

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
  const [statusId, setStatusId] = useState(1);

  // Requirements list
  const [requirements, setRequirements] = useState<AuctionRequirementSaveRequest[]>([
    { lineNo: 1, itemId: 1, technicalSpecification: 'Grade A Procurement Spec', quantity: 100, unitId: 1 },
  ]);

  // Master options
  const [items, setItems] = useState<Item[]>([]);
  const [units, setUnits] = useState<Unit[]>([]);

  // Validation & Submit state
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState('');

  useEffect(() => {
    if (isOpen) {
      loadMasters();
      if (initialData) {
        setAuctionName(initialData.auctionName || '');
        setAbout(initialData.about || '');
        setDocNoYearly(initialData.docNoYearly || '');
        setDocDate(initialData.docDate ? initialData.docDate.slice(0, 10) : new Date().toISOString().slice(0, 10));
        setIsForwardAuction(Boolean(initialData.isForwardAuction));
        setAuctionStartTime(
          initialData.auctionStartTime ? new Date(initialData.auctionStartTime).toISOString().slice(0, 16) : ''
        );
        setAuctionEndTime(
          initialData.auctionEndTime ? new Date(initialData.auctionEndTime).toISOString().slice(0, 16) : ''
        );
        setOpenToAll(Boolean(initialData.openToAll));
        setIsBidPriceHidden(Boolean(initialData.isBidPriceHidden));
        setOrganizationId(initialData.organizationId || userOrgId || 1);
        setStatusId(initialData.statusId || 1);

        if (initialData.auctionRequirements && initialData.auctionRequirements.length > 0) {
          setRequirements(
            initialData.auctionRequirements.map((r, i) => ({
              lineNo: r.lineNo || i + 1,
              itemId: r.itemId || r.item?.id || 1,
              technicalSpecification: r.technicalSpecification || '',
              quantity: r.quantity || 1,
              unitId: r.unitId || r.unit?.id || 1,
              documentAttachmentId: r.documentAttachmentId || null,
            }))
          );
        }
      } else {
        // Reset form defaults for create
        const now = new Date();
        const tomorrow = new Date(now.valueOf() + 86400000);
        const docNo = `AUC/${now.getFullYear()}/${Math.floor(100 + Math.random() * 900)}`;

        setAuctionName('');
        setAbout('');
        setDocNoYearly(docNo);
        setDocDate(now.toISOString().slice(0, 10));
        setIsForwardAuction(false);
        setAuctionStartTime(now.toISOString().slice(0, 16));
        setAuctionEndTime(tomorrow.toISOString().slice(0, 16));
        setOpenToAll(true);
        setIsBidPriceHidden(false);
        setOrganizationId(userOrgId || 1);
        setStatusId(1);
        setRequirements([
          { lineNo: 1, itemId: 1, technicalSpecification: 'Grade A Procurement Spec', quantity: 100, unitId: 1 },
        ]);
      }
      setErrors({});
      setFormError('');
    }
  }, [isOpen, initialData, token, userOrgId]);

  const loadMasters = async () => {
    try {
      const [itemList, unitList] = await Promise.all([api.getItems(token), api.getUnits(token)]);
      setItems(itemList);
      setUnits(unitList);
    } catch {
      // Fallbacks if master endpoint empty
    }
  };

  const addRequirement = () => {
    const nextLineNo = requirements.length + 1;
    const defaultItemId = items[0]?.id || 1;
    const defaultUnitId = units[0]?.id || 1;
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

  const defaultItemOptions = items.length
    ? items.map((i) => ({ value: i.id, label: i.itemName || i.name || `Item #${i.id}` }))
    : [
        { value: 1, label: 'Structural Steel Plate Grade A36' },
        { value: 2, label: 'Industrial Electric Motor 50HP' },
        { value: 3, label: 'Copper Cathode Grade A' },
        { value: 4, label: 'Diesel Fuel EN590 10PPM' },
      ];

  const defaultUnitOptions = units.length
    ? units.map((u) => ({ value: u.id, label: u.unitName || u.name || u.alias || `Unit #${u.id}` }))
    : [
        { value: 1, label: 'Metric Ton (MT)' },
        { value: 2, label: 'Pieces (PCS)' },
        { value: 3, label: 'Kilo Liters (KL)' },
        { value: 4, label: 'Meters (M)' },
      ];

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
          <Button variant="primary" onClick={handleSubmit} loading={submitting}>
            {isEdit ? 'Update Auction' : 'Create & Schedule Auction'}
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="bn-form-stack">
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

        <div className="bn-grid-2">
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
                  <th style={{ width: '50px' }}>Line</th>
                  <th style={{ width: '220px' }}>Item</th>
                  <th>Technical Specification</th>
                  <th style={{ width: '110px' }}>Quantity</th>
                  <th style={{ width: '160px' }}>Unit</th>
                  <th style={{ width: '50px' }}></th>
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
                        {defaultItemOptions.map((opt) => (
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
                        {defaultUnitOptions.map((opt) => (
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
