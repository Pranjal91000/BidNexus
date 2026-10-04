import React, { useState, useEffect } from 'react';
import { Star, CheckCircle2, AlertCircle } from 'lucide-react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Textarea } from '../ui/Input';
import { api } from '../../services/api';
import type { RatingFor, RatingParameter, RatingValueSubmit } from '../../types';

interface RatingModalProps {
  isOpen: boolean;
  onClose: () => void;
  token: string;
  auctionId: number;
  auctionName?: string;
  targetName: string;
  targetRole: 'Vendor' | 'Organization';
  onRatingSubmitted?: () => void;
  onShowToast?: (msg: string, tone?: 'success' | 'error' | 'info') => void;
}

export const RatingModal: React.FC<RatingModalProps> = ({
  isOpen,
  onClose,
  token,
  auctionId,
  auctionName,
  targetName,
  targetRole,
  onRatingSubmitted,
  onShowToast,
}) => {
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [ratingFor, setRatingFor] = useState<RatingFor | null>(null);
  const [parameters, setParameters] = useState<RatingParameter[]>([]);
  const [scores, setScores] = useState<Record<number, number>>({});
  const [remarks, setRemarks] = useState('');

  useEffect(() => {
    if (isOpen) {
      loadRatingMeta();
    }
  }, [isOpen, targetRole]);

  const loadRatingMeta = async () => {
    setLoading(true);
    setError('');
    try {
      const [fors, allParams] = await Promise.all([
        api.getRatingFors(token).catch(() => []),
        api.getRatingParameters(token).catch(() => []),
      ]);

      // Match target role (VENDOR vs ORGANIZATION)
      const targetCode = targetRole.toUpperCase();
      const matchedFor = fors.find(
        (f) => f.code.toUpperCase() === targetCode || f.name.toUpperCase().includes(targetCode)
      );

      if (matchedFor) {
        setRatingFor(matchedFor);
        const filteredParams = allParams.filter(
          (p) => !p.ratingForId || p.ratingForId === matchedFor.id
        );
        setParameters(filteredParams);

        // Default all scores to 5
        const initialScores: Record<number, number> = {};
        filteredParams.forEach((p) => {
          initialScores[p.id] = 5;
        });
        setScores(initialScores);
      } else {
        setParameters([]);
      }
    } catch (err: any) {
      setError(err instanceof Error ? err.message : 'Failed to load rating parameters.');
    } finally {
      setLoading(false);
    }
  };

  const handleScoreChange = (paramId: number, score: number) => {
    setScores((prev) => ({ ...prev, [paramId]: score }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!ratingFor) {
      setError('Rating category definition is missing.');
      return;
    }

    if (parameters.length === 0) {
      setError('No rating parameters available to submit.');
      return;
    }

    const ratingValues: RatingValueSubmit[] = parameters.map((p) => ({
      ratingParameterId: p.id,
      score: scores[p.id] || 5,
    }));

    setSubmitting(true);
    setError('');
    try {
      await api.submitRating(token, {
        auctionId,
        ratingForId: ratingFor.id,
        ratingValues,
        remarks: remarks.trim() || undefined,
      });

      onShowToast?.(`Rating for ${targetName} submitted successfully!`, 'success');
      onRatingSubmitted?.();
      onClose();
    } catch (err: any) {
      setError(err instanceof Error ? err.message : 'Failed to submit rating.');
    } finally {
      setSubmitting(false);
    }
  };

  const averageScore =
    parameters.length > 0
      ? (
          Object.values(scores).reduce((a, b) => a + b, 0) /
          parameters.length
        ).toFixed(1)
      : '5.0';

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={`Rate ${targetRole}: ${targetName}`}
      subtitle={`Submit verified performance feedback for Auction #${auctionId} (${auctionName || 'Procurement'})`}
      maxWidth="md"
      footer={
        <div className="bn-flex-end gap-2">
          <Button variant="outline" onClick={onClose} disabled={submitting}>
            Cancel
          </Button>
          <Button
            variant="primary"
            onClick={handleSubmit}
            loading={submitting}
            disabled={loading || parameters.length === 0}
            icon={<CheckCircle2 size={16} />}
          >
            Submit Authoritative Rating
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} className="bn-form-stack">
        {error && (
          <div className="bn-auth-error">
            <AlertCircle size={16} /> {error}
          </div>
        )}

        <div className="bn-rating-summary-card">
          <div className="bn-flex-between">
            <div>
              <span className="bn-eyebrow">TARGET ENTITY</span>
              <h4 className="bn-m-0">{targetName}</h4>
              <small className="bn-text-muted">{targetRole} • Verified Winner Participant</small>
            </div>
            <div className="bn-rating-overall-pill">
              <Star size={18} fill="#eab308" color="#eab308" />
              <strong>{averageScore}</strong>
              <small>/ 5.0</small>
            </div>
          </div>
        </div>

        {loading ? (
          <div className="bn-p-4 bn-text-center bn-text-muted">Loading rating parameters...</div>
        ) : parameters.length === 0 ? (
          <div className="bn-p-4 bn-text-center bn-text-muted">
            No rating parameters configured in GlobalData for {targetRole}.
          </div>
        ) : (
          <div className="bn-rating-param-list">
            <label className="bn-label bn-mb-2">Evaluation Metrics (1 = Poor, 5 = Excellent)</label>
            {parameters.map((param) => {
              const currentScore = scores[param.id] || 5;
              return (
                <div key={param.id} className="bn-rating-param-row">
                  <div className="bn-param-info">
                    <strong>{param.name}</strong>
                    <span className="bn-text-muted bn-text-xs"> ({param.code})</span>
                  </div>
                  <div className="bn-star-selector">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <button
                        key={star}
                        type="button"
                        className={`bn-star-btn ${star <= currentScore ? 'active' : ''}`}
                        onClick={() => handleScoreChange(param.id, star)}
                        title={`Score: ${star} out of 5`}
                      >
                        <Star
                          size={20}
                          fill={star <= currentScore ? '#eab308' : 'none'}
                          color={star <= currentScore ? '#eab308' : '#64748b'}
                        />
                      </button>
                    ))}
                    <span className="bn-score-tag">{currentScore}/5</span>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        <Textarea
          label="Performance Remarks & Feedback (Optional)"
          placeholder="Provide context regarding timeliness, compliance, material quality, or communication."
          rows={3}
          value={remarks}
          onChange={(e) => setRemarks(e.target.value)}
        />
      </form>
    </Modal>
  );
};
