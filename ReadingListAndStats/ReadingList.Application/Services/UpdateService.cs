using ReadingList.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Services
{
    public class UpdateService(IRepository<int, Book> repository) : IUpdateable
    {
        public Result<Book> MarkBookAsFinished(int bookId)
        {
            var updateResult = repository.UpdateFinished(bookId);
            if(!updateResult.IsSuccess || updateResult.Value == null)
            {
                return Result<Book>.Fail(updateResult.Error ?? "Unknown error occurred while marking book as finished.");
            }
            return Result<Book>.Ok(updateResult.Value!);
        }

        public Result<Book> RateBook(int bookId, int rating)
        {
            var rateResult = repository.Rate(bookId, rating);
            if(!rateResult.IsSuccess || rateResult.Value == null)
            {
                return Result<Book>.Fail(rateResult.Error ?? "Unknown error occurred while rating the book.");
            }
            return Result<Book>.Ok(rateResult.Value!);
        }
    }
}
