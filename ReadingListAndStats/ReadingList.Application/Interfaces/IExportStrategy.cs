using ReadingList.src.Domain;
using ReadingList.src.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Application.Interfaces
{
    public interface IExportStrategy
    {
        ExportType SupportedType { get; }
        Task<Result<string>> ExportAsync(List<Book> books, string filePath);
    }
}
