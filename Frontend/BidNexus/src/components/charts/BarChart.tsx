import { useWidth } from '../../lib/hooks';

export interface BarDatum {
  label: string;
  values: number[];
}

interface BarChartProps {
  data: BarDatum[];
  series: { name: string; variant?: 'primary' | 'secondary' }[];
  format: (n: number) => string;
  height?: number;
  /** Text for screen readers describing what the chart shows. */
  summary: string;
}

const PAD = { top: 12, right: 8, bottom: 28, left: 64 };

function niceMax(v: number) {
  if (v <= 0) return 1;
  const exp = Math.pow(10, Math.floor(Math.log10(v)));
  const f = v / exp;
  const nice = f <= 1 ? 1 : f <= 2 ? 2 : f <= 2.5 ? 2.5 : f <= 5 ? 5 : 10;
  return nice * exp;
}

export function BarChart({ data, series, format, height = 220, summary }: BarChartProps) {
  const [ref, width] = useWidth<HTMLDivElement>();
  const max = niceMax(Math.max(0, ...data.flatMap((d) => d.values)));
  const innerW = Math.max(0, width - PAD.left - PAD.right);
  const innerH = height - PAD.top - PAD.bottom;
  const groupW = data.length ? innerW / data.length : 0;
  const barGap = 4;
  const barW = Math.max(4, Math.min(36, (groupW * 0.6 - barGap * (series.length - 1)) / series.length));
  const ticks = [0, 0.25, 0.5, 0.75, 1].map((t) => t * max);
  const y = (v: number) => PAD.top + innerH - (v / max) * innerH;

  return (
    <div ref={ref} className="stack-sm">
      {series.length > 1 && (
        <div className="legend" aria-hidden="true">
          {series.map((s) => (
            <span key={s.name} className="legend__item">
              <span className={`legend__swatch${s.variant === 'secondary' ? ' legend__swatch--secondary' : ''}`} />
              {s.name}
            </span>
          ))}
        </div>
      )}
      {width > 0 && (
        <svg className="chart" width={width} height={height} role="img" aria-label={summary}>
          {ticks.map((t) => (
            <g key={t}>
              <line className="chart__grid" x1={PAD.left} x2={width - PAD.right} y1={y(t)} y2={y(t)} />
              <text x={PAD.left - 8} y={y(t)} textAnchor="end" dominantBaseline="middle">
                {format(t)}
              </text>
            </g>
          ))}
          {data.map((d, i) => {
            const groupX = PAD.left + i * groupW;
            const totalBarsW = series.length * barW + (series.length - 1) * barGap;
            const startX = groupX + (groupW - totalBarsW) / 2;
            return (
              <g key={d.label}>
                {d.values.map((v, j) => {
                  const h = Math.max(v > 0 ? 2 : 0, (v / max) * innerH);
                  return (
                    <rect
                      key={j}
                      className={`chart__bar${series[j]?.variant === 'secondary' ? ' chart__bar--secondary' : ''}`}
                      x={startX + j * (barW + barGap)}
                      y={PAD.top + innerH - h}
                      width={barW}
                      height={h}
                      rx={3}
                    >
                      <title>{`${d.label} · ${series[j]?.name ?? ''}: ${format(v)}`}</title>
                    </rect>
                  );
                })}
                <text x={groupX + groupW / 2} y={height - 8} textAnchor="middle">
                  {d.label}
                </text>
              </g>
            );
          })}
        </svg>
      )}
    </div>
  );
}
