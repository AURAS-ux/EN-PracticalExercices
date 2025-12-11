using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages.Decorators
{
    public class BeverageDecorator : IBeverage
    {
        protected IBeverage _beverage;
        private string _name;
        private decimal _cost;
        private string _description;
        public BeverageDecorator(IBeverage beverage, string name, decimal cost, string description)
        {
            _beverage = beverage;
            _name = name;
            _cost = cost;
            _description = description;
        }
        public string Name => $"{_beverage.Name} with added {_name}";

        public decimal Cost()
        {
            return _beverage.Cost() + _cost;
        }

        public string Describe()
        {
            return $"{_beverage.Describe()} Added {_description}.";
        }
    }
}
