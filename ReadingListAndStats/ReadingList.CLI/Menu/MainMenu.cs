using ReadingList.Application.Interfaces;

namespace ReadingList.CLI.Menu
{
    public class MainMenu(IImportService importService,IQueryService queryService,IUpdateable updateService, IExportService exportService)
    {
        public async Task RunAsync()
        {
            bool running = true;
            do
            {
                Console.WriteLine("=== ReadingList App Started ===");
                Console.WriteLine("1. Import books");
                Console.WriteLine("2. List & Query books");
                Console.WriteLine("3. Update book");
                Console.WriteLine("4. Export");
                Console.WriteLine("5. Help & Exit");
                Console.WriteLine();
                Console.Write("Select an option: ");
                string? choice = Console.ReadLine();
                if (choice == null)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }
                switch (choice)
                {
                    case "1":
                        var importMenu = new ImportMenu(importService);
                        await importMenu.RunAsync();
                        break;
                    case "2":
                        var listMenu = new ListMenu(queryService);
                        await listMenu.RunAsync();
                        break;
                    case "3":
                        var updateMenu = new UpdateMenu(updateService);
                        updateMenu.Run();
                        break;
                    case "4":
                        var exportMenu = new ExportMenu(exportService);
                        await exportMenu.RunAsync();
                        break;
                    case "5":
                        Console.WriteLine("Help: Select an option by entering the corresponding number.");
                        Console.WriteLine("Exiting the application. Goodbye!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            while (running);
        }
    }
}
