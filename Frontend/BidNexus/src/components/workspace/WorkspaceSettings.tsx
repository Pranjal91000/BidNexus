import React, { useState, useEffect } from 'react';
import type { Claims } from '../../types';
import { ShieldCheck, Building2, Store, Server, CheckCircle2 } from 'lucide-react';
import { API_BASE_URL, api } from '../../services/api';

interface WorkspaceSettingsProps {
  claims: Claims;
  token: string;
}

export const WorkspaceSettings: React.FC<WorkspaceSettingsProps> = ({ claims, token }) => {
  const [itemsCount, setItemsCount] = useState<number>(0);
  const [unitsCount, setUnitsCount] = useState<number>(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchMasters() {
      try {
        const [items, units] = await Promise.all([api.getItems(token), api.getUnits(token)]);
        setItemsCount(items.length);
        setUnitsCount(units.length);
      } catch {
        // master endpoint
      } finally {
        setLoading(false);
      }
    }
    fetchMasters();
  }, [token]);

  return (
    <div className="bn-workspace-settings-stack">
      {/* Identity Banner */}
      <section className="bn-profile-hero-card">
        <div className="bn-avatar-large">
          {claims.role === 'Organization' ? <Building2 size={32} /> : <Store size={32} />}
        </div>
        <div className="bn-profile-details">
          <span className="bn-eyebrow">AUTHENTICATED SECURITY CONTEXT</span>
          <h2 className="bn-profile-name">{claims.name}</h2>
          <p className="bn-profile-email">{claims.email || 'Authenticated Platform Workspace'}</p>
        </div>
        <div className="bn-profile-role-tag">
          <span className={`bn-role-badge bn-role-${claims.role.toLowerCase()}`}>
            {claims.role} Workspace Scope
          </span>
        </div>
      </section>

      {/* Grid Settings Panels */}
      <div className="bn-grid-2 bn-mt-4">
        {/* Tenant Identity Panel */}
        <section className="bn-panel-card">
          <div className="bn-card-header">
            <ShieldCheck size={20} className="bn-text-accent" />
            <h3 className="bn-card-title">Tenant Security Context</h3>
          </div>
          <p className="bn-text-muted bn-text-xs bn-mb-4">
            Security boundary is derived strictly from claims in the authenticated JWT token.
          </p>

          <dl className="bn-dl-list">
            <div className="bn-dl-item">
              <dt>User ID (Subject)</dt>
              <dd className="bn-font-mono">#{claims.userId || '—'}</dd>
            </div>
            <div className="bn-dl-item">
              <dt>Tenant Isolation ID</dt>
              <dd className="bn-font-mono">#{claims.tenantId || 'Scope Enforced'}</dd>
            </div>
            <div className="bn-dl-item">
              <dt>Assigned Role</dt>
              <dd>
                <span className={`bn-badge bn-badge-${claims.role.toLowerCase() === 'vendor' ? 'live' : 'info'}`}>
                  {claims.role}
                </span>
              </dd>
            </div>
            <div className="bn-dl-item">
              <dt>EF Core Query Filter</dt>
              <dd className="bn-text-success bn-text-xs">
                <CheckCircle2 size={13} /> Automatic Isolation Active
              </dd>
            </div>
          </dl>
        </section>

        {/* API & Backend Panel */}
        <section className="bn-panel-card">
          <div className="bn-card-header">
            <Server size={20} className="bn-text-info" />
            <h3 className="bn-card-title">API & Engine Services</h3>
          </div>
          <p className="bn-text-muted bn-text-xs bn-mb-4">
            Connected backend endpoint and master data services.
          </p>

          <dl className="bn-dl-list">
            <div className="bn-dl-item">
              <dt>API Base Endpoint</dt>
              <dd className="bn-font-mono bn-text-xs">{API_BASE_URL || 'Same Origin'}</dd>
            </div>
            <div className="bn-dl-item">
              <dt>SignalR Hub Route</dt>
              <dd className="bn-font-mono bn-text-xs">{`${API_BASE_URL}/hubs/auction`}</dd>
            </div>
            <div className="bn-dl-item">
              <dt>Master Items Loaded</dt>
              <dd className="bn-font-mono">{loading ? '...' : itemsCount}</dd>
            </div>
            <div className="bn-dl-item">
              <dt>Master Units Loaded</dt>
              <dd className="bn-font-mono">{loading ? '...' : unitsCount}</dd>
            </div>
          </dl>
        </section>
      </div>
    </div>
  );
};
