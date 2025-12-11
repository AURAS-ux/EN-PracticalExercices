using Cafe.Domain.Events;
using Cafe.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Infrastructure.Observers
{
    public class InMemoryOrderAnalytics : IOrderEventSubscriber,IAnalytics
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public void On(OrderPlaced orderPlaced)
        {
            TotalOrders++;
            TotalRevenue += orderPlaced.Total;
        }
    }
}
