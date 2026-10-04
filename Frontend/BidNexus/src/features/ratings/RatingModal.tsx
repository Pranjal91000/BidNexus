import { useEffect, useState } from 'react';
import type { RatingFor, RatingParameter } from '../../types';
import { api } from '../../services/api';
import { errorMessage, useApp } from '../../lib/appContext';
import { Modal } from '../../components/ui/Modal';
import { Button } from '../../components/ui/Button';
import { TextAreaField } from '../../components/ui/Field';
import { Callout, Loading } from '../../components/ui/States';

interface Props {
  auctionId: number;
  targetName: string;
  targetRole: 'Vendor' | 'Organization';
  onClose: () => void;
  onSubmitted: () => void;
}

const SCORES = [1, 2, 3, 4, 5];

export function RatingModal({ auctionId, targetName, targetRole, onClose, onSubmitted }: Props) {
  const { token, toast } = useApp();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [ratingFor, setRatingFor] = useState<RatingFor | null>(null);
  const [params, setParams] = useState<RatingParameter[]>([]);
  const [scores, setScores] = useState<Record<number, number>>({});
  const [remarks, setRemarks] = useState('');

  useEffect(() => {
    (async () => {
      try {
        const [fors, all] = await Promise.all([api.getRatingFors(token), api.getRatingParameters(token)]);
        const code = targetRole.toUpperCase();
        const match = (fors || []).find((f) => f.code?.toUpperCase() === code || f.name?.toUpperCase().includes(code));
        setRatingFor(match ?? null);
        setParams(match ? (all || []).filter((p) => !p.ratingForId || p.ratingForId === match.id) : []);
      } catch (err) {
        setError(errorMessage(err, 'Could not load rating questions.'));
      } finally {
        setLoading(false);
      }
    })();
  }, [token, targetRole]);

  const complete = params.length > 0 && params.every((p) => scores[p.id]);

  const submit = async () => {
    if (!ratingFor || !complete) return;
    setSaving(true);
    setError('');
    try {
      await api.submitRating(token, {
        auctionId,
        ratingForId: ratingFor.id,
        ratingValues: params.map((p) => ({ ratingParameterId: p.id, score: scores[p.id] })),
        remarks: remarks.trim() || undefined,
      });
      toast('Thanks — rating submitted.', 'success');
      onSubmitted();
    } catch (err) {
      setError(errorMessage(err, 'Could not submit the rating.'));
    } finally {
      setSaving(false);
    }
  };

  return (
    <Modal
      open
      title={`Rate ${targetName}`}
      onClose={onClose}
      footer={
        <>
          <Button variant="ghost" onClick={onClose}>Cancel</Button>
          <Button variant="primary" onClick={submit} loading={saving} disabled={!complete}>Submit rating</Button>
        </>
      }
    >
      {loading ? (
        <Loading />
      ) : (
        <>
          {error && <Callout tone="error">{error}</Callout>}
          {params.length === 0 && !error && <Callout>Rating questions are not set up yet.</Callout>}
          {params.map((p) => (
            <fieldset key={p.id} className="stack-sm" style={{ border: 0, padding: 0, margin: 0 }}>
              <legend className="field__label" style={{ marginBottom: 8 }}>{p.name}</legend>
              <div className="chip-select">
                {SCORES.map((s) => (
                  <button
                    key={s}
                    type="button"
                    aria-pressed={scores[p.id] === s}
                    aria-label={`${p.name}: ${s} out of 5`}
                    onClick={() => setScores((prev) => ({ ...prev, [p.id]: s }))}
                  >
                    {s}
                  </button>
                ))}
              </div>
            </fieldset>
          ))}
          <span className="small muted">1 = poor, 5 = excellent</span>
          <TextAreaField label="Comment (optional)" rows={3} value={remarks} onChange={(e) => setRemarks(e.target.value)} />
        </>
      )}
    </Modal>
  );
}
