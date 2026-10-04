import { useCallback, useEffect, useMemo, useState } from 'react';
import { Pencil, Plus, Search, Trash2 } from 'lucide-react';
import type { ChargeType, GlobalCategory, Item, TaxMaster, TaxNature, Unit } from '../../types';
import { ChargeTypeEnum, TaxNatureEnum } from '../../types';
import { api } from '../../services/api';
import { errorMessage, useApp } from '../../lib/appContext';
import { getTaxDisplayRate } from '../../utils/taxUtils';
import { Button } from '../../components/ui/Button';
import { Tabs } from '../../components/ui/Tabs';
import { Modal, ConfirmDialog } from '../../components/ui/Modal';
import { SelectField, TextAreaField, TextField } from '../../components/ui/Field';
import { Callout, Empty, Loading } from '../../components/ui/States';

type Kind = 'items' | 'units' | 'taxes';
type Editing = { kind: Kind; record: Item | Unit | TaxMaster | null } | null;

const LABEL: Record<Kind, { title: string; one: string; help: string }> = {
  items: { title: 'Items', one: 'item', help: 'What you buy or sell in auctions.' },
  units: { title: 'Units', one: 'unit', help: 'How quantities are measured, like MT or KG.' },
  taxes: { title: 'Taxes', one: 'tax', help: 'Taxes and charges you can add to bid lines.' },
};

const unitText = (u: Unit) => u.name || u.unitName || u.code || `Unit #${u.id}`;
const itemText = (i: Item) => i.itemName || i.name || `Item #${i.id}`;

export function MastersPage() {
  const { token, isOrg, toast } = useApp();
  const kinds: Kind[] = isOrg ? ['items', 'units'] : ['taxes', 'units'];
  const [kind, setKind] = useState<Kind>(kinds[0]);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(true);

  const [items, setItems] = useState<Item[]>([]);
  const [units, setUnits] = useState<Unit[]>([]);
  const [taxes, setTaxes] = useState<TaxMaster[]>([]);
  const [categories, setCategories] = useState<GlobalCategory[]>([]);
  const [natures, setNatures] = useState<TaxNature[]>([]);
  const [charges, setCharges] = useState<ChargeType[]>([]);

  const [editing, setEditing] = useState<Editing>(null);
  const [deleting, setDeleting] = useState<{ kind: Kind; id: number; name: string } | null>(null);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    const safe = <T,>(p: Promise<T[]>) => p.catch(() => [] as T[]);
    const [it, un, tx, ca, na, ch] = await Promise.all([
      isOrg ? safe(api.getItems(token)) : Promise.resolve([] as Item[]),
      safe(api.getUnits(token)),
      !isOrg ? safe(api.getTaxMasters(token)) : Promise.resolve([] as TaxMaster[]),
      isOrg ? safe(api.getCategories(token)) : Promise.resolve([] as GlobalCategory[]),
      !isOrg ? safe(api.getTaxNatures(token)) : Promise.resolve([] as TaxNature[]),
      !isOrg ? safe(api.getChargeTypes(token)) : Promise.resolve([] as ChargeType[]),
    ]);
    setItems(it);
    setUnits(un);
    setTaxes(tx);
    setCategories(ca);
    setNatures(na);
    setCharges(ch);
    setLoading(false);
  }, [token, isOrg]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- fetch on mount
    load();
  }, [load]);

  const counts: Record<Kind, number> = { items: items.length, units: units.length, taxes: taxes.length };
  const q = query.trim().toLowerCase();
  const match = (...parts: (string | undefined)[]) => !q || parts.join(' ').toLowerCase().includes(q);

  const categoryName = useMemo(() => new Map(categories.map((c) => [c.id, c.name ?? `Category ${c.id}`])), [categories]);
  const unitName = useMemo(() => new Map(units.map((u) => [u.id, u.code || unitText(u)])), [units]);

  const remove = async () => {
    if (!deleting) return;
    setBusy(true);
    try {
      if (deleting.kind === 'items') await api.deleteItem(token, deleting.id);
      if (deleting.kind === 'units') await api.deleteUnit(token, deleting.id);
      if (deleting.kind === 'taxes') await api.deleteTaxMaster(token, deleting.id);
      toast(`Deleted ${deleting.name}.`, 'success');
      setDeleting(null);
      load();
    } catch (err) {
      toast(errorMessage(err, `Could not delete ${deleting.name}. It may be in use.`), 'error');
    } finally {
      setBusy(false);
    }
  };

  const actions = (k: Kind, record: Item | Unit | TaxMaster, name: string) => (
    <td style={{ width: 96 }}>
      <span className="row row--end" style={{ gap: 0, flexWrap: 'nowrap' }}>
        <button type="button" className="icon-btn" aria-label={`Edit ${name}`} onClick={() => setEditing({ kind: k, record })}>
          <Pencil size={16} />
        </button>
        <button type="button" className="icon-btn icon-btn--danger" aria-label={`Delete ${name}`} onClick={() => setDeleting({ kind: k, id: record.id, name })}>
          <Trash2 size={16} />
        </button>
      </span>
    </td>
  );

  let table;
  if (kind === 'items') {
    const rows = items.filter((i) => match(itemText(i), i.code));
    table = rows.length ? (
      <table className="table">
        <thead>
          <tr><th>Item</th><th>Category</th><th>Units</th><th aria-label="Actions" /></tr>
        </thead>
        <tbody>
          {rows.map((i) => (
            <tr key={i.id}>
              <td>
                <span className="strong">{itemText(i)}</span>
                <span className="cell-sub">{[i.code, i.itemDescription].filter(Boolean).join(' · ')}</span>
              </td>
              <td>{i.categoryId ? categoryName.get(i.categoryId) ?? '—' : '—'}</td>
              <td className="muted">{(i.unitIds ?? []).map((id) => unitName.get(id)).filter(Boolean).join(', ') || '—'}</td>
              {actions('items', i, itemText(i))}
            </tr>
          ))}
        </tbody>
      </table>
    ) : null;
  } else if (kind === 'units') {
    const rows = units.filter((u) => match(unitText(u), u.code));
    table = rows.length ? (
      <table className="table">
        <thead>
          <tr><th>Unit</th><th>Code</th><th aria-label="Actions" /></tr>
        </thead>
        <tbody>
          {rows.map((u) => (
            <tr key={u.id}>
              <td className="strong">{unitText(u)}</td>
              <td className="muted">{u.code || '—'}</td>
              {actions('units', u, unitText(u))}
            </tr>
          ))}
        </tbody>
      </table>
    ) : null;
  } else {
    const rows = taxes.filter((t) => match(t.name, t.code));
    table = rows.length ? (
      <table className="table">
        <thead>
          <tr><th>Tax</th><th>Effect</th><th className="num">Rate</th><th aria-label="Actions" /></tr>
        </thead>
        <tbody>
          {rows.map((t) => (
            <tr key={t.id}>
              <td>
                <span className="strong">{t.name}</span>
                <span className="cell-sub">{t.code}</span>
              </td>
              <td className={t.taxNatureId === TaxNatureEnum.Deductive ? 'warn' : ''}>
                {t.taxNatureId === TaxNatureEnum.Deductive ? 'Deducts' : 'Adds'}
              </td>
              <td className="num">{getTaxDisplayRate(t)}</td>
              {actions('taxes', t, t.name)}
            </tr>
          ))}
        </tbody>
      </table>
    ) : null;
  }

  return (
    <main className="page">
      <div className="page-header">
        <div className="page-header__text">
          <h1>Masters</h1>
          <p className="muted">{LABEL[kind].help}</p>
        </div>
        <Button variant="primary" icon={<Plus size={18} />} onClick={() => setEditing({ kind, record: null })}>
          Add {LABEL[kind].one}
        </Button>
      </div>

      <div className="stack">
        <div className="toolbar">
          <Tabs
            label="Master type"
            tabs={kinds.map((k) => ({ id: k, label: LABEL[k].title, count: counts[k] }))}
            active={kind}
            onChange={(k) => {
              setKind(k);
              setQuery('');
            }}
          />
          <div className="search">
            <Search size={16} />
            <label className="sr-only" htmlFor="master-search">Search {LABEL[kind].title.toLowerCase()}</label>
            <input id="master-search" className="input" placeholder="Search name or code" value={query} onChange={(e) => setQuery(e.target.value)} />
          </div>
        </div>

        {loading ? (
          <Loading />
        ) : table ? (
          <div className="table-wrap">{table}</div>
        ) : (
          <div className="card">
            <Empty
              title={q ? 'No matches' : `No ${LABEL[kind].title.toLowerCase()} yet`}
              action={!q && <Button onClick={() => setEditing({ kind, record: null })}>Add {LABEL[kind].one}</Button>}
            />
          </div>
        )}
      </div>

      {editing && (
        <MasterForm
          editing={editing}
          units={units}
          categories={categories}
          natures={natures}
          charges={charges}
          onClose={() => setEditing(null)}
          onSaved={() => {
            setEditing(null);
            load();
          }}
        />
      )}

      <ConfirmDialog
        open={!!deleting}
        title={`Delete ${deleting?.name ?? ''}?`}
        message="It will no longer be available for new auctions or bids."
        confirmLabel="Delete"
        danger
        busy={busy}
        onConfirm={remove}
        onCancel={() => setDeleting(null)}
      />
    </main>
  );
}

interface FormProps {
  editing: NonNullable<Editing>;
  units: Unit[];
  categories: GlobalCategory[];
  natures: TaxNature[];
  charges: ChargeType[];
  onClose: () => void;
  onSaved: () => void;
}

function MasterForm({ editing, units, categories, natures, charges, onClose, onSaved }: FormProps) {
  const { token, toast } = useApp();
  const { kind, record } = editing;
  const isEdit = !!record;
  const item = kind === 'items' ? (record as Item | null) : null;
  const tax = kind === 'taxes' ? (record as TaxMaster | null) : null;

  const [name, setName] = useState(record ? (kind === 'items' ? itemText(record as Item) : kind === 'units' ? unitText(record as Unit) : (record as TaxMaster).name) : '');
  const [code, setCode] = useState(record?.code ?? '');
  const [categoryId, setCategoryId] = useState<number>(item?.categoryId ?? categories[0]?.id ?? 0);
  const [description, setDescription] = useState(item?.itemDescription ?? item?.about ?? '');
  const [unitIds, setUnitIds] = useState<number[]>(item?.unitIds ?? []);
  const [natureId, setNatureId] = useState<number>(tax?.taxNatureId ?? natures[0]?.id ?? TaxNatureEnum.Additive);
  const [chargeId, setChargeId] = useState<number>(tax?.chargeTypeId ?? charges[0]?.id ?? ChargeTypeEnum.Percentage);
  const [value, setValue] = useState(tax ? String(tax.taxValue) : '');
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const save = async () => {
    if (!name.trim() || !code.trim()) return setError('Name and code are required.');
    if (kind === 'items' && unitIds.length === 0) return setError('Choose at least one unit for this item.');
    if (kind === 'taxes' && (isNaN(parseFloat(value)) || parseFloat(value) < 0)) return setError('Enter a rate of 0 or more.');
    setSaving(true);
    setError('');
    const base = { name: name.trim(), code: code.trim().toUpperCase(), statusId: 1, statusRemarks: 'Active' };
    try {
      if (kind === 'items') {
        const body = { ...base, categoryId: categoryId || categories[0]?.id || 1, itemDescription: description.trim(), unitIds };
        if (isEdit) await api.updateItem(token, record!.id, body);
        else await api.createItem(token, body);
      } else if (kind === 'units') {
        if (isEdit) await api.updateUnit(token, record!.id, base);
        else await api.createUnit(token, base);
      } else {
        const body = { ...base, taxNatureId: natureId, chargeTypeId: chargeId, taxValue: parseFloat(value) };
        if (isEdit) await api.updateTaxMaster(token, record!.id, body);
        else await api.createTaxMaster(token, body);
      }
      toast(`${name.trim()} ${isEdit ? 'updated' : 'added'}.`, 'success');
      onSaved();
    } catch (err) {
      setError(errorMessage(err, 'Could not save.'));
    } finally {
      setSaving(false);
    }
  };

  const noun = LABEL[kind].one;
  return (
    <Modal
      open
      title={`${isEdit ? 'Edit' : 'Add'} ${noun}`}
      onClose={onClose}
      footer={
        <>
          <Button variant="ghost" onClick={onClose}>Cancel</Button>
          <Button variant="primary" onClick={save} loading={saving}>{isEdit ? 'Save changes' : `Add ${noun}`}</Button>
        </>
      }
    >
      {error && <Callout tone="error">{error}</Callout>}
      <div className="form-grid">
        <TextField label="Name" value={name} onChange={(e) => setName(e.target.value)} />
        <TextField label="Code" hint={kind === 'units' ? 'Short label, e.g. MT' : undefined} value={code} onChange={(e) => setCode(e.target.value)} />

        {kind === 'items' && (
          <>
            <SelectField
              label="Category"
              className="span-2"
              value={categoryId}
              onChange={(e) => setCategoryId(Number(e.target.value))}
              options={categories.length ? categories.map((c) => ({ value: c.id, label: c.name ?? `Category ${c.id}` })) : [{ value: 0, label: 'No categories available' }]}
            />
            <TextAreaField label="Description (optional)" className="span-2" rows={2} value={description} onChange={(e) => setDescription(e.target.value)} />
            <fieldset className="span-2 stack-sm" style={{ border: 0, padding: 0, margin: 0 }}>
              <legend className="field__label" style={{ marginBottom: 8 }}>Units it can be ordered in</legend>
              {units.length ? (
                <div className="chip-select">
                  {units.map((u) => {
                    const on = unitIds.includes(u.id);
                    return (
                      <button key={u.id} type="button" aria-pressed={on} onClick={() => setUnitIds((p) => (on ? p.filter((x) => x !== u.id) : [...p, u.id]))}>
                        {u.code || unitText(u)}
                      </button>
                    );
                  })}
                </div>
              ) : (
                <span className="small muted">Add a unit first.</span>
              )}
            </fieldset>
          </>
        )}

        {kind === 'taxes' && (
          <>
            <SelectField
              label="Effect"
              value={natureId}
              onChange={(e) => setNatureId(Number(e.target.value))}
              options={(natures.length ? natures : [{ id: TaxNatureEnum.Additive, name: 'Additive' }, { id: TaxNatureEnum.Deductive, name: 'Deductive' }]).map((n) => ({
                value: n.id,
                label: n.id === TaxNatureEnum.Deductive ? 'Deducts from amount' : 'Adds to amount',
              }))}
            />
            <SelectField
              label="Charged as"
              value={chargeId}
              onChange={(e) => setChargeId(Number(e.target.value))}
              options={(charges.length ? charges : [{ id: ChargeTypeEnum.Percentage, name: 'Percentage' }, { id: ChargeTypeEnum.Fixed, name: 'Fixed' }, { id: ChargeTypeEnum.PerUnit, name: 'Per unit' }]).map((c) => ({
                value: c.id,
                label: c.id === ChargeTypeEnum.Percentage ? 'Percent of amount' : c.id === ChargeTypeEnum.Fixed ? 'Fixed amount' : 'Per unit',
              }))}
            />
            <TextField
              label={chargeId === ChargeTypeEnum.Percentage ? 'Rate (%)' : 'Amount (₹)'}
              inputMode="decimal"
              value={value}
              onChange={(e) => setValue(e.target.value.replace(/[^0-9.]/g, ''))}
            />
          </>
        )}
      </div>
    </Modal>
  );
}
