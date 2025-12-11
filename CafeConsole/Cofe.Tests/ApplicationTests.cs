using Cafe.Application;
using Cafe.Application.Services;
using Cafe.Domain.Beverages;
using Cafe.Domain.Events;
using Cafe.Domain.Factories;
using Cafe.Domain.Pricing;
using Cafe.Infrastructure.Factories;
using Cafe.Infrastructure.Observers;
using Moq;

namespace Cofe.Tests
{
    public class ApplicationTests
    {
        private Mock<IBeverageFactory> beverageFactoryMock;
        private Mock<IOrderEventPublisher> eventPublisherMock;
        private IBeverageService beverageService;
        private Mock<IPricingStrategyManager> pricingStrategyManagerMock;
        public ApplicationTests()
        {
            eventPublisherMock = new Mock<IOrderEventPublisher>();
            beverageFactoryMock = new Mock<IBeverageFactory>();
            pricingStrategyManagerMock = new Mock<IPricingStrategyManager>();
            beverageService = new BeverageService(beverageFactoryMock.Object, eventPublisherMock.Object,pricingStrategyManagerMock.Object);
        }


        [Fact]
        public void TestDecoratorPricing()
        {
            //Arrange
            var beverage = new Mock<IBeverage>();
            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(2.50m);

            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);

            //Act
            beverageService.Serve("espresso");
            beverageService.Customize(["milk", "extrashot"]);
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.Regular))
                .Returns(new RegularPricing());
            beverageService.SetPricingStrategy(PricingStrategy.Regular);
            decimal total = beverageService.ApplyPricing();

            //Assert
            Assert.True(total == 3.70m);
        }

        [Fact]
        public void TestDecoratorItems()
        {
            //Arrange
            var beverage = new Mock<IBeverage>();
            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(2.50m);

            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);

            //Act
            beverageService.Serve("espresso");
            beverageService.Customize(["milk", "extrashot"]);
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.Regular))
                .Returns(new RegularPricing());
            beverageService.SetPricingStrategy(PricingStrategy.Regular);
            var receipt = beverageService.IssueReceipt();

            //Assert
            Assert.Contains("milk", receipt.Items);
            Assert.Contains("extrashot", receipt.Items);
        }

        [Fact]
        public void TestPricingStrategyRegular()
        {
            //Arrange
            decimal testPrice = 10m;
            var beverage = new Mock<IBeverage>();
            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(testPrice);

            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);

            //Act
            beverageService.Serve("espresso");
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.Regular))
                .Returns(new RegularPricing());
            beverageService.SetPricingStrategy(PricingStrategy.Regular);
            var receipt = beverageService.IssueReceipt();
            var total = beverageService.ApplyPricing();

            //Assert
            Assert.Equal(receipt.Total, testPrice);
            Assert.Equal(total, testPrice);
        }

        [Fact]
        public void TestPricingStrategyHappyHour()
        {
            //Arrange
            decimal testPrice = 10m;
            decimal discountedPrice = testPrice * 0.8m;
            var beverage = new Mock<IBeverage>();
            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(testPrice);

            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);

            //Act
            beverageService.Serve("espresso");
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.HappyHour))
                .Returns(new HappyHourPricing());
            beverageService.SetPricingStrategy(PricingStrategy.HappyHour);
            var receipt = beverageService.IssueReceipt();
            var total = beverageService.ApplyPricing();

            //Assert
            Assert.Equal(receipt.Total, discountedPrice);
            Assert.Equal(total, discountedPrice);
        }

        [Fact]
        public void BeverageFactory_CreatesCorrectBeverage()
        {
            //Arrange
            var beverageFactory = new BeverageFactory();
            //Act
            var espresso = beverageFactory.CreateBeverage("espresso");
            var tea = beverageFactory.CreateBeverage("tea");
            var hotChocolate = beverageFactory.CreateBeverage("hotchocolate");
            //Assert
            Assert.IsType<Espresso>(espresso);
            Assert.IsType<Tea>(tea);
            Assert.IsType<HotChocolate>(hotChocolate);
        }

        [Fact]
        public void BeverageFactory_ThrowsOnInvalidBeverage()
        {
            //Arrange
            var beverageFactory = new BeverageFactory();
            //Act & Assert
            Assert.Throws<ArgumentException>(() => beverageFactory.CreateBeverage("invalidbeverage"));
        }

        [Fact]
        public void EventSubscriber_OnCalledOnce()
        {
            //Arrange
            var beverage = new Mock<IBeverage>();
            var subscriber = new Mock<IOrderEventSubscriber>();
            var simplePublisher = new SimpleOrderEventPublisher([subscriber.Object]);
            var beverageServiceWithPublisher = new BeverageService(beverageFactoryMock.Object, simplePublisher, pricingStrategyManagerMock.Object);

            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(2.50m);
            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);
            //Act
            beverageServiceWithPublisher.Serve("espresso");
            beverageServiceWithPublisher.IssueReceipt();
            //Assert
            subscriber.Verify(sub => sub.On(It.IsAny<OrderPlaced>()), Times.Once);
        }

        [Fact]
        public void OrderAnalytics_TwoOrders()
        {
            //Arrange
            decimal testPrice = 3.5m;
            decimal testPrice2 = 2m;
            int ordersCount = 2;
            decimal price = 5.5m;

            var subscriber = new InMemoryOrderAnalytics();
            var simplePublisher = new SimpleOrderEventPublisher([subscriber]);
            var beverageServiceWithPublisher = new BeverageService(beverageFactoryMock.Object, simplePublisher, pricingStrategyManagerMock.Object);

            var beverage = new Mock<IBeverage>();
            beverage.Setup(b => b.Name).Returns("Espresso");
            beverage.Setup(b => b.Cost()).Returns(testPrice);

            var beverage2 = new Mock<IBeverage>();
            beverage2.Setup(b => b.Name).Returns("Tea");
            beverage2.Setup(b => b.Cost()).Returns(testPrice2);

            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("espresso")).Returns(beverage.Object);
            beverageFactoryMock.Setup(bfm => bfm.CreateBeverage("tea")).Returns(beverage2.Object);

            //Act
            beverageServiceWithPublisher.Serve("espresso");
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.Regular))
                .Returns(new RegularPricing());
            beverageServiceWithPublisher.SetPricingStrategy(PricingStrategy.Regular);
            var total = beverageServiceWithPublisher.ApplyPricing();
            var receipt = beverageServiceWithPublisher.IssueReceipt();

            beverageServiceWithPublisher.Serve("tea");
            pricingStrategyManagerMock.Setup(mock => mock.GetStrategy(PricingStrategy.Regular))
                .Returns(new RegularPricing());
            beverageServiceWithPublisher.SetPricingStrategy(PricingStrategy.Regular);
            var total2 = beverageServiceWithPublisher.ApplyPricing();
            var receipt2 = beverageServiceWithPublisher.IssueReceipt();

            //Assert
            Assert.Equal(subscriber.TotalOrders, ordersCount);
            Assert.Equal(subscriber.TotalRevenue, price);
        }
    }
}
