using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages
{
    public class Tea : IBeverage
    {
        private const decimal _cost = 2.00m;
        private const string _description = "A soothing cup of tea.";
        public string Name => "tea";

        public decimal Cost()
        {
            return _cost;
        }

        public string Describe()
        {
            return _description;
        }
    }
}
