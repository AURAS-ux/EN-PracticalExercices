using Cafe.Application.Services;
using Cafe.Domain.Events;
using Cafe.Domain.Extentions;
using Cafe.Domain.Interfaces;
using Cafe.Domain.Pricing;
using Cafe.Infrastructure.Observers;

namespace Cafe.ConsoleUI.Menus
{
    internal class MainMenu
    {
        private IBeverageService _beverageService;
        private IAnalytics _analytics;

        public MainMenu(IBeverageService beverageService, IAnalytics analytics)
        {
            _beverageService = beverageService;
            _analytics = analytics;
        }

        private void Start()
        {
            Console.WriteLine("=== Coffee Console Started ===");
            Console.WriteLine();
            Console.WriteLine("Please Provide your order details");
        }

        public void Run()
        {
            bool running = true;
            do
            {
                Start();
                SelectBeverage();
                SelectAddOns();
                SelectPricing();
                PrintReceipt();
                running = RestartProcess();
            }
            while (running);
            End();
        }

        private void End()
        {
            Console.WriteLine("=== Coffee Console Ended ===");
            Console.WriteLine($"=== Analitycs for ${DateTime.Now} ===");
            Console.WriteLine($"Total Orders: {_analytics.TotalOrders}");
            Console.WriteLine($"Total Revenue: ${_analytics.TotalRevenue}");
            Console.WriteLine("===/===");
            Console.WriteLine();
        }

        private bool RestartProcess()
        {
            Console.WriteLine("=== Order finished ===");
            Console.WriteLine("Do you want to order something else?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");
            Console.WriteLine("===/===");
            string input = Utils.RequestInput();
            switch (input)
            {
                case "1":
                    return true;
                case "2":
                    return false;
                default:
                    Console.WriteLine("Invalid selection. Exiting.");
                    return false;
            }
        }

        private void PrintReceipt()
        {
            Console.WriteLine("=== Printing Receipt ===");
            var receipt = _beverageService.IssueReceipt();
            Console.WriteLine(receipt.Display());
            Console.WriteLine("===/===");
        }

        private void SelectPricing()
        {
            Console.WriteLine("=== Select Pricing Type ===");
            Console.WriteLine();
            Console.WriteLine("1. Regular");
            Console.WriteLine("2. Happy Hour");
            Console.WriteLine("===/===");
            bool running = true;
            do
            {
                string input = Utils.RequestInput();
                switch (input)
                {
                    case "1":
                        _beverageService.SetPricingStrategy(PricingStrategy.Regular);
                        running = false;
                        break;
                    case "2":
                        _beverageService.SetPricingStrategy(PricingStrategy.HappyHour);
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
            while (running);
            try
            {
                _beverageService.ApplyPricing(); //TODO: Use subtotal
            }catch(InvalidOperationException ioe)
            {
                Console.WriteLine($"Error calculating the price {ioe.Message}");
            }
        }

        private void SelectAddOns()
        {
            Console.WriteLine("=== Available AddOns ===");
            Console.WriteLine();
            Console.WriteLine("1. Milk(+0.40)");
            Console.WriteLine("2. Syrup(+0.50)");
            Console.WriteLine("3. Extra Shot(+0.80)");
            Console.WriteLine("0. Done");
            Console.WriteLine("===/===");
            bool running = true;
            List<string> selectedAddOns = new List<string>();
            do
            {
                string input = Utils.RequestInput();
                switch (input)
                {
                    case "1":
                        selectedAddOns.Add("milk");
                        break;
                    case "2":
                        SelectSyrups(ref selectedAddOns);
                        break;
                    case "3":
                        selectedAddOns.Add("extrashot");
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
            while (running);

            _beverageService.Customize(selectedAddOns);
        }

        private void SelectSyrups(ref List<string> selectedAddOns)
        {
            Console.WriteLine("=== Syrup Selection ===");
            Console.WriteLine();
            Console.WriteLine("1. Vanilla");
            Console.WriteLine("2. Caramel");
            Console.WriteLine("3. Hazelnut");
            Console.WriteLine("0. Back");
            Console.WriteLine("===/===");
            bool running = true;
            do
            {
                string input = Utils.RequestInput();
                switch (input)
                {
                    case "1":
                        selectedAddOns.Add("syrup vanilla");
                        running = false;
                        break;
                    case "2":
                        selectedAddOns.Add("syrup caramel");
                        running = false;
                        break;
                    case "3":
                        selectedAddOns.Add("syrup hazelnut");
                        running = false;
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            } while(running);
            Console.WriteLine("Finished selection");
            Console.WriteLine("Back to addons selection\n");
        }

        private void SelectBeverage()
        {
            Console.WriteLine("=== Available Beverages ===");
            Console.WriteLine();
            Console.WriteLine("1. Espresso (base 2.50)");
            Console.WriteLine("2. Tea (base 2.00)");
            Console.WriteLine("3. Hot Chocolate (base 3.00)");
            Console.WriteLine("===/===");
            Console.WriteLine();
            bool running = true;
            do
            {
                string input = Utils.RequestInput();
                switch (input)
                {
                    case "1":
                        _beverageService.Serve("espresso");
                        running = false;
                        break;
                    case "2":
                        _beverageService.Serve("tea");
                        running = false;
                        break;
                    case "3":
                        _beverageService.Serve("hotchocolate");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Please try again.");
                        break;
                }
            }
            while (running);
        }
    }
}
