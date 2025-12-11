using ReadingList.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.CLI.Menu
{
    internal class ExportMenu
    {
        private readonly IExportService exportService;
        public ExportMenu(IExportService exportService)
        {
            Console.WriteLine();
            Console.WriteLine("=== Export menu started ===");
            Console.WriteLine("Please input one of the following commads:");
            Console.WriteLine("> export csv - Export the reading list to a CSV file");
            Console.WriteLine("> export json - Export the reading list to a JSON file");
            Console.WriteLine("> exit - Return to the main menu");
            Console.WriteLine("===========================");
            Console.WriteLine();
            this.exportService = exportService;
        }

        public async Task RunAsync()
        {
            bool running = true;
            do
            {
                string inputCommand = Console.ReadLine()!;
                Result<ExportResult>? result = null; 
                string exportPath = string.Empty;
                if (string.IsNullOrEmpty(inputCommand))
                {
                    Console.WriteLine("Invalid command. Please try again.");
                    continue;
                }
                if (inputCommand.StartsWith("exit"))
                {
                    Console.Clear();
                    running = false;
                }
                else if (inputCommand.StartsWith("export"))
                {
                    var exportLocation = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
                    if (!Directory.Exists(exportLocation))
                    {
                        Directory.CreateDirectory(exportLocation);
                    }
                    exportPath = Path.Combine(exportLocation, $"reading_list_{DateTime.Now:yyyyMMdd_HHmmss}");
                    var args = inputCommand.Split(' ');
                    if (args.Length != 2)
                    {
                        Console.WriteLine("Invalid command format. Please use 'export csv' or 'export json'.");
                        continue;
                    }
                    var format = args[1].ToLower();
                    switch (format)
                    {
                        case "csv":
                            result = await exportService.ExportReadingListAsync(ExportType.CSV, exportPath);
                            break;
                        case "json":
                            result = await exportService.ExportReadingListAsync(ExportType.JSON, exportPath);
                            break;
                        default:
                            Console.WriteLine("Unsupported export format. Please use 'csv' or 'json'.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Please try again.");
                }
                if (result != null)
                {
                    if (result.IsSuccess && result.Value.IsSuccessful)
                    {
                        Console.WriteLine($"Export completed successfully at path {exportPath}.");
                    }
                    else
                    {
                        Console.WriteLine($"Export failed: {result.Error}");
                    }
                }
            }
            while (running);
        }
    }
}
