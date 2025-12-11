using ReadingList.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.CLI.Menu
{
    public class ListMenu
    {
        private readonly IQueryService queryService;
        public ListMenu(IQueryService service)
        {
            Console.WriteLine();
            Console.WriteLine("=== Entered Profile Menu ===");
            Console.WriteLine("=== Available options ===");
            Console.WriteLine("Please input of of the following commands");
            Console.WriteLine(">list-books(lb)");
            Console.WriteLine(">list-books-finished(lbf)");
            Console.WriteLine(">list-books-unfinished(lbu)");
            Console.WriteLine(">list-top-n-books(lt)");
            Console.WriteLine(">list-books-by-author(lba)");
            Console.WriteLine(">view-stats(vs)");
            Console.WriteLine(">exit(e)");
            Console.WriteLine("=========================");
            this.queryService = service;
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
                switch (inputCommand.ToLower())
                {
                    case "list-books":
                    case "lb":
                        Console.WriteLine("Listing all books...");
                        var listingResult = await queryService.ListAllBooks();
                        if (!listingResult.IsSuccess || listingResult.Value! == null || listingResult.Value!.Count == 0)
                        {
                            throw new Exception("Error in listing books");
                        }
                        foreach (var book in listingResult.Value!)
                        {
                            Console.WriteLine(book);
                        }
                        Console.WriteLine();
                        break;
                    case "list-books-finished":
                    case "lbf":
                        Console.WriteLine("Listing finished books...");
                        var finishedListingResult = queryService.ListFinishedBooks();
                        if (!finishedListingResult.IsSuccess || finishedListingResult.Value! == null || finishedListingResult.Value!.Count == 0)
                        {
                            throw new Exception("Error in listing finished books");
                        }
                        foreach (var book in finishedListingResult.Value!)
                        {
                            Console.WriteLine(book);
                        }
                        Console.WriteLine();
                        break;
                    case "list-books-unfinished":
                    case "lbu":
                        Console.WriteLine("Listing unfinished books...");
                        var unfinishedListingResult = queryService.ListUnfinishedBooks();
                        if (!unfinishedListingResult.IsSuccess || unfinishedListingResult.Value! == null || unfinishedListingResult.Value!.Count == 0)
                        {
                            throw new Exception("Error in listing unfinished books");
                        }
                        foreach (var book in unfinishedListingResult.Value!)
                        {
                            Console.WriteLine(book);
                        }
                        Console.WriteLine();
                        break;
                    case "list-top-n-books":
                    case "lt":
                        Console.WriteLine("Listing top N books...");
                        try
                        {
                            TopNMenuHandler();
                        }
                        catch
                        {
                            throw;
                        }
                            break;
                    case "list-books-by-author":
                    case "lba":
                        Console.WriteLine("Listing books by author...");
                        try
                        {
                            ListByAuthorMenuHandler();
                        }
                        catch
                        {
                            throw;
                        }
                        break;
                    case "view-stats":
                    case "vs":
                        Console.WriteLine("Viewing statistics...");
                        var statsResult = await queryService.ViewStatistics();
                        if (!statsResult.IsSuccess || statsResult.Value == null)
                        {
                            throw new Exception("Error in viewing statistics");
                        }
                        Console.WriteLine(statsResult.Value);
                        Console.WriteLine();
                        break;
                    case "exit":
                    case "e":
                        Console.Clear();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
            while (running);
        }

        private void ListByAuthorMenuHandler()
        {
            string authorInput = Console.ReadLine()!;
            var byAuthorResult = queryService.ListBooksByAuthor(authorInput);
            if (!byAuthorResult.IsSuccess || byAuthorResult.Value! == null || byAuthorResult.Value!.Count == 0)
            {
                throw new Exception("Error in listing books by author");
            }
            foreach (var book in byAuthorResult.Value!)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine();
        }

        private void TopNMenuHandler()
        {
            string nInput = Console.ReadLine()!;
            if (!int.TryParse(nInput, out int n) || n <= 0)
            {
                Console.WriteLine("Invalid number. Please enter a positive integer.");
                return;
            }
            var topNResult = queryService.ListTopNBooks(n);
            if (!topNResult.IsSuccess || topNResult.Value! == null || topNResult.Value!.Count == 0)
            {
                throw new Exception("Error in listing top N books");
            }
            foreach (var book in topNResult.Value!)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine();
        }
    }
}
