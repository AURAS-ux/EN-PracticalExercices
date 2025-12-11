using ReadingList.Domain.Entities;
using ReadingList.src.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IQueryService
    {
        Task<Result<List<Book>>> ListAllBooks();
        Result<List<Book>> ListFinishedBooks();
        Result<List<Book>> ListUnfinishedBooks();
        Result<List<Book>> ListTopNBooks(int n);
        Result<List<Book>> ListBooksByAuthor(string author);
        Task<Result<ReadingStats>> ViewStatistics();
    }
}
