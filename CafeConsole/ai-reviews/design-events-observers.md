# Design: Order Events & Observers

## Goal
Bring the Observer pattern back in line with the project brief by publishing an `OrderPlaced` event whenever a receipt is issued and having both a console logger and in-memory analytics subscriber react to it.

---

## 1. Model the Event Contracts (Domain Layer)
Create the event record and the subscriber/publisher interfaces under `Cafe.Domain/Events` so everything else can depend on pure abstractions.

```csharp
// Cafe.Domain/Events/OrderPlaced.cs
namespace Cafe.Domain.Events;

public sealed record OrderPlaced(
    Guid OrderId,
    DateTimeOffset At,
    string Description,
    decimal Subtotal,
    decimal Total);

// Cafe.Domain/Events/IOrderEventSubscriber.cs
namespace Cafe.Domain.Events;

public interface IOrderEventSubscriber
{
    void On(OrderPlaced evt);
}

// Cafe.Domain/Events/IOrderEventPublisher.cs
namespace Cafe.Domain.Events;

public interface IOrderEventPublisher
{
    void Publish(OrderPlaced evt);
}
```

> Keep these types free of any console concerns to maintain the "domain has no I/O" rule.

---

## 2. Implement a Simple Publisher (Application Layer)
`SimpleOrderEventPublisher` just iterates through every registered subscriber. Put it under `Cafe.Application/Services` (or a dedicated `Events` folder) so higher layers can inject it.

```csharp
using Cafe.Domain.Events;

namespace Cafe.Application.Events;

public sealed class SimpleOrderEventPublisher : IOrderEventPublisher
{
    private readonly IEnumerable<IOrderEventSubscriber> _subscribers;

    public SimpleOrderEventPublisher(IEnumerable<IOrderEventSubscriber> subscribers)
        => _subscribers = subscribers;

    public void Publish(OrderPlaced evt)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.On(evt);
        }
    }
}
```

---

## 3. Raise the Event from `BeverageService`
Once pricing is applied and the final receipt is ready, map it into an `OrderPlaced` payload and hand it to the publisher. Inject `IOrderEventPublisher` through the service constructor.

```csharp
public class BeverageService : IBeverageService
{
    private readonly IOrderEventPublisher _publisher;

    public BeverageService(IBeverageFactory factory,
                           IOrderEventPublisher publisher /* ... */)
    {
        _publisher = publisher;
        // ...
    }

    public Receipt IssueReceipt()
    {
        if (_receipt.Subtotal <= 0)
        {
            throw new InvalidOperationException("Pricing must be applied before issuing the receipt.");
        }

        var receipt = _receipt;
        _publisher.Publish(new OrderPlaced(
            receipt.OrderId,
            receipt.At,
            string.Join(", ", receipt.Items.Select(i => i.Name)),
            receipt.Subtotal,
            receipt.Total));
        return receipt;
    }
}
```

Guard this call so it only fires after pricing is computed, ensuring observers always see consistent totals.

---

## 4. Flesh Out the Observers (Infrastructure Layer)
Implement the concrete subscribers so they actually do something useful.

```csharp
// Cafe.Infrastructure/Observers/ConsoleOrderLogger.cs
using Cafe.Domain.Events;

namespace Cafe.Infrastructure.Observers;

public sealed class ConsoleOrderLogger : IOrderEventSubscriber
{
    public void On(OrderPlaced evt)
    {
        Console.WriteLine(
            $"[{evt.At:HH:mm:ss}] Order {evt.OrderId} subtotal {evt.Subtotal:C} total {evt.Total:C}");
    }
}

// Cafe.Infrastructure/Observers/InMemoryOrderAnalytics.cs
using Cafe.Domain.Events;

namespace Cafe.Infrastructure.Observers;

public sealed class InMemoryOrderAnalytics : IOrderEventSubscriber
{
    public int Orders { get; private set; }
    public decimal Revenue { get; private set; }

    public void On(OrderPlaced evt)
    {
        Orders++;
        Revenue += evt.Total;
    }
}
```

The analytics class exposes read-only properties so tests (and future diagnostics screens) can inspect the accumulated state.

---

## 5. Wire Everything in `Program.cs`
Register the publisher as a singleton so the same analytics instance can accumulate state, and add both observers to the service collection.

```csharp
services.AddSingleton<IOrderEventPublisher, SimpleOrderEventPublisher>();
services.AddSingleton<IOrderEventSubscriber, ConsoleOrderLogger>();
services.AddSingleton<IOrderEventSubscriber, InMemoryOrderAnalytics>();
```

Because `SimpleOrderEventPublisher` depends on `IEnumerable<IOrderEventSubscriber>`, the container will inject both observers automatically. `MainMenu` (or any future orchestrator) just resolves `IBeverageService`; the event pipeline stays hidden behind the interfaces.

---

## 6. Test the Pipeline
Add unit tests under `Cafe.Tests` that:

1. Arrange a fake subscriber, publish an `OrderPlaced`, and assert `On` was invoked exactly once.
2. Verify `InMemoryOrderAnalytics` increments `Orders` and `Revenue` when receiving multiple events.

This keeps the Observer pattern from regressing again.

---

Following these steps reintroduces the event flow mandated by the brief: every finalized order emits an `OrderPlaced` snapshot, the console logger prints it, analytics capture metrics, and all dependencies stay loosely coupled through interfaces.
