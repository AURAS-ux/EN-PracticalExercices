using Cafe.Application;
using Cafe.Application.Services;
using Cafe.ConsoleUI.Menus;
using Cafe.Domain.Events;
using Cafe.Domain.Factories;
using Cafe.Domain.Interfaces;
using Cafe.Domain.Pricing;
using Cafe.Infrastructure.Factories;
using Cafe.Infrastructure.Observers;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IBeverageFactory, BeverageFactory>();
services.AddScoped<IBeverageService, BeverageService>();
services.AddSingleton<IPricingStrategyManager, PricingStrategyManager>();
services.AddSingleton<RegularPricing>();
services.AddSingleton<HappyHourPricing>();
services.AddSingleton<IOrderEventPublisher, SimpleOrderEventPublisher>();
services.AddSingleton<IOrderEventSubscriber, ConsoleOrderObserver>();
services.AddSingleton<IOrderEventSubscriber, InMemoryOrderAnalytics>();
services.AddSingleton<IAnalytics, InMemoryOrderAnalytics>();
services.AddSingleton<MainMenu>();

var serviceProvider = services.BuildServiceProvider();
var menu = serviceProvider.GetRequiredService<MainMenu>();
menu.Run();