using Cafe.Domain.Beverages;

namespace Cafe.Domain.Factories
{
    public interface IBeverageFactory
    {
        IBeverage CreateBeverage(string beverageType);
        IBeverage Customize(string addon,IBeverage baseBeverge);
    }
}
