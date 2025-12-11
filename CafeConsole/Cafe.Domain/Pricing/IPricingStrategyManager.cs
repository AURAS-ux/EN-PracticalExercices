using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Pricing
{
    public interface IPricingStrategyManager
    {
        IPricingStrategy GetStrategy(PricingStrategy strategy);
    }
}
