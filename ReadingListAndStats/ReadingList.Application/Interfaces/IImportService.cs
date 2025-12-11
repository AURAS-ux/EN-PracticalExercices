using ReadingList.src.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IImportService
    {
        public Task<Result<List<Book>>> ImportAsync(string[] files);
    }
}
