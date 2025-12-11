using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages
{
    public class HotChocolate : IBeverage
    {
        private const decimal _price = 3.00m;
        private const string _description = "A warm and comforting hot chocolate.";
        public string Name => "hotchocolate";

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
