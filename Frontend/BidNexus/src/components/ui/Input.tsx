import React from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  hint?: string;
  leftIcon?: React.ReactNode;
}

export const Input: React.FC<InputProps> = ({
  label,
  error,
  hint,
  leftIcon,
  className = '',
  id,
  ...props
}) => {
  const inputId = id || (label ? `input-${label.toLowerCase().replace(/\s+/g, '-')}` : undefined);

  return (
    <div className={`bn-field ${error ? 'bn-field-error' : ''}`}>
      {label && (
        <label htmlFor={inputId} className="bn-label">
          {label} {props.required && <span className="bn-required">*</span>}
        </label>
      )}
      <div className="bn-input-wrapper">
        {leftIcon && <span className="bn-input-icon">{leftIcon}</span>}
        <input
          id={inputId}
          className={`bn-input ${leftIcon ? 'has-icon' : ''} ${className}`}
          {...props}
        />
      </div>
      {error && <p className="bn-error-msg">{error}</p>}
      {!error && hint && <p className="bn-hint-msg">{hint}</p>}
    </div>
  );
};

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  hint?: string;
  options: { value: string | number; label: string }[];
}

export const Select: React.FC<SelectProps> = ({
  label,
  error,
  hint,
  options,
  className = '',
  id,
  ...props
}) => {
  const selectId = id || (label ? `select-${label.toLowerCase().replace(/\s+/g, '-')}` : undefined);

  return (
    <div className={`bn-field ${error ? 'bn-field-error' : ''}`}>
      {label && (
        <label htmlFor={selectId} className="bn-label">
          {label} {props.required && <span className="bn-required">*</span>}
        </label>
      )}
      <select id={selectId} className={`bn-select ${className}`} {...props}>
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <p className="bn-error-msg">{error}</p>}
      {!error && hint && <p className="bn-hint-msg">{hint}</p>}
    </div>
  );
};

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  hint?: string;
}

export const Textarea: React.FC<TextareaProps> = ({
  label,
  error,
  hint,
  className = '',
  id,
  ...props
}) => {
  const textareaId = id || (label ? `textarea-${label.toLowerCase().replace(/\s+/g, '-')}` : undefined);

  return (
    <div className={`bn-field ${error ? 'bn-field-error' : ''}`}>
      {label && (
        <label htmlFor={textareaId} className="bn-label">
          {label} {props.required && <span className="bn-required">*</span>}
        </label>
      )}
      <textarea id={textareaId} className={`bn-textarea ${className}`} {...props} />
      {error && <p className="bn-error-msg">{error}</p>}
      {!error && hint && <p className="bn-hint-msg">{hint}</p>}
    </div>
  );
};
