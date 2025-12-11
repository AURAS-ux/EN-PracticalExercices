# CafeConsole Progress Log

## Session Summary
- Clarified how BeverageService.Customize should wrap drinks with decorators and how to expose add-on choices.
- Discussed introducing DI in Program.cs using ServiceCollection, then explored handling multiple pricing strategies via keyed services or selector delegates.
- Reviewed the brief’s expectations for Observer and receipt patterns; produced the first code review (i-reviews/code-review.md).
- Authored i-reviews/design-events-observers.md detailing how to model OrderPlaced, hook up SimpleOrderEventPublisher, and wire observers.
- Examined DTO vs Receipt usage for events and discussed fixing receipt/decorator inconsistencies.
- Ran a second full review against CafeConsole_Project_Brief.md; captured findings in i-reviews/code-review-2025-11-12.md (receipt reuse, syrup flavor gaps, DI bypass, currency formatting, Happy Hour rounding, missing tests).

## Next Opportunities
1. Rework receipt lifecycle (new ID/time per order, itemized add-ons, discount line).
2. Capture syrup flavors end-to-end and expose decorator metadata for display.
3. Inject pricing strategies through DI or keyed services instead of PricingStrategyManager.
4. Move rendering/currency formatting out of the domain layer and adopt DateTimeOffset.
5. Round Happy Hour totals to two decimals and add the Cafe.Tests project with strategy/decorator/observer coverage.
