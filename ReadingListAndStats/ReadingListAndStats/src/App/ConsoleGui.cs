using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using ReadingList.src.Infrastructure;
using ReadingList.src.Infrastructure.Services;
using Serilog;
using System.Threading.Tasks;

namespace ReadingList.src.App
{
    public class ConsoleGui : ITextGui
    {
        private ILogger logger = AppLogger.Logger;
        private IImportService _importer = new CsvImporterService();
        private IRepository<int, Book> _bookRepository = new InMemoryRepository<int, Book>();
        public ConsoleGui()
        {
            Console.WriteLine("=== ReadingList App started ===");
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("1. Import books");
            Console.WriteLine("2. List & Query books");
            Console.WriteLine("3. Update book");
            Console.WriteLine("4. Export");
            Console.WriteLine("5. Help & Exit");
        }

        public async Task RunAsync()
        {
            while (true)
            {
                ShowMainMenu();
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        await ShowImportMenuAsync();
                        break;
                    case "2":
                        await ShowListAndQueryMenuAsync();
                        break;
                    case "3":
                        ShowUpdateMenu();
                        break;
                    case "4":
                        ShowExportMenu();
                        break;
                    case "5":
                        ShowHelpAndExitMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        public async Task ShowImportMenuAsync()
        {
            Console.WriteLine();
            Console.WriteLine("=== Entered Import Menu ===");
            Console.WriteLine("=== Available options ===");
            Console.WriteLine(">import <file> | import <file1> <file2> ...");
            Console.WriteLine(">exit");
            Console.WriteLine("=========================");
            bool exitRecorded = false;
            while (!exitRecorded)
            {
                var line = Console.ReadLine();
                if (line == null)
                {
                    continue;
                }
                var args = line.Split(' ');
                if (args.Length == 0)
                {
                    logger.Warning("No command entered in Import Menu");
                    break;
                }

                var command = args[0].ToLower();
                switch (command)
                {
                    case "import":
                        await AppUtils.HandleImport(args, _importer, _bookRepository);
                        break;
                    case "exit":
                        logger.Information("Exiting Import Menu");
                        exitRecorded = true;
                        break;
                    default:
                        logger.Warning($"Unknown command '{command}' in Import Menu");
                        break;
                }

            }
            Console.WriteLine("=== Import finished successfuly ===");
            Console.WriteLine();
        }

        public async Task ShowListAndQueryMenuAsync()
        {
            Console.WriteLine();
            Console.WriteLine("=== Entered Profile Menu ===");
            Console.WriteLine("=== Available options ===");
            Console.WriteLine(">list-books(lb)");
            Console.WriteLine(">list-books-finished(lbf)");
            Console.WriteLine(">list-books-unfinished(lbu)");
            Console.WriteLine(">list-top-n-books(lt)");
            Console.WriteLine(">list-books-by-author(lba)");
            Console.WriteLine(">view-stats(vs)");
            Console.WriteLine(">exit(e)");
            Console.WriteLine("=========================");
            bool exitRecorded = false;
            do
            {
                var command = Console.ReadLine();
                if (command == null)
                {
                    logger.Warning("No command entered in List & Query Menu");
                    continue;
                }
                switch (command.ToLower())
                {
                    case "list-books":
                    case "lb":
                        Result<List<Book>> allBooks = await _bookRepository.GetAllAsync();
                        Console.WriteLine("=== All Books ===");
                        foreach (var book in allBooks.Value!)
                        {
                            Console.WriteLine(book.ToString());
                        }
                        Console.WriteLine("=================");
                        break;
                    case "list-books-finished":
                    case "lbf":
                        Result<List<Book>> finishedBooks = _bookRepository.GetAllWhen(book => book.Finished);
                        Console.WriteLine("=== Finished Books ===");
                        foreach (var book in finishedBooks.Value!)
                        {
                            Console.WriteLine(book.ToString());
                        }
                        Console.WriteLine("=================");
                        break;
                    case "list-books-unfinished":
                    case "lbu":
                        Result<List<Book>> unfinishedBooks = _bookRepository.GetAllWhen(book => !book.Finished);
                        Console.WriteLine("=== Unfinished Books ===");
                        foreach (var book in unfinishedBooks.Value!)
                        {
                            Console.WriteLine(book.ToString());
                        }
                        Console.WriteLine("=================");
                        break;
                    case "list-top-n-books":
                    case "lt":
                        Console.Write("Enter N: ");
                        var nInput = Console.ReadLine();
                        if (int.TryParse(nInput, out int n))
                        {
                            Result<List<Book>> topNBooks = _bookRepository.GetAllWhen(book => book.Rating >= 4.5f);
                            var topN = topNBooks.Value!.OrderByDescending(b => b.Rating).Take(n).ToList();
                            Console.WriteLine($"=== Top {n} Books ===");
                            foreach (var book in topN)
                            {
                                Console.WriteLine(book.ToString());
                            }
                            Console.WriteLine("=================");
                        }
                        else
                        {
                            logger.Warning("Invalid number entered for Top N Books");
                        }
                        break;
                    case "list-books-by-author":
                    case "lba":
                        Console.Write("Enter Author Name: ");
                        var authorName = Console.ReadLine();
                        if (!string.IsNullOrEmpty(authorName))
                        {
                            Result<List<Book>> booksByAuthor = _bookRepository.GetAllWhen(book => book.Author.Equals(authorName, StringComparison.OrdinalIgnoreCase));
                            Console.WriteLine($"=== Books by {authorName} ===");
                            foreach (var book in booksByAuthor.Value!)
                            {
                                Console.WriteLine(book.ToString());
                            }
                            Console.WriteLine("=================");
                        }
                        else
                        {
                            logger.Warning("No author name entered for Books by Author");
                        }
                        break;
                    case "view-stats":
                    case "vs":
                        Console.WriteLine(AppUtils.ComputeUserStats(ref _bookRepository));
                        break;
                    case "exit":
                    case "e":
                        logger.Information("Exiting List & Query Menu");
                        exitRecorded = true;
                        break;
                    default:
                        logger.Warning($"Unknown command '{command}' in List & Query Menu");
                        break;
                }
            } while (!exitRecorded);


            Console.WriteLine("=== Profile viewed successfuly ===");
            Console.WriteLine();
        }

        public void ShowUpdateMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== Update menu started");
            Console.WriteLine();
            Console.WriteLine(">mark finished <id>");
            Console.WriteLine(">rate <id> <stars/5>");
            Console.WriteLine(">exit");
            bool exitRecorded = false;
            IUpdateRepositoryService<int, Book> updateService = new UpdateRepositoryService<int, Book>();
            do
            {
                string command = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(command))
                {
                    logger.Warning("No command entered in Update Menu");
                    continue;
                }
                var args = command.Split(' ');
                switch (args[0])
                {
                    case "mark":
                        var updateResult = updateService.UpdateFinished(int.Parse(args[2]), ref _bookRepository);
                        if (updateResult.IsSuccess)
                        {
                            logger.Information($"Book Id:{args[2]} marked as finished.");
                        }
                        else
                        {
                            logger.Error($"Failed to mark Book Id:{args[2]} as finished. {updateResult.Error}");
                        }
                        break;
                    case "rate":
                        var rateResult = updateService.RateAsync(int.Parse(args[1]), int.Parse(args[2]), ref _bookRepository);
                        if (rateResult.IsSuccess)
                        {
                            logger.Information($"Book Id:{args[1]} rated with {args[2]} stars.");
                        }
                        else
                        {
                            logger.Error($"Failed to rate Book Id:{args[1]}. {rateResult.Error}");
                        }
                        break;
                    case "exit":
                        logger.Information("Exiting Update Menu");
                        exitRecorded = true;
                        break;
                    default:
                        logger.Warning($"Unknown command '{command}' in Update Menu");
                        break;
                }
            } while (!exitRecorded);

            Console.WriteLine("=== Update finished ===");
        }

        public void ShowExportMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== Export Menu ===");
            Console.WriteLine();
            Console.WriteLine(">export json <path>");
            Console.WriteLine(">export csv <path>");
            Console.WriteLine(">exit");
            Console.WriteLine();
            bool exitRecorded = false;
            do
            {
                var line = Console.ReadLine();
                if(string.IsNullOrEmpty(line))
                {
                    logger.Warning("No command entered in Export Menu");
                    continue;
                }
                var args = line.Split(' ');
                if(args.Length == 0)
                {
                    logger.Warning("No command entered in Export Menu");
                    continue;
                }
                if (args[0] == "exit")
                {
                    logger.Information("Exiting Export Menu");
                    exitRecorded = true;
                    continue;
                }
                switch (args[1])
                {
                    case "json":
                        IExportStrategy exportStrategy = new JsonExportStrategyService();
                        exportStrategy.ExportDataToPath(args[2]);
                        break;
                    case "csv":
                        
                        break;
                    default:
                        logger.Warning($"Unknown export type '{line}' in Export Menu");
                        break;
                }
            } while (!exitRecorded);
        }

        public void ShowHelpAndExitMenu()
        {
            throw new NotImplementedException();
        }
    }
}
