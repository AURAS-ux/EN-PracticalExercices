using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain.Interfaces
{
    public interface IUpdateRepositoryService<Tkey, T> where Tkey : notnull
    {
        public Result<T> UpdateFinished(Tkey id, ref IRepository<Tkey,T> repository);
        public Result<T> RateAsync(Tkey id, int rating, ref IRepository<Tkey,T> repository);
    }
}
