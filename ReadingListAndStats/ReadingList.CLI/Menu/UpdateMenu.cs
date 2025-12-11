using ReadingList.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.CLI.Menu
{
    internal class UpdateMenu
    {
        private readonly IUpdateable updateService;
        public UpdateMenu(IUpdateable updateable)
        {
            Console.WriteLine();
            Console.WriteLine("=== Update menu initialized ===");
            Console.WriteLine("Please input one of the following commands:");
            Console.WriteLine(">mark finished <id>");
            Console.WriteLine(">rate <id> <0-5>");
            Console.WriteLine(">exit");
            Console.WriteLine();
            updateService = updateable;
        }

        public void Run()
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
                switch (inputCommand.ToLower().Split(" ").First())
                {
                    case "mark":
                        Console.WriteLine("Marking book as finished...");
                        string[] inputArgs = inputCommand.Split(' ');
                        if (inputArgs.Length != 3 || !int.TryParse(inputArgs[2], out int bookId))
                        {
                            Console.WriteLine("Invalid command format. Usage: mark finished <id>");
                            break;
                        }
                        var updateResult = updateService.MarkBookAsFinished(bookId);
                        if (updateResult.IsSuccess)
                        {
                            Console.WriteLine($"Book with ID {bookId} marked as finished.");
                            Console.WriteLine(updateResult.Value);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to mark book as finished: {updateResult.Error}");
                        }
                        break;
                    case "rate":
                        Console.WriteLine("Rating book...");
                        string[] rateArgs = inputCommand.Split(' ');
                        if (rateArgs.Length != 3 || !int.TryParse(rateArgs[1], out int rateBookId) || !int.TryParse(rateArgs[2], out int rating) || rating < 0 || rating > 5)
                        {
                            Console.WriteLine("Invalid command format. Usage: rate <id> <0-5>");
                            break;
                        }
                        var rateResult = updateService.RateBook(rateBookId, rating);
                        if (rateResult.IsSuccess)
                        {
                            Console.WriteLine($"Book with ID {rateBookId} rated {rating}.");
                            Console.WriteLine(rateResult.Value);
                        }
                        else
                        {
                            Console.WriteLine($"Failed to rate book: {rateResult.Error}");
                        }
                        break;
                    case "exit":
                        Console.Clear();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }

            } while (running);
        }
    }
}
