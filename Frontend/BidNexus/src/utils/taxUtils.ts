import type { BidTaxDetailSaveRequest } from '../types';
import { ChargeTypeEnum, TaxNatureEnum } from '../types';

export { ChargeTypeEnum, TaxNatureEnum };


/**
 * Detects whether a tax item represents "Per Unit"
 */
export function isPerUnitCharge(tax: { chargeTypeId?: number }): boolean {
  return tax.chargeTypeId === ChargeTypeEnum.PerUnit;
}

/**
 * Detects whether a tax item represents "Fixed"
 */
export function isFixedCharge(tax: { chargeTypeId?: number }): boolean {
  return tax.chargeTypeId === ChargeTypeEnum.Fixed;
}

/**
 * Detects whether a tax item represents "Percentage"
 */
export function isPercentageCharge(tax: { chargeTypeId?: number }): boolean {
  return tax.chargeTypeId === ChargeTypeEnum.Percentage;
}

/**
 * Returns human-friendly rate/value label (e.g. "₹5/unit", "₹100 fixed", or "18%")
 */
export function getTaxDisplayRate(tax: { chargeTypeId?: number; taxValue: number }): string {
  const val = Number(tax.taxValue) || 0;
  if (tax.chargeTypeId === ChargeTypeEnum.PerUnit) {
    return `₹${val}/unit`;
  }
  if (tax.chargeTypeId === ChargeTypeEnum.Fixed) {
    return `₹${val} fixed`;
  }
  return `${val}%`;
}

export interface TaxInput {
  id?: number | null;
  name?: string;
  code?: string;
  taxNatureId?: number;
  chargeTypeId?: number;
  taxValue: number;
}

/**
 * Computes single tax amount given charge type and tax nature strictly using enums.
 * ChargeTypes: Fixed = 1, Percentage = 2, PerUnit = 3.
 * TaxNatures: Additive = 1, Deductive = 2.
 */
export function calculateTaxAmount(
  tax: { chargeTypeId?: number; taxNatureId?: number; taxValue: number },
  baseAmount: number,
  quantity: number
): { amount: number; isDeductive: boolean } {
  const val = Number(tax.taxValue) || 0;
  let amt = 0;

  switch (tax.chargeTypeId) {
    case ChargeTypeEnum.PerUnit:
      amt = quantity * val;
      break;
    case ChargeTypeEnum.Fixed:
      amt = val;
      break;
    case ChargeTypeEnum.Percentage:
    default:
      amt = (baseAmount * val) / 100;
      break;
  }

  const isDeductive = tax.taxNatureId === TaxNatureEnum.Deductive;
  return { amount: Math.round(amt * 100) / 100, isDeductive };
}

/**
 * Converts selected taxes (both Master taxes and ad-hoc custom taxes) into BidTaxDetailSaveRequest models for submission.
 */
export function buildBidTaxDetails(
  taxes: TaxInput[],
  baseAmount: number,
  quantity: number
): { taxes: BidTaxDetailSaveRequest[]; totalTaxAmount: number } {
  let netTaxAmount = 0;
  const taxDetails: BidTaxDetailSaveRequest[] = [];

  for (const tax of taxes) {
    const { amount, isDeductive } = calculateTaxAmount(tax, baseAmount, quantity);
    if (isDeductive) {
      netTaxAmount -= amount;
    } else {
      netTaxAmount += amount;
    }

    taxDetails.push({
      taxId: tax.id ?? null,
      taxName: tax.name || (tax.id ? `Tax #${tax.id}` : 'Custom Tax'),
      taxCode: tax.code || '',
      taxNatureId: tax.taxNatureId || TaxNatureEnum.Additive,
      chargeTypeId: tax.chargeTypeId || ChargeTypeEnum.Percentage,
      taxValue: Number(tax.taxValue) || 0,
      taxAmount: amount,
    });
  }

  return { taxes: taxDetails, totalTaxAmount: Math.round(netTaxAmount * 100) / 100 };
}
