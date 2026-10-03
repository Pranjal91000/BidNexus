import React, { useEffect, useState } from 'react';
import { Clock } from 'lucide-react';

interface CountdownProps {
  endTime: string;
  startTime?: string;
  onExpire?: () => void;
  className?: string;
}

export const Countdown: React.FC<CountdownProps> = ({ endTime, startTime, onExpire, className = '' }) => {
  const [timeLeft, setTimeLeft] = useState<{
    hours: number;
    minutes: number;
    seconds: number;
    isEnded: boolean;
    isUpcoming: boolean;
    totalSeconds: number;
  }>({ hours: 0, minutes: 0, seconds: 0, isEnded: false, isUpcoming: false, totalSeconds: 0 });

  useEffect(() => {
    const calculateTime = () => {
      const now = new Date().getTime();
      const start = startTime ? new Date(startTime).getTime() : 0;
      const end = new Date(endTime).getTime();

      if (start && now < start) {
        const diff = Math.max(0, Math.floor((start - now) / 1000));
        const hours = Math.floor(diff / 3600);
        const minutes = Math.floor((diff % 3600) / 60);
        const seconds = diff % 60;
        return { hours, minutes, seconds, isEnded: false, isUpcoming: true, totalSeconds: diff };
      }

      if (isNaN(end) || now >= end) {
        return { hours: 0, minutes: 0, seconds: 0, isEnded: true, isUpcoming: false, totalSeconds: 0 };
      }

      const diff = Math.max(0, Math.floor((end - now) / 1000));
      const hours = Math.floor(diff / 3600);
      const minutes = Math.floor((diff % 3600) / 60);
      const seconds = diff % 60;
      return { hours, minutes, seconds, isEnded: false, isUpcoming: false, totalSeconds: diff };
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

  if (timeLeft.isEnded) {
    return (
      <div className={`bn-countdown bn-countdown-ended ${className}`}>
        <Clock size={16} />
        <span>AUCTION CLOSED</span>
      </div>
    );
  }

  const isUrgent = !timeLeft.isUpcoming && timeLeft.totalSeconds < 600; // < 10 mins

  const pad = (n: number) => String(n).padStart(2, '0');

  return (
    <div
      className={`bn-countdown ${timeLeft.isUpcoming ? 'bn-countdown-upcoming' : isUrgent ? 'bn-countdown-urgent' : 'bn-countdown-live'} ${className}`}
    >
      <Clock size={16} className={isUrgent ? 'bn-pulse-slow' : ''} />
      <span className="bn-countdown-label">
        {timeLeft.isUpcoming ? 'STARTS IN' : 'TIME REMAINING'}
      </span>
      <span className="bn-countdown-timer">
        {pad(timeLeft.hours)}:{pad(timeLeft.minutes)}:{pad(timeLeft.seconds)}
      </span>
    </div>
  );
};
