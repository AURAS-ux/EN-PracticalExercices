using ReadingList.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.CLI.Menu
{
    public class ImportMenu
    {
        private readonly IImportService importService;
        public ImportMenu(IImportService importService)
        {
            Console.WriteLine();
            Console.WriteLine("=== Entered Import Menu ===");
            Console.WriteLine("=== Available options ===");
            Console.WriteLine(">import <file> | import <file1> <file2> ...");
            Console.WriteLine(">exit");
            Console.WriteLine("=========================");
            this.importService = importService;
        }

        public async Task RunAsync()
        {
            bool running = true;
            do
            {
                string? inputCommand = Console.ReadLine();
                if (string.IsNullOrEmpty(inputCommand))
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    continue;
                }
                var commandParts = inputCommand.Split(' ');
                switch (commandParts[0].ToLower())
                {
                    case "import":
                        var files = commandParts.Skip(1).ToArray();
                        if (files.Length == 0)
                        {
                            Console.WriteLine("No files specified. Please try again.");
                            break;
                        }
                        Console.WriteLine($"Importing files: {string.Join(", ", files)}");
                        var importResult = await importService.ImportAsync(files);
                        if (!importResult.IsSuccess)
                        {
                            Console.WriteLine($"Error in importing files({files})");
                            break;
                        }
                        Console.WriteLine($"Successfuly imported {importResult.Value!.Count} entries");
                        break;
                    case "exit":
                        running = false;
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            } while (running);
        }
    }
}
