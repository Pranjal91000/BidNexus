import { useEffect, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import type { Auction, AuctionCreateRequest, Item, Unit } from '../../types';
import { api } from '../../services/api';
import { errorMessage, useApp } from '../../lib/appContext';
import { STATUS } from '../../lib/auction';
import { Modal } from '../../components/ui/Modal';
import { Button } from '../../components/ui/Button';
import { Checkbox, TextAreaField, TextField } from '../../components/ui/Field';
import { Callout, Loading } from '../../components/ui/States';

interface LineDraft {
  key: string;
  itemId: number;
  unitId: number;
  quantity: string;
  spec: string;
}

const pad = (n: number) => String(n).padStart(2, '0');
const toLocalInput = (d: Date) => `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
const toDateInput = (d: Date) => `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
const newKey = () => Math.random().toString(36).slice(2);

const itemLabel = (i: Item) => `${i.itemName || i.name || `Item #${i.id}`}${i.code ? ` · ${i.code}` : ''}`;
const unitLabel = (u: Unit) => u.code || u.name || u.unitName || `Unit #${u.id}`;
const unitIdsOf = (i?: Item): number[] =>
  i?.unitIds?.length ? i.unitIds : (i?.applicableUnits ?? []).map((u: { unitId?: number; id?: number }) => u.unitId ?? u.id ?? 0).filter(Boolean);

interface Props {
  /** null → create; an auction → edit. */
  auction: Auction | null;
  onClose: () => void;
  onSaved: (id?: number) => void;
}

export function AuctionFormModal({ auction, onClose, onSaved }: Props) {
  const { token, claims, toast, navigate } = useApp();
  const isEdit = !!auction;

  const [loading, setLoading] = useState(true);
  const [items, setItems] = useState<Item[]>([]);
  const [units, setUnits] = useState<Unit[]>([]);

  const [defaults] = useState(() => {
    const now = new Date();
    const start = new Date(now.getTime() + 60 * 60 * 1000);
    start.setMinutes(0, 0, 0);
    return {
      today: toDateInput(now),
      docNo: `AUC/${now.getFullYear()}/${Math.floor(1000 + Math.random() * 9000)}`,
      start: toLocalInput(start),
      end: toLocalInput(new Date(start.getTime() + 2 * 60 * 60 * 1000)),
    };
  });
  const [name, setName] = useState('');
  const [docNo, setDocNo] = useState(defaults.docNo);
  const [docDate, setDocDate] = useState(defaults.today);
  const [about, setAbout] = useState('');
  const [forward, setForward] = useState(false);
  const [start, setStart] = useState(defaults.start);
  const [end, setEnd] = useState(defaults.end);
  const [openToAll, setOpenToAll] = useState(true);
  const [hidePrices, setHidePrices] = useState(false);
  const [lines, setLines] = useState<LineDraft[]>([]);

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState<'draft' | 'publish' | null>(null);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const [itemList, unitList, full] = await Promise.all([
          api.getItems(token).catch(() => [] as Item[]),
          api.getUnits(token).catch(() => [] as Unit[]),
          auction ? api.getAuctionById(token, auction.id).catch(() => auction) : Promise.resolve(null),
        ]);
        if (cancelled) return;
        setItems(itemList || []);
        setUnits(unitList || []);
        if (full) {
          setName(full.auctionName || '');
          setDocNo(full.docNoYearly || '');
          setDocDate(full.docDate ? String(full.docDate).slice(0, 10) : defaults.today);
          setAbout(full.about || '');
          setForward(!!full.isForwardAuction);
          setStart(toLocalInput(new Date(full.auctionStartTime)));
          setEnd(toLocalInput(new Date(full.auctionEndTime)));
          setOpenToAll(!!full.openToAll);
          setHidePrices(!!full.isBidPriceHidden);
          setLines(
            (full.auctionRequirements ?? []).map((r) => ({
              key: newKey(),
              itemId: r.itemId ?? r.item?.id ?? 0,
              unitId: r.unitId ?? r.unit?.id ?? 0,
              quantity: String(r.quantity ?? ''),
              spec: r.technicalSpecification ?? '',
            })),
          );
        } else if (itemList?.length) {
          const first = itemList[0];
          setLines([{ key: newKey(), itemId: first.id, unitId: unitIdsOf(first)[0] ?? unitList?.[0]?.id ?? 0, quantity: '', spec: '' }]);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [auction?.id, token]);

  const unitsFor = (itemId: number) => {
    const allowed = unitIdsOf(items.find((i) => i.id === itemId));
    return allowed.length ? units.filter((u) => allowed.includes(u.id)) : units;
  };

  const updateLine = (key: string, patch: Partial<LineDraft>) =>
    setLines((prev) =>
      prev.map((l) => {
        if (l.key !== key) return l;
        const next = { ...l, ...patch };
        if (patch.itemId != null) {
          const allowed = unitsFor(patch.itemId);
          if (!allowed.some((u) => u.id === next.unitId)) next.unitId = allowed[0]?.id ?? 0;
        }
        return next;
      }),
    );

  const addLine = () => {
    const first = items[0];
    if (!first) return;
    setLines((prev) => [...prev, { key: newKey(), itemId: first.id, unitId: unitsFor(first.id)[0]?.id ?? 0, quantity: '', spec: '' }]);
  };

  const validate = () => {
    const e: Record<string, string> = {};
    if (!name.trim()) e.name = 'Give the auction a name.';
    if (!docNo.trim()) e.docNo = 'Document number is required.';
    if (!start) e.start = 'Choose a start time.';
    if (!end) e.end = 'Choose an end time.';
    if (start && end && new Date(end) <= new Date(start)) e.end = 'End must be after the start.';
    if (lines.length === 0) e.lines = 'Add at least one item.';
    lines.forEach((l) => {
      if (!(Number(l.quantity) > 0)) e[`qty-${l.key}`] = 'Enter a quantity';
      if (!l.itemId || !l.unitId) e[`item-${l.key}`] = 'Choose an item and unit';
    });
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const save = async (mode: 'draft' | 'publish') => {
    setFormError('');
    if (!validate()) return;
    setSaving(mode);
    const payload: AuctionCreateRequest = {
      auctionName: name.trim(),
      about: about.trim(),
      docNoYearly: docNo.trim(),
      docDate,
      isForwardAuction: forward,
      auctionStartTime: new Date(start).toISOString(),
      auctionEndTime: new Date(end).toISOString(),
      openToAll,
      isBidPriceHidden: hidePrices,
      organizationId: auction?.organizationId || auction?.organization?.id || claims.userId,
      statusId: mode === 'draft' ? STATUS.Draft : STATUS.Authorized,
      docAttachmentId: auction?.docAttachmentId ?? null,
      auctionRequirements: lines.map((l, i) => ({
        lineNo: i + 1,
        itemId: l.itemId,
        unitId: l.unitId,
        quantity: Number(l.quantity),
        technicalSpecification: l.spec.trim(),
        documentAttachmentId: null,
      })),
    };
    try {
      const res = isEdit ? await api.updateAuction(token, auction!.id, payload) : await api.createAuction(token, payload);
      toast(mode === 'draft' ? 'Saved as draft.' : 'Auction published.', 'success');
      onSaved(res?.id ?? auction?.id);
    } catch (err) {
      setFormError(errorMessage(err, 'Could not save the auction.'));
    } finally {
      setSaving(null);
    }
  };

  const noMasters = !loading && (items.length === 0 || units.length === 0);

  return (
    <Modal
      open
      wide
      title={isEdit ? 'Edit auction' : 'New auction'}
      onClose={onClose}
      footer={
        <>
          <Button variant="ghost" onClick={onClose}>Cancel</Button>
          <Button onClick={() => save('draft')} loading={saving === 'draft'} disabled={!!saving || noMasters}>
            Save as draft
          </Button>
          <Button variant="primary" onClick={() => save('publish')} loading={saving === 'publish'} disabled={!!saving || noMasters}>
            {isEdit ? 'Save and publish' : 'Publish'}
          </Button>
        </>
      }
    >
      {loading ? (
        <Loading />
      ) : (
        <>
          {formError && <Callout tone="error">{formError}</Callout>}
          {noMasters && (
            <Callout tone="warn">
              <span className="grow">You need at least one item and one unit before creating an auction.</span>
              <button
                type="button"
                className="link-btn"
                onClick={() => {
                  onClose();
                  navigate({ page: 'masters' });
                }}
              >
                Go to Masters
              </button>
            </Callout>
          )}

          <div className="form-grid">
            <TextField label="Auction name" className="span-2" value={name} error={errors.name} maxLength={150} onChange={(e) => setName(e.target.value)} />
            <TextField label="Document no." value={docNo} error={errors.docNo} maxLength={50} onChange={(e) => setDocNo(e.target.value)} />
            <TextField label="Document date" type="date" value={docDate} onChange={(e) => setDocDate(e.target.value)} />
            <TextAreaField label="Description (optional)" className="span-2" rows={2} maxLength={1000} value={about} onChange={(e) => setAbout(e.target.value)} />
          </div>

          <div className="stack">
            <h3>How it runs</h3>
            <div className="segmented" role="group" aria-label="Auction type">
              <button type="button" className="segmented__option" aria-pressed={!forward} onClick={() => setForward(false)}>
                <span className="strong">Reverse auction</span>
                <span className="small muted">You are buying. Lowest bid wins.</span>
              </button>
              <button type="button" className="segmented__option" aria-pressed={forward} onClick={() => setForward(true)}>
                <span className="strong">Forward auction</span>
                <span className="small muted">You are selling. Highest bid wins.</span>
              </button>
            </div>
            <div className="form-grid">
              <TextField label="Starts" type="datetime-local" value={start} error={errors.start} onChange={(e) => setStart(e.target.value)} />
              <TextField label="Ends" type="datetime-local" value={end} error={errors.end} onChange={(e) => setEnd(e.target.value)} />
            </div>
            <Checkbox label="Open to all vendors" hint="Off: only vendors you approve can bid." checked={openToAll} onChange={setOpenToAll} />
            <Checkbox label="Hide bid amounts from vendors" hint="Vendors see only their own rank, never other bids." checked={hidePrices} onChange={setHidePrices} />
          </div>

          <div className="stack">
            <div className="row row--between">
              <h3>Items</h3>
              <Button size="sm" icon={<Plus size={16} />} onClick={addLine} disabled={items.length === 0}>
                Add item
              </Button>
            </div>
            {errors.lines && <span className="field__error">{errors.lines}</span>}
            {lines.map((l, i) => {
              const lineUnits = unitsFor(l.itemId);
              return (
                <div key={l.key} className="card" style={{ padding: 16 }}>
                  <div className="row row--between" style={{ marginBottom: 12 }}>
                    <span className="small muted">Line {i + 1}</span>
                    <button
                      type="button"
                      className="icon-btn icon-btn--danger"
                      aria-label={`Remove line ${i + 1}`}
                      onClick={() => setLines((prev) => prev.filter((x) => x.key !== l.key))}
                      disabled={lines.length === 1}
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                  <div className="form-grid" style={{ gridTemplateColumns: 'minmax(0,2fr) minmax(0,1fr) minmax(0,1fr)' }}>
                    <div className="field">
                      <label className="field__label" htmlFor={`item-${l.key}`}>Item</label>
                      <select id={`item-${l.key}`} className="select" value={l.itemId} onChange={(e) => updateLine(l.key, { itemId: Number(e.target.value) })}>
                        {items.map((it) => (
                          <option key={it.id} value={it.id}>{itemLabel(it)}</option>
                        ))}
                      </select>
                    </div>
                    <div className="field">
                      <label className="field__label" htmlFor={`qty-${l.key}`}>Quantity</label>
                      <input
                        id={`qty-${l.key}`}
                        className="input"
                        inputMode="decimal"
                        value={l.quantity}
                        aria-invalid={!!errors[`qty-${l.key}`]}
                        onChange={(e) => updateLine(l.key, { quantity: e.target.value.replace(/[^0-9.]/g, '') })}
                      />
                      {errors[`qty-${l.key}`] && <span className="field__error">{errors[`qty-${l.key}`]}</span>}
                    </div>
                    <div className="field">
                      <label className="field__label" htmlFor={`unit-${l.key}`}>Unit</label>
                      <select id={`unit-${l.key}`} className="select" value={l.unitId} onChange={(e) => updateLine(l.key, { unitId: Number(e.target.value) })}>
                        {lineUnits.map((u) => (
                          <option key={u.id} value={u.id}>{unitLabel(u)}</option>
                        ))}
                      </select>
                    </div>
                    <div className="field" style={{ gridColumn: '1 / -1' }}>
                      <label className="field__label" htmlFor={`spec-${l.key}`}>Specification (optional)</label>
                      <input id={`spec-${l.key}`} className="input" maxLength={1000} value={l.spec} onChange={(e) => updateLine(l.key, { spec: e.target.value })} />
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </>
      )}
    </Modal>
  );
}
