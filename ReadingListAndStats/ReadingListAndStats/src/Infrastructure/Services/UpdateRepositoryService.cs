using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using Serilog;

namespace ReadingList.src.Infrastructure.Services
{
    public class UpdateRepositoryService<TKey, T> : IUpdateRepositoryService<TKey, T> where TKey : notnull
    {
        private readonly ILogger logger = AppLogger.Logger;
        public Result<T> RateAsync(TKey id, int rating, ref IRepository<TKey, T> repository)
        {
            var bookResult = repository.GetById(id);
            if (!bookResult.IsSuccess || bookResult.Value == null)
            {
                return Result<T>.Fail("Entity not found for rating.");
            }

            if (!(bookResult.Value is Book bookToRate))
            {
                return Result<T>.Fail($"Id:{id} does not belong to a book");
            }

            bookToRate.Rating = rating;
            logger.Information($"Book '{bookToRate.Title}' rated with {rating} stars.");
            return Result<T>.Ok((T)(object)bookToRate);
        }

        public Result<T> UpdateFinished(TKey id, ref IRepository<TKey, T> repository)
        {
            var bookResult = repository.GetById(id);
            if (!bookResult.IsSuccess || bookResult.Value == null)
            {
                return Result<T>.Fail("Entity not found for updating finished status.");
            }
            if (!(bookResult.Value is Book bookToUpdate))
            {
                return Result<T>.Fail($"Id:{id} does not belong to a book");
            }
            bookToUpdate.Finished = true;
            logger.Information($"Book '{bookToUpdate.Title}' marked as finished.");
            return Result<T>.Ok((T)(object)bookToUpdate);
        }
    }
}
