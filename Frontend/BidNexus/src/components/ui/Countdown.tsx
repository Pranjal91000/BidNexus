import React, { useEffect, useState } from 'react';
import { Timer, Clock, AlertTriangle } from 'lucide-react';

export interface CountdownProps {
  endTime: string;
  startTime?: string;
  onExpire?: () => void;
  className?: string;
  size?: 'sm' | 'md' | 'lg';
  compact?: boolean;
  showIcon?: boolean;
}

export const Countdown: React.FC<CountdownProps> = ({
  endTime,
  startTime,
  onExpire,
  className = '',
  size = 'md',
  compact = false,
  showIcon = true,
}) => {
  const [timeLeft, setTimeLeft] = useState<{
    days: number;
    hours: number;
    minutes: number;
    seconds: number;
    isEnded: boolean;
    isUpcoming: boolean;
    totalSeconds: number;
  }>({ days: 0, hours: 0, minutes: 0, seconds: 0, isEnded: false, isUpcoming: false, totalSeconds: 0 });

  useEffect(() => {
    const calculateTime = () => {
      const now = new Date().getTime();
      const start = startTime ? new Date(startTime).getTime() : 0;
      const end = new Date(endTime).getTime();

      if (start && now < start) {
        const diff = Math.max(0, Math.floor((start - now) / 1000));
        const days = Math.floor(diff / 86400);
        const hours = Math.floor((diff % 86400) / 3600);
        const minutes = Math.floor((diff % 3600) / 60);
        const seconds = diff % 60;
        return { days, hours, minutes, seconds, isEnded: false, isUpcoming: true, totalSeconds: diff };
      }

      if (isNaN(end) || now >= end) {
        return { days: 0, hours: 0, minutes: 0, seconds: 0, isEnded: true, isUpcoming: false, totalSeconds: 0 };
      }

      const diff = Math.max(0, Math.floor((end - now) / 1000));
      const days = Math.floor(diff / 86400);
      const hours = Math.floor((diff % 86400) / 3600);
      const minutes = Math.floor((diff % 3600) / 60);
      const seconds = diff % 60;
      return { days, hours, minutes, seconds, isEnded: false, isUpcoming: false, totalSeconds: diff };
    };

    const initial = calculateTime();
    setTimeLeft(initial);
    if (initial.isEnded && onExpire) {
      onExpire();
    }

    const timer = setInterval(() => {
      const updated = calculateTime();
      setTimeLeft(updated);
      if (updated.isEnded) {
        clearInterval(timer);
        if (onExpire) onExpire();
      }
    }, 1000);

    return () => clearInterval(timer);
  }, [endTime, startTime, onExpire]);

  const pad = (n: number) => String(n).padStart(2, '0');
  const isUrgent = !timeLeft.isUpcoming && !timeLeft.isEnded && timeLeft.totalSeconds < 600; // < 10 mins

  // Compact Pill Renderer (for lists, cards, tables)
  if (compact || size === 'sm') {
    if (timeLeft.isEnded) {
      return (
        <span className={`bn-stopwatch-pill bn-stopwatch-pill-ended ${className}`}>
          {showIcon && <Clock size={12} />}
          <span>Ended</span>
        </span>
      );
    }

    const toneClass = timeLeft.isUpcoming
      ? 'bn-stopwatch-pill-upcoming'
      : isUrgent
      ? 'bn-stopwatch-pill-urgent'
      : 'bn-stopwatch-pill-live';

    return (
      <span className={`bn-stopwatch-pill ${toneClass} ${className}`} title={timeLeft.isUpcoming ? 'Starts in' : 'Time remaining until auction end'}>
        {showIcon && <Timer size={12} className={isUrgent ? 'bn-pulse-fast' : ''} />}
        <span>
          {timeLeft.days > 0 ? `${timeLeft.days}d ` : ''}
          {pad(timeLeft.hours)}:{pad(timeLeft.minutes)}:{pad(timeLeft.seconds)}
        </span>
      </span>
    );
  }

  // Full Hero Stopwatch HUD Renderer
  if (timeLeft.isEnded) {
    return (
      <div className={`bn-stopwatch-hud bn-stopwatch-ended bn-countdown-${size} ${className}`}>
        <div className="bn-stopwatch-icon-wrap">
          <Clock size={size === 'lg' ? 22 : 18} />
        </div>
        <div className="bn-stopwatch-content">
          <span className="bn-stopwatch-label">AUCTION STATUS</span>
          <span className="bn-stopwatch-digits">CONCLUDED</span>
        </div>
      </div>
    );
  }

  const statusClass = timeLeft.isUpcoming
    ? 'bn-stopwatch-upcoming'
    : isUrgent
    ? 'bn-stopwatch-urgent'
    : 'bn-stopwatch-live';

  return (
    <div
      className={`bn-stopwatch-hud ${statusClass} bn-countdown-${size} ${className}`}
      aria-label="Auction Live Stopwatch"
    >
      <div className="bn-stopwatch-icon-wrap">
        {isUrgent ? (
          <AlertTriangle size={size === 'lg' ? 22 : 18} className="bn-pulse-fast" />
        ) : (
          <Timer size={size === 'lg' ? 22 : 18} />
        )}
        <span className="bn-stopwatch-live-dot" />
      </div>

      <div className="bn-stopwatch-content">
        <span className="bn-stopwatch-label">
          {timeLeft.isUpcoming ? 'STARTS IN' : isUrgent ? 'CLOSING IMMINENT' : 'AUCTION STOPWATCH • CLOSING IN'}
        </span>
        <div className="bn-stopwatch-digits">
          {timeLeft.days > 0 && (
            <>
              <span className="bn-digit-block">{timeLeft.days}d</span>
              <span>:</span>
            </>
          )}
          <span className="bn-digit-block">{pad(timeLeft.hours)}h</span>
          <span>:</span>
          <span className="bn-digit-block">{pad(timeLeft.minutes)}m</span>
          <span>:</span>
          <span className="bn-digit-block bn-digit-sec">{pad(timeLeft.seconds)}s</span>
        </div>
      </div>
    </div>
  );
};
