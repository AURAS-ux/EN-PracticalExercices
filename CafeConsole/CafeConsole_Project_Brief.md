# Cafe Console — .NET Console App Practice

A small console ordering app for a café, designed to practice **SOLID**, **loose coupling**, and four GoF patterns: **Strategy**, **Factory**, **Decorator**, and **Observer**.  
Target stack: **.NET 8/9 Console App**, **xUnit**, **Moq**. **No database**.

---

## Table of Contents

1. [Overview](#overview)  
2. [Learning Goals](#learning-goals)  
3. [Functional Requirements](#functional-requirements)  
4. [Non-Functional Requirements](#non-functional-requirements)  
5. [Architecture & Patterns](#architecture--patterns)  
6. [Domain Model](#domain-model)  
7. [Console UI Flow](#console-ui-flow)  
8. [Testing Plan](#testing-plan)  
9. [Acceptance Criteria](#acceptance-criteria)  
10. [Solution Layout](#solution-layout)  
11. [Build, Run, Test](#build-run-test)  
12. [Design Notes & Rationale](#design-notes--rationale)  
13. [Extension Ideas](#extension-ideas)  
14. [Reviewer’s Checklist & Rubric](#reviewers-checklist--rubric)  
15. [Coding Conventions](#coding-conventions)  
16. [License](#license)

---

## Overview

Build a console ordering app where a user:

- Picks a **base beverage**: espresso, tea, or hot chocolate.  
- Adds **optional extras** (milk, syrup with flavor, extra shot).  
- Chooses a **pricing policy** (Regular or Happy Hour).  
- Receives a **printed receipt** and the app **publishes an event** (`OrderPlaced`) observed by a console logger and an in-memory analytics component.

> Emphasis: **SOLID**, **loose coupling**, and practical use of **Strategy**, **Factory**, **Decorator**, and **Observer**.

---

## Learning Goals

- Apply **SRP**, **OCP**, **DIP** across a small codebase.  
- Decouple business rules behind **interfaces** and **manual DI**.  
- Implement & combine patterns:
  - **Factory**: create base beverages
  - **Decorator**: compose add-ons
  - **Strategy**: pricing policies
  - **Observer**: side-effect handling for order events
- Write **unit tests** for strategies, decorators, factories, and observer dispatching.  
- Keep **console UI** separate from domain logic.

---

## Functional Requirements

### Menu Flow

- List base beverages with base prices.  
- Choose one base beverage.  
- Add 0..N add-ons (repeat until “Done”).  
- Choose pricing policy (Regular / HappyHour).  
- Show receipt with itemized description, subtotal, discounts (if any), total.  
- After receipt, prompt for another order or exit.

### Pricing

- **Total** = Base beverage + add-ons.  
- **Strategies**:
  - `RegularPricing`: no discount.  
  - `HappyHourPricing`: **20%** discount on the beverage subtotal.

### Events

When an order is finalized, publish `OrderPlaced` containing:

- `OrderId (Guid)`, `At (DateTimeOffset)`, `Description (string)`, `Subtotal (decimal)`, `Total (decimal)`

At least two observers handle it:

- `ConsoleOrderLogger`: writes to console.  
- `InMemoryOrderAnalytics`: counts orders & tracks revenue.

---

## Non-Functional Requirements

- **SOLID**: small, focused classes; no “God” types.  
- **Loose coupling**: depend on **interfaces**; wiring in `Program.cs`.  
- **Testability**: business logic covered by **xUnit** (+ **Moq** as needed).  
- **Error handling**: validate and re-prompt on bad input (no crashes).  
- **Internationalization-ready**: **Currency** option (default `$`)—no hard-coded symbol in domain.  
- **No I/O in domain**: domain is **pure**; only UI uses `Console.*`.

---

## Architecture & Patterns

**Projects/Layers**

```
CafeConsole.sln
├─ Cafe.Domain              # Pure domain: beverages, decorators, pricing, events, factories (interfaces)
├─ Cafe.Application         # Use cases/orchestration: order service, event publisher
├─ Cafe.Infrastructure      # Factories + observers implementations
├─ Cafe.ConsoleUI           # Program.cs: menus and composition root
└─ Cafe.Tests               # xUnit tests (+ Moq)
```

**Patterns**

- **Factory**: `IBeverageFactory` → creates base beverages from a key (`"espresso"`, `"tea"`, `"choc"`).  
- **Decorator**: `IBeverage` + `MilkDecorator`, `SyrupDecorator(flavor)`, `ExtraShotDecorator`.  
- **Strategy**: `IPricingStrategy`: `RegularPricing`, `HappyHourPricing`.  
- **Observer**: `IOrderEventPublisher` + `IOrderEventSubscriber` (`ConsoleOrderLogger`, `InMemoryOrderAnalytics`).  
- **DIP**: `Program.cs` wires interfaces to concrete types.

---

## Domain Model

### Interfaces (domain)

```csharp
public interface IBeverage
{
    string Name { get; }
    decimal Cost();
    string Describe();
}

public interface IPricingStrategy
{
    decimal Apply(decimal subtotal);
}

public interface IOrderEventSubscriber
{
    void On(OrderPlaced evt);
}

public interface IOrderEventPublisher
{
    void Publish(OrderPlaced evt);
}

public interface IBeverageFactory
{
    IBeverage Create(string key);
}

public sealed record OrderPlaced(
    Guid OrderId,
    DateTimeOffset At,
    string Description,
    decimal Subtotal,
    decimal Total
);
```

### Base Beverages (domain)

- `Espresso` (base **$2.50**)  
- `Tea` (base **$2.00**)  
- `HotChocolate` (base **$3.00**)

### Decorators (domain)

- `MilkDecorator` (+ **$0.40**)  
- `SyrupDecorator` (+ **$0.50**, requires `flavor`)  
- `ExtraShotDecorator` (+ **$0.80**)

### Pricing Strategies (domain)

```csharp
public sealed class RegularPricing : IPricingStrategy
{
    public decimal Apply(decimal subtotal) => subtotal;
}

public sealed class HappyHourPricing : IPricingStrategy
{
    public decimal Apply(decimal subtotal) => Math.Round(subtotal * 0.80m, 2);
}
```

---

## Console UI Flow

**Main Menu**
1. Choose base:
   - `1) Espresso` `2) Tea` `3) Hot Chocolate`
2. Add-ons (repeat until Done):
   - `1) Milk (+0.40)` `2) Syrup (+0.50)` `3) Extra shot (+0.80)` `0) Done`  
   - If *Syrup* selected → prompt for `flavor` (e.g., `"vanilla"`).
3. Pricing policy:
   - `1) Regular` `2) Happy Hour`
4. Receipt:
   - Description, Subtotal, Discount (if any), Total, Order Id, Timestamp.
5. Again?
   - `1) New order` `0) Exit`

**Sample Receipt**

```
Order 7f0a7a1f-... @ 2025-10-25T10:22:13+02:00
Items: Espresso, milk, vanilla syrup, extra shot
Subtotal: $4.20
Pricing: HappyHour (-20%)
Total: $3.36
```

---

## Testing Plan

**Frameworks**: xUnit + Moq

Recommended unit tests:

1. **Decorators add cost & description**  
   - Espresso $2.50 + Milk $0.40 + ExtraShot $0.80 = **$3.70**  
   - `Describe()` contains `"milk"` and `"extra shot"`.

2. **Pricing strategies**  
   - `RegularPricing.Apply(10.00m) == 10.00m`  
   - `HappyHourPricing.Apply(10.00m) == 8.00m` (rounded to 2 decimals)

3. **Factory returns correct type**  
   - `"espresso"` → `Espresso`, `"tea"` → `Tea`  
   - Invalid key → throws `ArgumentException`.

4. **Observer is notified**  
   - Moq `IOrderEventSubscriber`, verify `On` called once with matching totals.

5. **Order analytics accumulates**  
   - Publish totals `3.50` and `2.00` → revenue `5.50`, count `2`.

---

## Acceptance Criteria

- **Base, no add-ons, Regular** → total equals base price.  
- **Espresso + Milk + Extra Shot** → subtotal `2.50 + 0.40 + 0.80 = 3.70`.  
- **Any subtotal 10.00, HappyHour** → total `8.00` (rounded to 2 decimals).  
- **Order finalized** → both subscribers invoked **exactly once**.  
- **Invalid input** → user is re-prompted; app never crashes.  
- **Receipt** → includes description, subtotal, pricing policy, total, order id, timestamp.

---

## Solution Layout

```
CafeConsole.sln
├── Cafe.Domain
│   ├── Beverages
│   │   ├── IBeverage.cs
│   │   ├── Espresso.cs
│   │   ├── Tea.cs
│   │   ├── HotChocolate.cs
│   │   ├── Decorators
│   │   │   ├── MilkDecorator.cs
│   │   │   ├── SyrupDecorator.cs
│   │   │   └── ExtraShotDecorator.cs
│   ├── Pricing
│   │   ├── IPricingStrategy.cs
│   │   ├── RegularPricing.cs
│   │   └── HappyHourPricing.cs
│   ├── Events
│   │   ├── OrderPlaced.cs
│   │   ├── IOrderEventSubscriber.cs
│   │   └── IOrderEventPublisher.cs
│   └── Factories
│       └── IBeverageFactory.cs
├── Cafe.Application
│   └── Services
│       ├── OrderService.cs
│       └── SimpleOrderEventPublisher.cs
├── Cafe.Infrastructure
│   ├── Factories
│   │   └── BeverageFactory.cs
│   └── Observers
│       ├── ConsoleOrderLogger.cs
│       └── InMemoryOrderAnalytics.cs
├── Cafe.ConsoleUI
│   └── Program.cs
└── Cafe.Tests
    ├── DecoratorTests.cs
    ├── PricingStrategyTests.cs
    ├── BeverageFactoryTests.cs
    ├── EventPublisherTests.cs
    └── AnalyticsTests.cs
```

---

## Build, Run, Test

### Prerequisites
- **.NET 8 SDK** or **.NET 9 SDK**

### Commands

```bash
dotnet restore
dotnet build
dotnet run --project ./Cafe.ConsoleUI
dotnet test ./Cafe.Tests
```

---

## Design Notes & Rationale

### Composition Root Example

```csharp
IBeverageFactory beverageFactory = new BeverageFactory();
IOrderEventSubscriber logger = new ConsoleOrderLogger();
var analytics = new InMemoryOrderAnalytics();
IOrderEventPublisher publisher = new SimpleOrderEventPublisher(new [] { logger, analytics });
```

- **DIP**: UI knows only interfaces.  
- **OCP**: new beverages/add-ons extend without modifying existing types.  
- **Purity**: domain logic is deterministic and testable.

---

## Extension Ideas

- Template Method for beverage preparation sequence.  
- State pattern for Kiosk UI flow.  
- Proxy for remote analytics API.  
- Composite for Combo orders.  
- Options pattern for JSON-based pricing.  
- Strategy variants: membership or coupons.

---

## Reviewer’s Checklist & Rubric

| Area | Points | Criteria |
|------|---------|----------|
| Correctness & Features | 35 | Menu, pricing, receipt, events |
| Design Quality | 25 | SOLID, patterns, decoupling |
| Tests | 20 | Unit tests, mocks |
| Code Quality | 10 | Naming, readability |
| Error Handling & UX | 5 | Validation, clarity |
| Extensibility | 5 | Easy to extend |

---

## Coding Conventions

- PascalCase for types/members; camelCase for locals.  
- Guard clauses for validation.  
- Nullable reference types enabled.  
- Manual DI; no external container required.

---

## License

MIT (or organization standard).
