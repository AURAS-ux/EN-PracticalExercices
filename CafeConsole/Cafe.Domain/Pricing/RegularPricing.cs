using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Pricing
{
    public class RegularPricing : IPricingStrategy
    {
        public decimal Apply(decimal subTotal)
        {
            return subTotal;
        }
    }
}
