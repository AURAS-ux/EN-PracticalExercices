using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadingList.Application.Interfaces;
using ReadingList.Application.Services;
using ReadingList.CLI.Menu;
using ReadingList.Infrastructure.Persistance;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using ReadingList.Infrastructure.Parsers;
using System.Text;
using ReadingList.src.Application.Interfaces;
using ReadingList.Infrastructure.Exporters;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IParser<Book>, CsvParser>();
        services.AddSingleton<IRepository<int, Book>, InMemoryRepository<int, Book>>();
        services.AddSingleton<IImporter, ImportCsv>();
        services.AddSingleton<IImportService, ImportService>();
        services.AddSingleton<IQueryService, ListAndQueryService>();
        services.AddSingleton<IUpdateable, UpdateService>();
        services.AddSingleton<IExportStrategy, CsvExportStrategy>();
        services.AddSingleton<IExportStrategy, JsonExportStrategy>();
        services.AddSingleton<IExportService, ExportService>();
        services.AddSingleton<MainMenu>();
        services.AddSingleton<ImportMenu>();
    })
    .Build();

await host.Services.GetRequiredService<MainMenu>().RunAsync();