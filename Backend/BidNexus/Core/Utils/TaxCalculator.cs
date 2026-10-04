using Core.Entities.Auction;
using Core.Enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utils
{
    public class TaxCalculator
    {
        public void CalculateTax(ref BidDetail item)
        {
            var baseAmount = item.BaseAmount;

            var totalAdditiveTax = 0m;
            var totalDeductiveTax = 0m;
            var totalDiscount = 0m;


            foreach(var tax in item.Taxes)
            {

                if (tax.TaxNatureId == (short)TaxNatureEnum.Additive)
                {
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.Fixed)
                    {
                        totalAdditiveTax += tax.TaxValue;
                    }
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.Percentage)
                    {
                        totalAdditiveTax += baseAmount * tax.TaxValue / 100;
                    }
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.PerUnit)
                    {
                        totalAdditiveTax += tax.TaxValue * item.AuctionRequirement.Quantity;
                    }
                }
                else
                {
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.Fixed)
                    {
                        totalDeductiveTax += tax.TaxValue;
                    }
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.Percentage)
                    {
                        totalDeductiveTax += baseAmount * tax.TaxValue / 100;
                    }
                    if (tax.ChargeTypeId == (short)ChargeTypeEnum.PerUnit)
                    {
                        totalDeductiveTax += tax.TaxValue * item.AuctionRequirement.Quantity;
                    }
                }
            }

            item.NetAmount = baseAmount + totalAdditiveTax - totalDeductiveTax - totalDiscount;
        }
    }
}
