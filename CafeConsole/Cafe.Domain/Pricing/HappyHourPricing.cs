using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Pricing
{
    public class HappyHourPricing : IPricingStrategy
    {
        private const decimal discount = 0.8m;
        public decimal Apply(decimal subTotal)
        {
            return subTotal * discount;
        }
    }
}
