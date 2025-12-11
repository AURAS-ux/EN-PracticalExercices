using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using ReadingList.src.Infrastructure;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.App
{
    public static class AppUtils
    {
        private static readonly ILogger logger = AppLogger.Logger;
        public static async Task HandleImport(string[] args, IImportService importer, IRepository<int, Book> repository)
        {
            logger.Information("Import command received");
            var filesToImport = args.Skip(1).ToList();

            Result<List<Book>> importResult = await importer.Import(filesToImport.ToArray());

            if (importResult.IsSuccess && importResult.Value != null)
            {
                if (importResult.Value.Count == 1)
                {
                    var addedBook = repository.Add(importResult.Value[0].Id, importResult.Value[0]);
                    if (addedBook.IsSuccess)
                    {
                        logger.Information($"Successfully imported book: {addedBook.Value!.Title} by {addedBook.Value!.Author}");
                    }
                    else
                    {
                        logger.Error($"Failed to add imported book to repository: {addedBook.Error}");
                    }
                }
                else
                {
                    var keys = importResult.Value.Select(b => b.Id).ToArray();
                    var entities = importResult.Value.ToArray();
                    var addedBooksResult = await repository.AddMultipleAsync(keys, entities);
                    if (addedBooksResult.IsSuccess && addedBooksResult.Value != null)
                    {
                        logger.Information($"Successfully imported {addedBooksResult.Value.Count} books.");
                    }
                    else
                    {
                        logger.Error($"Failed to add imported books to repository: {addedBooksResult.Error}");
                    }
                }
            }
            else
            {
                logger.Error($"Failed to import book: {importResult.Error}");
            }

        }
        public static Stats ComputeUserStats(ref IRepository<int, Book> bookRepository)
        {
            int totalBooks = bookRepository.GetAllWhen(b => true).Value?.Count ?? 0;
            int finishedBooks = bookRepository.GetAllWhen(b => b.Finished).Value?.Count ?? 0;
            float avgRating = bookRepository.GetAllWhen(b => b.Finished).Value != null && bookRepository.GetAllWhen(b => b.Finished).Value!.Count > 0
                ? bookRepository.GetAllWhen(b => b.Finished).Value!.Average(b => b.Rating)
                : 0.0f; //TODO move as extention method
            var pagesByGenre = bookRepository.GetAllWhen(b => b.Finished).Value!
                                        .GroupBy(b => b.Genre)
                                        .Select(g => (Pages: g.Sum(b => b.Pages), Genre: g.Key))
                                        .OrderByDescending(pg => pg.Pages)
                                        .ToList();
            var topAuthorsByBookCount = bookRepository.GetAllWhen(b => true).Value!
                                        .GroupBy(b => b.Author)
                                        .Select(g => (Author: g.Key, BookCount: g.Count()))
                                        .OrderByDescending(a => a.BookCount)
                                        .Take(5)
                                        .ToList();
            var stats = new Stats
            {
                AvgRating = avgRating,
                FinishedBooks = finishedBooks,
                PagesByGenre = pagesByGenre,
                TotalBooks = totalBooks,
                TopAutorsByBookCount = topAuthorsByBookCount
            };
            return stats;
        }
    }
}
