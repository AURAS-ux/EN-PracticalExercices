using Cafe.Domain.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Domain.Extentions
{
    public static class DisplayReceipt
    {
        public static string Display(this Receipt receipt)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Order {receipt.OrderId} @ {receipt.At:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("Items:");

            if (receipt.Items != null && receipt.Items.Any())
            {
                foreach (var item in receipt.Items)
                {
                    sb.AppendLine($"  • {item}");
                }
            }
            else
            {
                sb.AppendLine("  (No items)");
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Subtotal: ${receipt.Subtotal:0.00}");
            sb.AppendLine($"Pricing Policy: {receipt.Pricing}");
            sb.AppendLine($"Total: ${receipt.Total:0.00}");
            sb.AppendLine(new string('-', 40));

            return sb.ToString();
        }
    }
}
