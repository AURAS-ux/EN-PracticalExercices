using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Infrastructure.Persistance
{
    public class InMemoryRepository<Tkey, T>: IRepository<Tkey, T>
        where Tkey : notnull // Fix for CS8714
    {
        private readonly ConcurrentDictionary<Tkey, T> _readingList = new(); // Fix for IDE0044
        public Result<T> Add(Tkey key, T entity)
        {
            if (!_readingList.TryAdd(key, entity))
            {
                return Result<T>.Fail("Entity with the same key already exists.\n");
            }
            return Result<T>.Ok(entity);
        }

        public async Task<Result<List<T>>> AddMultipleAsync(Tkey[] keys, T[] entities)
        {
            if (keys == null || entities == null)
            {
                return Result<List<T>>.Fail("Keys or entities cannot be empty or null.");
            }

            if (keys.Length != entities.Length)
            {
                return Result<List<T>>.Fail("Keys and entities count do not match.");
            }
            List<Task<Result<T>>> tasks = keys.Select((key, index) => Task.Run(() => Add(key, entities[index]))).ToList();
            Result<T>[] results;
            try
            {
                results = await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                return Result<List<T>>.Fail($"An error occurred while adding entities: {ex.Message}");
            }

            var failures = results.Where(r => !r.IsSuccess).ToList();
            if (failures.Count > 0)
            {
                string combinedErrors = string.Join("; ", failures.Select(f => f.Error));
                return Result<List<T>>.Fail(combinedErrors);
            }

            var addedEntities = results.Where(result => result.IsSuccess)
                                        .Select(result => result.Value!)
                                        .ToList();
            return Result<List<T>>.Ok(addedEntities);
        }

        public Task<Result<T>> DeleteAsync(Tkey id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<T>>> GetAllAsync(CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested)
            {
                return Task.FromResult(Result<List<T>>.Fail("Operation was canceled."));
            }
            var items = _readingList.Values.ToList();
            return Task.FromResult(Result<List<T>>.Ok(items));
        }

        public Result<List<T>> GetAllWhen(Func<T, bool> queryPredicate)
        {
            var filteredItems = _readingList.Values
                                            .Where(item => queryPredicate(item))
                                            .ToList();
            if (filteredItems.Count == 0)
            {
                return Result<List<T>>.Fail("No items matched the given predicate.");
            }
            return Result<List<T>>.Ok(filteredItems);
        }

        public Result<T> GetById(Tkey id)
        {
            var exists = _readingList.TryGetValue(id, out T? entity);
            if (!exists || entity == null)
            {
                return Result<T>.Fail("Entity not found.");
            }
            return Result<T>.Ok(entity);
        }

        public Result<Book> Rate(Tkey id, int score)
        {
            T? entity = GetById(id).Value;
            if (entity == null)
            {
                return Result<Book>.Fail("Entity not found.");
            }
            if(entity is not Book book)
            {
                return Result<Book>.Fail("Entity is not of type Book.");
            }
            book.Rating = score;
            return Result<Book>.Ok(book);
        }

        public Result<Book> UpdateFinished(Tkey id)
        {
            T? entity = GetById(id).Value;
            if (entity == null)
            {
                return Result<Book>.Fail("Entity not found.");
            }
            if(entity is not Book book)
            {
                return Result<Book>.Fail("Entity is not of type Book.");
            }
            if(book.Finished)
            {
                return Result<Book>.Fail("Book is already marked as finished.");
            }
            book.Finished = true;
            return Result<Book>.Ok(book);
        }
    }
}
