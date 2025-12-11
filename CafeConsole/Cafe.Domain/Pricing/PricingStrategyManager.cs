using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Pricing
{
    public class PricingStrategyManager : IPricingStrategyManager
    {
        private readonly RegularPricing _regularPricingStrategy;
        private readonly HappyHourPricing _happyHourPricingStrategy;

        public PricingStrategyManager(RegularPricing regularPricingStrategy, HappyHourPricing happyHourPricingStrategy)
        {
            _regularPricingStrategy = regularPricingStrategy;
            _happyHourPricingStrategy = happyHourPricingStrategy;
        }

        public IPricingStrategy GetStrategy(PricingStrategy strategy)
        {
            switch (strategy)
            {
                case PricingStrategy.Regular:
                    return _regularPricingStrategy;
                case PricingStrategy.HappyHour:
                    return _happyHourPricingStrategy;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }
    }
}
