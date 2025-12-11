using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages
{
    public class Espresso : IBeverage
    {
        private const decimal _price = 2.5m;
        private const string _description = "A strong and bold espresso shot.";
        public string Name => "espresso";

        public decimal Cost()
        {
            return _price;
        }

        public string Describe()
        {
            return _description;
        }
    }
}
