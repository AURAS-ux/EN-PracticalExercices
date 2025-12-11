using Cafe.Domain.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Extentions
{
    public static class PricingStrategyString
    {
        public static string GetName(this IPricingStrategy pricingStrategy)
        {
            switch (pricingStrategy.GetType().Name)
            {
                case "RegularPricing":
                    return "Regular Pricing";
                case "HappyHourPricing":
                    return "Happy Hour (-20%)";
                default:
                    throw new ArgumentOutOfRangeException($"Unknown pricing strategy: {pricingStrategy.GetType().Name}");
            }
        }
    }
}
