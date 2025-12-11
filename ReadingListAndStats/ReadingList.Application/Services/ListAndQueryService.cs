using ReadingList.Application.Interfaces;
using ReadingList.Domain.Entities;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Services
{
    public class ListAndQueryService(IRepository<int, Book> repository) : IQueryService
    {
        public async Task<Result<List<Book>>> ListAllBooks()
        {
            var books = await repository.GetAllAsync();
            if(!books.IsSuccess || books.Value == null)
            {
                return Result<List<Book>>.Fail("Failed to retrieve books.");
            }
            return Result<List<Book>>.Ok(books.Value!);
        }

        public Result<List<Book>> ListBooksByAuthor(string author)
        {
            var books = repository.GetAllWhen(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase));
            if (!books.IsSuccess || books.Value == null)
            {
                return Result<List<Book>>.Fail("Failed to retrieve books by author.");
            }
            return Result<List<Book>>.Ok(books.Value!);
        }

        public Result<List<Book>> ListFinishedBooks()
        {
            var books = repository.GetAllWhen(b => b.Finished);
            if (!books.IsSuccess || books.Value == null)
            {
                return Result<List<Book>>.Fail("Failed to retrieve finished books.");
            }
            return Result<List<Book>>.Ok(books.Value!);
        }

        public Result<List<Book>> ListTopNBooks(int n)
        {
            if(n <= 0)
            {
                return Result<List<Book>>.Fail("N must be a positive integer.");
            }
            var books = repository.GetAllWhen(b => b.Finished);
            if(!books.IsSuccess || books.Value == null)
            {
                return Result<List<Book>>.Fail("Failed to retrieve finished books.");
            }
            var topNBooks = books.Value!
                .OrderByDescending(b => b.Rating)
                .Take(n)
                .ToList();
            return Result<List<Book>>.Ok(topNBooks);
        }

        public Result<List<Book>> ListUnfinishedBooks()
        {
            var unfinishedBooks = repository.GetAllWhen(b => !b.Finished);
            if (!unfinishedBooks.IsSuccess || unfinishedBooks.Value == null)
            {
                return Result<List<Book>>.Fail("Failed to retrieve unfinished books.");
            }
            return Result<List<Book>>.Ok(unfinishedBooks.Value!);
        }

        public async Task<Result<ReadingStats>> ViewStatistics()
        {
            var allBooksResult = await repository.GetAllAsync();
            if (!allBooksResult.IsSuccess || allBooksResult.Value == null)
            {
                return Result<ReadingStats>.Fail("Failed to retrieve books for statistics.");
            }
            var allBooks = allBooksResult.Value!;
            int totalBooks = allBooks.Count;
            int finishedBooks = allBooks.Count(b => b.Finished);
            float averageRating = finishedBooks == 0 ? 0 : allBooks.Where(b => b.Finished).Average(b => b.Rating);
            int pagesByGenreCount = allBooks
                .GroupBy(b => b.Genre)
                .Select(g => g.Sum(b => b.Pages))
                .Sum();
            List<(string,string)> topAuthors = allBooks
                .GroupBy(b => b.Author)
                .Select(g => (Author: g.Key, BooksCount: g.Count(b => true)))
                .OrderByDescending(a => a.BooksCount)
                .Take(5)
                .Select(a => (a.Author, a.BooksCount.ToString()))
                .ToList();
            return Result<ReadingStats>.Ok(new ReadingStats
            {
                TotalBooks = totalBooks,
                FinishedBooks = finishedBooks,
                UnfinishedBooks = totalBooks - finishedBooks,
                AverageRating = averageRating,
                TotalPages = pagesByGenreCount,
                TopAuthors = topAuthors.Select(a => new AuthorStatistic
                {
                    AuthorName = a.Item1,
                    BookCount = int.Parse(a.Item2)
                }).ToList()
            });
        }
    }
}
