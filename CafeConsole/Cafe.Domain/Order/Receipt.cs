using Cafe.Domain.Beverages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Order
{
    public class Receipt
    {
        public Guid OrderId { get; set; }
        public DateTime At { get; set; }
        public List<string> Items { get; set; } = new List<string>();
        public decimal Subtotal { get; set; }
        public string Pricing { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
