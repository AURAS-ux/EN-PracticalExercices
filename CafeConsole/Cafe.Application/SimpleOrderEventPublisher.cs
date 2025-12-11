using Cafe.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Application
{
    public class SimpleOrderEventPublisher : IOrderEventPublisher
    {
        private IEnumerable<IOrderEventSubscriber> _subscribers;

        public SimpleOrderEventPublisher(IEnumerable<IOrderEventSubscriber> subscribers)
        {
            _subscribers = subscribers;
        }

        public void Published(OrderPlaced orderPlaced)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.On(orderPlaced);
            }
        }
    }
}
