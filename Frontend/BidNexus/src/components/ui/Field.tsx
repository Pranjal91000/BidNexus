import { useId, type InputHTMLAttributes, type ReactNode, type SelectHTMLAttributes, type TextareaHTMLAttributes } from 'react';

interface FieldShellProps {
  label: string;
  hint?: ReactNode;
  error?: string;
  className?: string;
  children: (id: string, describedBy?: string) => ReactNode;
}

function FieldShell({ label, hint, error, className = '', children }: FieldShellProps) {
  const id = useId();
  const describedBy = error ? `${id}-err` : hint ? `${id}-hint` : undefined;
  return (
    <div className={`field ${className}`}>
      <label className="field__label" htmlFor={id}>{label}</label>
      {children(id, describedBy)}
      {error ? (
        <span id={`${id}-err`} className="field__error">{error}</span>
      ) : hint ? (
        <span id={`${id}-hint`} className="field__hint">{hint}</span>
      ) : null}
    </div>
  );
}

type Common = { label: string; hint?: ReactNode; error?: string; className?: string };

export function TextField({ label, hint, error, className, ...rest }: Common & InputHTMLAttributes<HTMLInputElement>) {
  return (
    <FieldShell label={label} hint={hint} error={error} className={className}>
      {(id, describedBy) => <input id={id} className="input" aria-invalid={!!error} aria-describedby={describedBy} {...rest} />}
    </FieldShell>
  );
}

export function SelectField({
  label,
  hint,
  error,
  className,
  options,
  ...rest
}: Common & SelectHTMLAttributes<HTMLSelectElement> & { options: { value: string | number; label: string }[] }) {
  return (
    <FieldShell label={label} hint={hint} error={error} className={className}>
      {(id, describedBy) => (
        <select id={id} className="select" aria-invalid={!!error} aria-describedby={describedBy} {...rest}>
          {options.map((o) => (
            <option key={o.value} value={o.value}>{o.label}</option>
          ))}
        </select>
      )}
    </FieldShell>
  );
}

export function TextAreaField({ label, hint, error, className, ...rest }: Common & TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return (
    <FieldShell label={label} hint={hint} error={error} className={className}>
      {(id, describedBy) => <textarea id={id} className="textarea" aria-invalid={!!error} aria-describedby={describedBy} {...rest} />}
    </FieldShell>
  );
}

export function Checkbox({ label, hint, checked, onChange }: { label: string; hint?: string; checked: boolean; onChange: (v: boolean) => void }) {
  return (
    <label className="check">
      <input type="checkbox" checked={checked} onChange={(e) => onChange(e.target.checked)} />
      <span className="stack-sm" style={{ gap: 0 }}>
        <span>{label}</span>
        {hint && <span className="small muted">{hint}</span>}
      </span>
    </label>
  );
}
