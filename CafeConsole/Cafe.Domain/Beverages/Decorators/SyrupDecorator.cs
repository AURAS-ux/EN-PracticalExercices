using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages.Decorators
{
    public class SyrupDecorator : BeverageDecorator
    {
        private string _flavor;
        public SyrupDecorator(IBeverage beverage,string flavour) : base(beverage, "syrup", 0.50m, $"a sweep drop of syrup with added {flavour}")
        {
        }
    }
}
