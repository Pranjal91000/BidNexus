import React from 'react';
import type { Requirement } from '../../types';
import { Layers } from 'lucide-react';

interface RequirementListTableProps {
  requirements?: Requirement[];
}

export const RequirementListTable: React.FC<RequirementListTableProps> = ({ requirements = [] }) => {
  if (requirements.length === 0) {
    return (
      <div className="bn-history-empty">
        <Layers size={28} className="bn-text-muted" />
        <p>No requirement line items attached to this auction scope.</p>
      </div>
    );
  }

  return (
    <div className="bn-requirements-container">
      <div className="bn-table-header-kicker">
        <span className="bn-eyebrow">AUCTION SCOPE & SPECIFICATIONS</span>
        <h4 className="bn-table-title">Line Item Requirements ({requirements.length})</h4>
      </div>

      <div className="bn-table-responsive">
        <table className="bn-table">
          <thead>
            <tr>
              <th style={{ width: '60px' }}>Line</th>
              <th>Material / Item Description</th>
              <th>Technical Specification</th>
              <th>Quantity</th>
              <th>Unit</th>
            </tr>
          </thead>
          <tbody>
            {requirements.map((req) => {
              return (
                <tr key={req.id || req.lineNo}>
                  <td>
                    <strong className="bn-req-line-no">#{req.lineNo}</strong>
                  </td>
                  <td>
                    <strong>{req.item?.itemName || req.item?.name || `Item #${req.itemId || req.id}`}</strong>
                  </td>
                  <td>
                    <span className="bn-text-muted bn-text-sm">
                      {req.technicalSpecification || 'Standard technical specification applicable.'}
                    </span>
                  </td>
                  <td>
                    <span className="bn-font-mono bn-font-bold">{req.quantity}</span>
                  </td>
                  <td>
                    <span className="bn-unit-chip">
                      {req.unit?.alias || req.unit?.name || req.unit?.unitName || `Unit #${req.unitId || ''}`}
                    </span>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};
