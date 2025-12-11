using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain.Interfaces
{
    public interface IRepository<Tkey, T>
    {
        public Result<T> GetById(Tkey id);
        public Task<Result<List<T>>> GetAllAsync(CancellationToken ct = default);
        public Result<List<T>> GetAllWhen(Func<T, bool> queryPredicate);
        public Result<T> Add(Tkey key,T entity);
        public Task<Result<List<T>>> AddMultipleAsync(Tkey[] keys, T[] entities);
        public Task<Result<T>> UpdateFinishedAsync(Tkey id);
        public Task<Result<T>> RateAsync(Tkey id);
        public Task<Result<T>> DeleteAsync(Tkey id);
    }
}
