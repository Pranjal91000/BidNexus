import React, { useState, useEffect } from 'react';
import {
  Package,
  Ruler,
  Percent,
  Plus,
  Trash2,
  Search,
  AlertCircle,
  CheckCircle2
} from 'lucide-react';
import { Button } from '../ui/Button';
import { Input, Textarea, Select } from '../ui/Input';
import { Modal } from '../ui/Modal';
import { Badge } from '../ui/Badge';
import { LoadingState } from '../ui/LoadingState';
import { api } from '../../services/api';
import type { Item, Unit, TaxMaster, GlobalCategory, ChargeType, TaxNature, Claims } from '../../types';
import { getTaxDisplayRate, isPerUnitCharge, isFixedCharge } from '../../utils/taxUtils';

interface MastersViewProps {
  token: string;
  claims: Claims;
  onShowToast: (msg: string, type?: 'success' | 'error' | 'warning' | 'info') => void;
}

export const MastersView: React.FC<MastersViewProps> = ({ token, claims, onShowToast }) => {
  const isOrg = claims.role === 'Organization';
  const isVendor = claims.role === 'Vendor';
  const [activeTab, setActiveTab] = useState<'items' | 'units' | 'taxes'>(isVendor ? 'taxes' : 'items');
  const [searchQuery, setSearchQuery] = useState('');

  // Data states
  const [items, setItems] = useState<Item[]>([]);
  const [units, setUnits] = useState<Unit[]>([]);
  const [taxes, setTaxes] = useState<TaxMaster[]>([]);
  const [categories, setCategories] = useState<GlobalCategory[]>([]);
  const [taxNatures, setTaxNatures] = useState<TaxNature[]>([]);
  const [chargeTypes, setChargeTypes] = useState<ChargeType[]>([]);
  const [loading, setLoading] = useState(true);

  // Modal states
  const [isUnitModalOpen, setIsUnitModalOpen] = useState(false);
  const [isItemModalOpen, setIsItemModalOpen] = useState(false);
  const [isTaxModalOpen, setIsTaxModalOpen] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [modalError, setModalError] = useState('');

  // Unit Form
  const [unitName, setUnitName] = useState('');
  const [unitCode, setUnitCode] = useState('');

  // Item Form
  const [itemName, setItemName] = useState('');
  const [itemCode, setItemCode] = useState('');
  const [itemCategoryId, setItemCategoryId] = useState<number>(0);
  const [itemDesc, setItemDesc] = useState('');
  const [selectedUnitIds, setSelectedUnitIds] = useState<number[]>([]);

  // Tax Master Form
  const [taxName, setTaxName] = useState('');
  const [taxCode, setTaxCode] = useState('');
  const [taxNatureId, setTaxNatureId] = useState<number>(0);
  const [taxChargeTypeId, setTaxChargeTypeId] = useState<number>(0);
  const [taxValue, setTaxValue] = useState<number>(18);

  const loadAllData = async () => {
    setLoading(true);
    try {
      const [itemsRes, unitsRes, taxesRes, catsRes, naturesRes, chargesRes] = await Promise.all([
        isOrg ? api.getItems(token).catch(() => []) : Promise.resolve([]),
        api.getUnits(token).catch(() => []),
        isVendor ? api.getTaxMasters(token).catch(() => []) : Promise.resolve([]),
        isOrg ? api.getCategories(token).catch(() => []) : Promise.resolve([]),
        isVendor ? api.getTaxNatures(token).catch(() => []) : Promise.resolve([]),
        isVendor ? api.getChargeTypes(token).catch(() => []) : Promise.resolve([]),
      ]);

      setItems(itemsRes);
      setUnits(unitsRes);
      setTaxes(taxesRes);
      setCategories(catsRes);
      setTaxNatures(naturesRes);
      setChargeTypes(chargesRes);

      if (catsRes.length > 0 && itemCategoryId === 0) {
        setItemCategoryId(catsRes[0].id);
      }
      if (naturesRes.length > 0 && taxNatureId === 0) {
        setTaxNatureId(naturesRes[0].id);
      }
      if (chargesRes.length > 0 && taxChargeTypeId === 0) {
        setTaxChargeTypeId(chargesRes[0].id);
      }
    } catch (err: any) {
      onShowToast(err.message || 'Failed to load master catalog data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const [loadingCategories, setLoadingCategories] = useState(false);

  const loadCategories = async () => {
    setLoadingCategories(true);
    try {
      const catsRes = await api.getCategories(token);
      if (catsRes && catsRes.length > 0) {
        setCategories(catsRes);
        if (!itemCategoryId || itemCategoryId === 0) {
          setItemCategoryId(catsRes[0].id);
        }
      }
    } catch (err: any) {
      console.warn('Failed to load item categories:', err);
    } finally {
      setLoadingCategories(false);
    }
  };

  useEffect(() => {
    loadAllData();
  }, [token]);

  useEffect(() => {
    if (isItemModalOpen) {
      loadCategories();
    }
  }, [isItemModalOpen, token]);

  // Handle Unit Submit
  const handleUnitSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!unitName.trim() || !unitCode.trim()) {
      setModalError('Unit Name and Code are required.');
      return;
    }
    setSubmitting(true);
    setModalError('');
    try {
      await api.createUnit(token, {
        name: unitName.trim(),
        code: unitCode.trim().toUpperCase(),
        statusId: 1,
        statusRemarks: 'Active',
      });
      onShowToast(`Unit "${unitName}" created successfully!`, 'success');
      setIsUnitModalOpen(false);
      setUnitName('');
      setUnitCode('');
      loadAllData();
    } catch (err: any) {
      setModalError(err.message || 'Failed to create unit.');
    } finally {
      setSubmitting(false);
    }
  };

  // Handle Item Submit
  const handleItemSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isOrg) {
      onShowToast('Vendors are not authorized to create catalog items.', 'error');
      return;
    }
    if (!itemName.trim() || !itemCode.trim()) {
      setModalError('Item Name and Code are required.');
      return;
    }
    setSubmitting(true);
    setModalError('');
    try {
      await api.createItem(token, {
        name: itemName.trim(),
        code: itemCode.trim().toUpperCase(),
        categoryId: itemCategoryId || (categories[0]?.id ?? 1),
        itemDescription: itemDesc.trim(),
        statusId: 1,
        statusRemarks: 'Active',
        unitIds: selectedUnitIds.length > 0 ? selectedUnitIds : units[0] ? [units[0].id] : [],
      });
      onShowToast(`Item "${itemName}" registered successfully!`, 'success');
      setIsItemModalOpen(false);
      setItemName('');
      setItemCode('');
      setItemDesc('');
      setSelectedUnitIds([]);
      loadAllData();
    } catch (err: any) {
      setModalError(err.message || 'Failed to create item.');
    } finally {
      setSubmitting(false);
    }
  };

  // Handle Tax Master Submit
  const handleTaxSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!taxName.trim() || !taxCode.trim()) {
      setModalError('Tax Name and Code are required.');
      return;
    }
    setSubmitting(true);
    setModalError('');
    try {
      await api.createTaxMaster(token, {
        name: taxName.trim(),
        code: taxCode.trim().toUpperCase(),
        taxNatureId: taxNatureId || (taxNatures[0]?.id ?? 1),
        chargeTypeId: taxChargeTypeId || (chargeTypes[0]?.id ?? 1),
        taxValue: Number(taxValue) || 0,
        statusId: 1,
        statusRemarks: 'Active',
      });
      onShowToast(`Tax Master "${taxName}" created successfully!`, 'success');
      setIsTaxModalOpen(false);
      setTaxName('');
      setTaxCode('');
      setTaxValue(18);
      loadAllData();
    } catch (err: any) {
      setModalError(err.message || 'Failed to create tax master.');
    } finally {
      setSubmitting(false);
    }
  };

  // Delete Handlers
  const handleDeleteUnit = async (id: number, name: string) => {
    if (!confirm(`Are you sure you want to delete unit "${name}"?`)) return;
    try {
      await api.deleteUnit(token, id);
      onShowToast(`Unit "${name}" deleted.`, 'info');
      loadAllData();
    } catch (err: any) {
      onShowToast(err.message || 'Cannot delete unit. It may be referenced in catalog items.', 'error');
    }
  };

  const handleDeleteItem = async (id: number, name: string) => {
    if (!confirm(`Are you sure you want to delete item "${name}"?`)) return;
    try {
      await api.deleteItem(token, id);
      onShowToast(`Item "${name}" deleted.`, 'info');
      loadAllData();
    } catch (err: any) {
      onShowToast(err.message || 'Cannot delete item. It may be referenced in auction lines.', 'error');
    }
  };

  const handleDeleteTax = async (id: number, name: string) => {
    if (!confirm(`Are you sure you want to delete tax rule "${name}"?`)) return;
    try {
      await api.deleteTaxMaster(token, id);
      onShowToast(`Tax master "${name}" deleted.`, 'info');
      loadAllData();
    } catch (err: any) {
      onShowToast(err.message || 'Cannot delete tax master.', 'error');
    }
  };

  const toggleUnitSelection = (unitId: number) => {
    setSelectedUnitIds((prev) =>
      prev.includes(unitId) ? prev.filter((id) => id !== unitId) : [...prev, unitId]
    );
  };

  // Filtered queries
  const q = searchQuery.toLowerCase();
  const filteredItems = items.filter(
    (i) => (i.name || i.itemName || '').toLowerCase().includes(q) || (i.code || '').toLowerCase().includes(q)
  );
  const filteredUnits = units.filter(
    (u) => (u.name || u.unitName || '').toLowerCase().includes(q) || (u.code || u.alias || '').toLowerCase().includes(q)
  );
  const filteredTaxes = taxes.filter(
    (t) => (t.name || '').toLowerCase().includes(q) || (t.code || '').toLowerCase().includes(q)
  );

  return (
    <div className="bn-masters-workspace">
      {/* Header Banner */}
      <div className="bn-hero-banner bn-mb-4">
        <div>
          <span className="bn-eyebrow">ENTERPRISE CONFIGURATION</span>
          <h2 className="bn-hero-title">Master Catalog & Data Dictionary</h2>
          <p className="bn-hero-subtitle">
            {isOrg
              ? `Manage procurement items and measurement units for Tenant #${claims.tenantId}.`
              : `Manage your tax schedules and measurement units for Tenant #${claims.tenantId}.`}
          </p>
        </div>
        <div className="bn-flex-center gap-2">
          <Badge tone="info" icon={false}>Tenant Isolated</Badge>
          <Badge tone="live" icon={true}>{claims.role} Workspace</Badge>
        </div>
      </div>

      {/* Summary KPI Cards */}
      <div className="bn-stats-row bn-mb-4">
        {isOrg && (
          <div className="bn-stat-card">
            <div className="bn-stat-icon bn-tone-cyan"><Package size={20} /></div>
            <div className="bn-stat-value">{items.length}</div>
            <div className="bn-stat-title">Catalog Items</div>
            <div className="bn-stat-sub">Procurement materials & line specs</div>
          </div>
        )}
        <div className="bn-stat-card">
          <div className="bn-stat-icon bn-tone-amber"><Ruler size={20} /></div>
          <div className="bn-stat-value">{units.length}</div>
          <div className="bn-stat-title">Units of Measure</div>
          <div className="bn-stat-sub">Metric tons, pieces, liters, etc.</div>
        </div>
        {isVendor && (
          <div className="bn-stat-card">
            <div className="bn-stat-icon bn-tone-green"><Percent size={20} /></div>
            <div className="bn-stat-value">{taxes.length}</div>
            <div className="bn-stat-title">Tax Masters</div>
            <div className="bn-stat-sub">GST, VAT, custom duty rates</div>
          </div>
        )}
      </div>

      {/* Sub Tabs and Actions Bar */}
      <div className="bn-workstation-tabs-bar bn-mb-4 bn-flex-between">
        <div className="bn-tabs-list">
          {isOrg && (
            <button
              className={`bn-tab-btn ${activeTab === 'items' ? 'active' : ''}`}
              onClick={() => { setActiveTab('items'); setSearchQuery(''); }}
            >
              <Package size={16} /> Items Catalog ({items.length})
            </button>
          )}
          <button
            className={`bn-tab-btn ${activeTab === 'units' ? 'active' : ''}`}
            onClick={() => { setActiveTab('units'); setSearchQuery(''); }}
          >
            <Ruler size={16} /> Units of Measure ({units.length})
          </button>
          {isVendor && (
            <button
              className={`bn-tab-btn ${activeTab === 'taxes' ? 'active' : ''}`}
              onClick={() => { setActiveTab('taxes'); setSearchQuery(''); }}
            >
              <Percent size={16} /> Tax Masters ({taxes.length})
            </button>
          )}
        </div>

        <div className="bn-flex-center gap-2">
          <div className="bn-search-box" style={{ width: '220px' }}>
            <Search size={15} className="bn-search-icon" />
            <input
              type="text"
              className="bn-search-input"
              placeholder={`Search ${activeTab}...`}
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          {activeTab === 'items' && isOrg && (
            <Button
              variant="primary"
              size="sm"
              icon={<Plus size={15} />}
              onClick={() => { setModalError(''); setIsItemModalOpen(true); }}
            >
              Add Item
            </Button>
          )}

          {activeTab === 'units' && (
            <Button
              variant="primary"
              size="sm"
              icon={<Plus size={15} />}
              onClick={() => { setModalError(''); setIsUnitModalOpen(true); }}
            >
              Add Unit
            </Button>
          )}

          {activeTab === 'taxes' && isVendor && (
            <Button
              variant="primary"
              size="sm"
              icon={<Plus size={15} />}
              onClick={() => { setModalError(''); setIsTaxModalOpen(true); }}
            >
              Add Tax Rule
            </Button>
          )}
        </div>
      </div>

      {/* Main Content Area */}
      {loading ? (
        <LoadingState message="Loading tenant master catalog..." />
      ) : (
        <>
          {/* Items Tab */}
          {activeTab === 'items' && (
            <div className="bn-leaderboard-container">
              {filteredItems.length === 0 ? (
                <div className="bn-empty-stream-panel">
                  <Package size={36} className="bn-text-muted" />
                  <h4>No Catalog Items Defined</h4>
                  <p>Register materials, commodities, or services in your catalog to enable auction requirement creation.</p>
                  {isOrg && (
                    <Button
                      variant="primary"
                      size="sm"
                      icon={<Plus size={14} />}
                      onClick={() => setIsItemModalOpen(true)}
                      className="bn-mt-2"
                    >
                      Add First Item
                    </Button>
                  )}
                </div>
              ) : (
                <div className="bn-table-responsive">
                  <table className="bn-table">
                    <thead>
                      <tr>
                        <th style={{ width: '70px' }}>ID</th>
                        <th style={{ width: '130px' }}>Code</th>
                        <th>Item Name</th>
                        <th>Category</th>
                        <th>Applicable Units</th>
                        {isOrg && <th style={{ width: '80px' }}>Actions</th>}
                      </tr>
                    </thead>
                    <tbody>
                      {filteredItems.map((item) => (
                        <tr key={item.id}>
                          <td><strong>#{item.id}</strong></td>
                          <td><span className="bn-text-mono bn-badge bn-badge-info">{item.code || 'N/A'}</span></td>
                          <td>
                            <strong>{item.name || item.itemName}</strong>
                            {item.about && <p className="bn-text-xs bn-text-muted bn-m-0">{item.about}</p>}
                          </td>
                          <td>
                            {categories.find((c) => c.id === item.categoryId)?.name || 'General'}
                          </td>
                          <td>
                            <div className="bn-flex-center gap-1">
                              {item.applicableUnits && item.applicableUnits.length > 0 ? (
                                item.applicableUnits.map((u: any, idx: number) => (
                                  <span key={idx} className="bn-badge bn-badge-scheduled">
                                    {u.unit?.name || u.unit?.code || `Unit #${u.unitId}`}
                                  </span>
                                ))
                              ) : (
                                <span className="bn-text-muted bn-text-xs">All Standard Units</span>
                              )}
                            </div>
                          </td>
                          {isOrg && (
                            <td>
                              <button
                                className="bn-icon-btn bn-text-danger"
                                onClick={() => handleDeleteItem(item.id, item.name || item.itemName || `Item #${item.id}`)}
                                title="Delete Item"
                              >
                                <Trash2 size={14} />
                              </button>
                            </td>
                          )}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* Units Tab */}
          {activeTab === 'units' && (
            <div className="bn-leaderboard-container">
              {filteredUnits.length === 0 ? (
                <div className="bn-empty-stream-panel">
                  <Ruler size={36} className="bn-text-muted" />
                  <h4>No Units of Measure</h4>
                  <p>Configure metric measurement units (e.g. Metric Tons, Pieces, Liters, Meters) for quantity definitions.</p>
                  <Button
                    variant="primary"
                    size="sm"
                    icon={<Plus size={14} />}
                    onClick={() => setIsUnitModalOpen(true)}
                    className="bn-mt-2"
                  >
                    Add First Unit
                  </Button>
                </div>
              ) : (
                <div className="bn-table-responsive">
                  <table className="bn-table">
                    <thead>
                      <tr>
                        <th style={{ width: '70px' }}>ID</th>
                        <th style={{ width: '130px' }}>Code</th>
                        <th>Unit Name / Description</th>
                        <th style={{ width: '120px' }}>Status</th>
                        <th style={{ width: '80px' }}>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredUnits.map((unit) => (
                        <tr key={unit.id}>
                          <td><strong>#{unit.id}</strong></td>
                          <td><span className="bn-text-mono bn-badge bn-badge-amber">{unit.code || unit.alias || 'N/A'}</span></td>
                          <td><strong>{unit.name || unit.unitName}</strong></td>
                          <td><Badge tone="live">Active</Badge></td>
                          <td>
                            <button
                              className="bn-icon-btn bn-text-danger"
                              onClick={() => handleDeleteUnit(unit.id, unit.name || unit.unitName || `Unit #${unit.id}`)}
                              title="Delete Unit"
                            >
                              <Trash2 size={14} />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* Tax Masters Tab — Vendor Only */}
          {activeTab === 'taxes' && isVendor && (
            <div className="bn-leaderboard-container">
              {filteredTaxes.length === 0 ? (
                <div className="bn-empty-stream-panel">
                  <Percent size={36} className="bn-text-muted" />
                  <h4>No Tax Masters Defined</h4>
                  <p>Define your tax rates, duties, or deductions that will be applied during the live bidding process.</p>
                  <Button
                    variant="primary"
                    size="sm"
                    icon={<Plus size={14} />}
                    onClick={() => setIsTaxModalOpen(true)}
                    className="bn-mt-2"
                  >
                    Add First Tax Rule
                  </Button>
                </div>
              ) : (
                <div className="bn-table-responsive">
                  <table className="bn-table">
                    <thead>
                      <tr>
                        <th style={{ width: '70px' }}>ID</th>
                        <th style={{ width: '130px' }}>Tax Code</th>
                        <th>Tax Name</th>
                        <th>Nature</th>
                        <th>Charge Type</th>
                        <th>Default Rate</th>
                        <th style={{ width: '80px' }}>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredTaxes.map((tax) => {
                        const nature = taxNatures.find((n) => n.id === tax.taxNatureId);
                        const charge = chargeTypes.find((c) => c.id === tax.chargeTypeId);
                        return (
                          <tr key={tax.id}>
                            <td><strong>#{tax.id}</strong></td>
                            <td><span className="bn-text-mono bn-badge bn-badge-info">{tax.code}</span></td>
                            <td><strong>{tax.name}</strong></td>
                            <td>
                              <span className="bn-badge bn-badge-scheduled">
                                {nature?.name || `Nature #${tax.taxNatureId}`}
                              </span>
                            </td>
                            <td>
                              <span className="bn-badge bn-badge-scheduled">
                                {charge?.name || `Type #${tax.chargeTypeId}`}
                              </span>
                            </td>
                            <td>
                              <strong className="bn-font-mono">{getTaxDisplayRate(tax)}</strong>
                            </td>
                            <td>
                              <button
                                className="bn-icon-btn bn-text-danger"
                                onClick={() => handleDeleteTax(tax.id, tax.name)}
                                title="Delete Tax Master"
                              >
                                <Trash2 size={14} />
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}
        </>
      )}

      {/* Create Unit Modal */}
      {isUnitModalOpen && (
        <Modal
          isOpen={isUnitModalOpen}
          onClose={() => setIsUnitModalOpen(false)}
          title="Add Measurement Unit"
          subtitle="Define a measurement unit for auction requirement line items"
          maxWidth="sm"
          footer={
            <div className="bn-flex-end gap-2">
              <Button variant="outline" onClick={() => setIsUnitModalOpen(false)} disabled={submitting}>
                Cancel
              </Button>
              <Button variant="primary" onClick={handleUnitSubmit} loading={submitting}>
                Save Unit
              </Button>
            </div>
          }
        >
          <form onSubmit={handleUnitSubmit} className="bn-form-stack">
            {modalError && (
              <div className="bn-auth-error">
                <AlertCircle size={16} /> {modalError}
              </div>
            )}
            <Input
              label="Unit Name"
              placeholder="e.g. Metric Ton, Pieces, Liters"
              value={unitName}
              onChange={(e) => setUnitName(e.target.value)}
              required
            />
            <Input
              label="Unit Code / Symbol"
              placeholder="e.g. MT, PCS, LTR, M"
              value={unitCode}
              onChange={(e) => setUnitCode(e.target.value)}
              required
            />
          </form>
        </Modal>
      )}

      {/* Create Item Modal */}
      {isItemModalOpen && isOrg && (
        <Modal
          isOpen={isItemModalOpen && isOrg}
          onClose={() => setIsItemModalOpen(false)}
          title="Add Catalog Item"
          subtitle="Define a material, component, or service for your procurement master"
          maxWidth="md"
          footer={
            <div className="bn-flex-end gap-2">
              <Button variant="outline" onClick={() => setIsItemModalOpen(false)} disabled={submitting}>
                Cancel
              </Button>
              <Button variant="primary" onClick={handleItemSubmit} loading={submitting}>
                Register Item
              </Button>
            </div>
          }
        >
          <form onSubmit={handleItemSubmit} className="bn-form-stack">
            {modalError && (
              <div className="bn-auth-error">
                <AlertCircle size={16} /> {modalError}
              </div>
            )}
            <div className="bn-grid-2">
              <Input
                label="Item Name"
                placeholder="e.g. High Tensile Structural Steel"
                value={itemName}
                onChange={(e) => setItemName(e.target.value)}
                required
              />
              <Input
                label="Item Code / SKU"
                placeholder="e.g. STL-PLT-001"
                value={itemCode}
                onChange={(e) => setItemCode(e.target.value)}
                required
              />
            </div>

            <Select
              label="Item Category"
              value={String(itemCategoryId || (categories[0]?.id ?? 1))}
              onChange={(e) => setItemCategoryId(Number(e.target.value))}
              hint={loadingCategories ? 'Loading available categories...' : undefined}
              options={
                categories.length > 0
                  ? categories.map((c) => ({
                      value: String(c.id),
                      label: `${c.name || 'Category'} (${c.code || '#' + c.id})`,
                    }))
                  : [
                      { value: '1', label: 'General Procurement (GEN)' },
                      { value: '2', label: 'Raw Materials & Metals (RAW)' },
                      { value: '3', label: 'Machinery & Equipment (EQP)' },
                      { value: '4', label: 'Services & Operations (SVC)' },
                    ]
              }
            />

            <Textarea
              label="Description / Specifications"
              placeholder="General technical properties, standards (ISO, ASTM), or composition."
              rows={2}
              value={itemDesc}
              onChange={(e) => setItemDesc(e.target.value)}
            />

            {units.length > 0 && (
              <div className="bn-field">
                <label className="bn-label">Applicable Units of Measure</label>
                <div className="bn-flex-center gap-2" style={{ flexWrap: 'wrap' }}>
                  {units.map((u) => {
                    const isSelected = selectedUnitIds.includes(u.id);
                    return (
                      <button
                        key={u.id}
                        type="button"
                        className={`bn-badge ${isSelected ? 'bn-badge-winner' : 'bn-badge-scheduled'}`}
                        style={{ cursor: 'pointer', padding: '0.4rem 0.75rem' }}
                        onClick={() => toggleUnitSelection(u.id)}
                      >
                        {isSelected && <CheckCircle2 size={13} />} {u.name || u.unitName} ({u.code || u.alias})
                      </button>
                    );
                  })}
                </div>
              </div>
            )}
          </form>
        </Modal>
      )}

      {/* Create Tax Master Modal — Vendor Only */}
      {isTaxModalOpen && isVendor && (
        <Modal
          isOpen={isTaxModalOpen}
          onClose={() => setIsTaxModalOpen(false)}
          title="Add Tax Master Rule"
          subtitle="Configure default tax schedules for bidding calculation"
          maxWidth="md"
          footer={
            <div className="bn-flex-end gap-2">
              <Button variant="outline" onClick={() => setIsTaxModalOpen(false)} disabled={submitting}>
                Cancel
              </Button>
              <Button variant="primary" onClick={handleTaxSubmit} loading={submitting}>
                Save Tax Rule
              </Button>
            </div>
          }
        >
          <form onSubmit={handleTaxSubmit} className="bn-form-stack">
            {modalError && (
              <div className="bn-auth-error">
                <AlertCircle size={16} /> {modalError}
              </div>
            )}
            <div className="bn-grid-2">
              <Input
                label="Tax Name"
                placeholder="e.g. Standard GST 18%"
                value={taxName}
                onChange={(e) => setTaxName(e.target.value)}
                required
              />
              <Input
                label="Tax Code"
                placeholder="e.g. GST_18, VAT_05"
                value={taxCode}
                onChange={(e) => setTaxCode(e.target.value)}
                required
              />
            </div>

            <div className="bn-grid-2">
              <Select
                label="Tax Nature"
                value={String(taxNatureId)}
                onChange={(e) => setTaxNatureId(Number(e.target.value))}
                options={taxNatures.map((n) => ({ value: String(n.id), label: n.name || `Nature #${n.id}` }))}
              />
              <Select
                label="Charge Type"
                value={String(taxChargeTypeId)}
                onChange={(e) => setTaxChargeTypeId(Number(e.target.value))}
                options={chargeTypes.map((c) => ({ value: String(c.id), label: c.name || `Type #${c.id}` }))}
              />
            </div>

            {(() => {
              const isPerUnit = isPerUnitCharge({ chargeTypeId: taxChargeTypeId });
              const isFixed = isFixedCharge({ chargeTypeId: taxChargeTypeId });
              return (
                <Input
                  label={isPerUnit ? "Tax Amount (₹ per unit)" : isFixed ? "Fixed Tax Amount (₹)" : "Tax Rate (%)"}
                  type="number"
                  step="any"
                  min="0"
                  placeholder={isPerUnit ? "5.00" : isFixed ? "100.00" : "18"}
                  value={String(taxValue)}
                  onChange={(e) => setTaxValue(Number(e.target.value))}
                  required
                />
              );
            })()}
          </form>
        </Modal>
      )}
    </div>
  );
};
