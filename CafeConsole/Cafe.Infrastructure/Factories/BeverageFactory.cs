using Cafe.Domain.Beverages;
using Cafe.Domain.Beverages.Decorators;
using Cafe.Domain.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Infrastructure.Factories
{
    public class BeverageFactory : IBeverageFactory
    {
        public IBeverage CreateBeverage(string beverageType)
        {
            return beverageType.ToLower() switch
            {
                "espresso" => new Espresso(),
                "tea" => new Tea(),
                "hotchocolate" => new HotChocolate(),
                _ => throw new ArgumentException($"Beverage type '{beverageType}' is not recognized.")
            };
        }

        public IBeverage Customize(string addOn, IBeverage baseBeverage)
        {
            return addOn.ToLower() switch
                    {
                        "milk" => new MilkDecorator(baseBeverage),
                        "syrup vanilla" => new SyrupDecorator(baseBeverage, "vanilla"),
                        "syrup caramel" => new SyrupDecorator(baseBeverage, "caramel"),
                        "syrup hazelnut" => new SyrupDecorator(baseBeverage, "hazelnut"),
                        "extrashot" => new ExtraShotDecorator(baseBeverage),

                        _ => baseBeverage
                    };
        }
    }
}
