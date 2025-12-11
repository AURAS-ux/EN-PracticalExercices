using Cafe.Domain.Beverages;
using Cafe.Domain.Order;
using Cafe.Domain.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Application.Services
{
    public interface IBeverageService
    {
        void Serve(string beverageType);
        void Customize(List<string> addOns);
        decimal ApplyPricing();
        void SetPricingStrategy(PricingStrategy strategy);
        Receipt IssueReceipt();
    }
}
