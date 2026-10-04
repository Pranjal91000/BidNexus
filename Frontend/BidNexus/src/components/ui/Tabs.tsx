export interface TabItem<T extends string> {
  id: T;
  label: string;
  count?: number;
  icon?: any;
}

export function Tabs<T extends string>({
  tabs,
  active,
  activeTab,
  onChange,
  label = 'Navigation',
}: {
  tabs: TabItem<T>[];
  active?: T;
  activeTab?: T;
  onChange: (id: T) => void;
  label?: string;
}) {
  const current = active ?? activeTab;
  return (
    <div className="tabs" role="tablist" aria-label={label}>
      {tabs.map((t) => {
        const isSelected = t.id === current;
        return (
          <button
            key={t.id}
            type="button"
            role="tab"
            aria-selected={isSelected}
            className={`tab${isSelected ? ' tab--active' : ''}`}
            onClick={() => onChange(t.id)}
          >
            {t.icon && <span style={{ display: 'inline-flex', marginRight: 6 }}>{t.icon}</span>}
            {t.label}
            {t.count != null && <span className="tab__count">{t.count}</span>}
          </button>
        );
      })}
    </div>
  );
}
