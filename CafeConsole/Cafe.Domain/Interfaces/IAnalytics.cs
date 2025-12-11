using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Interfaces
{
    public interface IAnalytics
    {
        int TotalOrders { get; }
        decimal TotalRevenue { get; }
    }
}
