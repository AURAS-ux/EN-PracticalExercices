using Cafe.Domain.Events;
using Cafe.Domain.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Extentions
{
    public static class ReceiptToOrderPlaced
    {
        public static OrderPlaced ToOrderPlaced(this Receipt receipt)
        {
            return new OrderPlaced(receipt.OrderId,receipt.At,string.Join(", ",receipt.Items.Select(item => item)),receipt.Subtotal,receipt.Total);
        }
    }
}
