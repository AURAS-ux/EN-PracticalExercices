using Cafe.Domain.Beverages;
using Cafe.Domain.Beverages.Decorators;
using Cafe.Domain.Events;
using Cafe.Domain.Extentions;
using Cafe.Domain.Factories;
using Cafe.Domain.Order;
using Cafe.Domain.Pricing;

namespace Cafe.Application.Services
{
    public class BeverageService : IBeverageService
    {
        private IBeverageFactory _beverageFactory;
        private IPricingStrategyManager _pricingStrategyManager;
        private IPricingStrategy _pricingStrategy;
        private Receipt _receipt;

        private IBeverage? _beverage;
        private IOrderEventPublisher _publisher;

        public BeverageService(IBeverageFactory beverageFactory, IOrderEventPublisher publisher, IPricingStrategyManager pricingStrategyManager)
        {
            _beverageFactory = beverageFactory;
            _publisher = publisher;
            _pricingStrategyManager = pricingStrategyManager;
            _pricingStrategy = pricingStrategyManager.GetStrategy(PricingStrategy.Regular);
            _receipt = new Receipt();
        }

        public decimal ApplyPricing()
        {
            if(_beverage == null)
            {
                throw new InvalidOperationException("Beverage must be served before applying pricing.");
            }
            if(_pricingStrategy == null)
            {
                throw new InvalidOperationException("Pricing strategy must be set before applying pricing.");
            }
            _receipt.Subtotal = _beverage.Cost();
            decimal total = _pricingStrategy.Apply(_beverage.Cost());
            _receipt.Total = total;
            return total;
        }

        public void Customize(List<string> addOns)
        {
            if(_beverage == null)
            {
                throw new InvalidOperationException("Beverage must be served before customization.");
            }
            if(addOns == null || addOns.Count == 0)
            {
                return;
            }
            foreach (var addOn in addOns)
            {
                _beverage = _beverageFactory.Customize(addOn, _beverage);
                _receipt.Items.Add(addOn);
            }
        }

        public Receipt IssueReceipt()
        {
            _publisher.Published(
                new OrderPlaced(_receipt.OrderId, _receipt.At, string.Join(", ", _receipt.Items.Select(i => i)), _receipt.Subtotal, _receipt.Total)
                );
            return _receipt;
        }

        public void Serve(string beverageType)
        {
            _receipt = new Receipt
            {
                OrderId = Guid.NewGuid(),
                At = DateTime.UtcNow,
                Items = new List<string>()
            };
            _beverage = _beverageFactory.CreateBeverage(beverageType);
            _receipt.Items.Add(_beverage.Name);
        }

        public void SetPricingStrategy(PricingStrategy strategy)
        {
            _pricingStrategy = _pricingStrategyManager!.GetStrategy(strategy);
            _receipt.Pricing = _pricingStrategy.GetName();
        }
    }
}
