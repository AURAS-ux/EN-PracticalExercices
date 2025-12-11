using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Events
{
    public interface IOrderEventSubscriber
    {
        void On(OrderPlaced orderPlaced);
    }
}
