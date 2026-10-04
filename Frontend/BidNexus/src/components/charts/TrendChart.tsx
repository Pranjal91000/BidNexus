import { useWidth } from '../../lib/hooks';
import { time } from '../../lib/format';

export interface TrendPoint {
  t: number;
  v: number;
}

export interface TrendSeries {
  name: string;
  points: TrendPoint[];
  variant?: 'market' | 'mine';
}

interface TrendChartProps {
  series: TrendSeries[];
  start: number;
  end: number;
  format: (n: number) => string;
  height?: number;
  summary: string;
}

const PAD = { top: 12, right: 16, bottom: 28, left: 72 };

/** A step line: the value holds until the next bid changes it. */
export function TrendChart({ series, start, end, format, height = 220, summary }: TrendChartProps) {
  const [ref, width] = useWidth<HTMLDivElement>();
  const all = series.flatMap((s) => s.points.map((p) => p.v));
  const drawable = series.filter((s) => s.points.length > 0);

  const lo = all.length ? Math.min(...all) : 0;
  const hi = all.length ? Math.max(...all) : 1;
  const span = hi - lo || hi * 0.05 || 1;
  const yMin = lo - span * 0.12;
  const yMax = hi + span * 0.12;

  const t0 = Math.min(start, ...series.flatMap((s) => s.points.map((p) => p.t)));
  const t1 = Math.max(end, t0 + 1, ...series.flatMap((s) => s.points.map((p) => p.t)));
  const innerW = Math.max(0, width - PAD.left - PAD.right);
  const innerH = height - PAD.top - PAD.bottom;
  const x = (t: number) => PAD.left + ((t - t0) / (t1 - t0)) * innerW;
  const y = (v: number) => PAD.top + innerH - ((v - yMin) / (yMax - yMin)) * innerH;

  const path = (pts: TrendPoint[]) => {
    if (!pts.length) return '';
    let d = `M ${x(pts[0].t)} ${y(pts[0].v)}`;
    for (let i = 1; i < pts.length; i++) d += ` H ${x(pts[i].t)} V ${y(pts[i].v)}`;
    return d + ` H ${x(t1)}`;
  };

  const yTicks = [0, 1 / 3, 2 / 3, 1].map((f) => yMin + f * (yMax - yMin));
  const xTicks = (width < 480 ? [0, 1] : [0, 1 / 3, 2 / 3, 1]).map((f) => t0 + f * (t1 - t0));

  return (
    <div ref={ref} className="stack-sm">
      {drawable.length > 1 && (
        <div className="legend" aria-hidden="true">
          {drawable.map((s) => (
            <span key={s.name} className="legend__item">
              <span className={`legend__swatch${s.variant === 'mine' ? ' legend__swatch--mine' : ''}`} />
              {s.name}
            </span>
          ))}
        </div>
      )}
      {width > 0 && (
        <svg className="chart" width={width} height={height} role="img" aria-label={summary}>
          {yTicks.map((v, i) => (
            <g key={i}>
              <line className="chart__grid" x1={PAD.left} x2={width - PAD.right} y1={y(v)} y2={y(v)} />
              <text x={PAD.left - 8} y={y(v)} textAnchor="end" dominantBaseline="middle">
                {format(v)}
              </text>
            </g>
          ))}
          {xTicks.map((t, i) => (
            <text key={i} x={x(t)} y={height - 8} textAnchor={i === 0 ? 'start' : i === xTicks.length - 1 ? 'end' : 'middle'}>
              {time(new Date(t).toISOString())}
            </text>
          ))}
          {drawable.map((s) => (
            <g key={s.name}>
              <path className={`chart__line${s.variant === 'mine' ? ' chart__line--mine' : ''}`} d={path(s.points)} />
              {s.points.map((p, i) => (
                <circle
                  key={i}
                  cx={x(p.t)}
                  cy={y(p.v)}
                  r={2.5}
                  fill={s.variant === 'mine' ? 'var(--warn)' : 'var(--accent)'}
                >
                  <title>{`${s.name} · ${time(new Date(p.t).toISOString())}: ${format(p.v)}`}</title>
                </circle>
              ))}
            </g>
          ))}
        </svg>
      )}
    </div>
  );
}
