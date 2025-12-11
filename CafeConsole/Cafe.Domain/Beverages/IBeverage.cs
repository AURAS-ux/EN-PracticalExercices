using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Beverages
{
    public interface IBeverage
    {
        string Name { get; }
        decimal Cost();
        string Describe();
    }
}
