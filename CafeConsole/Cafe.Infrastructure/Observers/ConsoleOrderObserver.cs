using Cafe.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Infrastructure.Observers
{
    public sealed class ConsoleOrderObserver : IOrderEventSubscriber
    {
        public void On(OrderPlaced orderPlaced)
        {
            Console.WriteLine($"[{orderPlaced.At:HH:mm:ss}] Order {orderPlaced.OrderId} subtotal {orderPlaced.Subtotal:C} total {orderPlaced.Total:C}");
        }
    }
}
