using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages.Decorators
{
    public class ExtraShotDecorator : BeverageDecorator
    {
        public ExtraShotDecorator(IBeverage beverage) : base(beverage, "extra shot", 0.80m, "an extra shot of espresso")
        {
        }
    }
}
