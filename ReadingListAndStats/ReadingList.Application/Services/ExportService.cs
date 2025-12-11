using ReadingList.Application.Interfaces;
using ReadingList.src.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Enums;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Services
{
    public class ExportService(IEnumerable<IExportStrategy> exportStrategies, IRepository<int, Book> repository) : IExportService
    {
        public async Task<Result<ExportResult>> ExportReadingListAsync(ExportType type, string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return Result<ExportResult>.Fail("The specified path is invalid.");
            }
            path += type == ExportType.CSV ? ".csv" : ".json";
            var books = await repository.GetAllAsync();
            if (!books.IsSuccess || books.Value == null)
            {
                return Result<ExportResult>.Fail("Failed to retrieve books from the repository.");
            }
            var strategy = exportStrategies.FirstOrDefault(s => s.SupportedType == type);
            if (strategy == null)
            {
                return Result<ExportResult>.Fail("No export strategy found for the specified type.");
            }
            var exportResult = await strategy.ExportAsync(books.Value!, path);
            if (!exportResult.IsSuccess)
            {
                return Result<ExportResult>.Fail(exportResult.Error!);
            }
            return Result<ExportResult>.Ok(new ExportResult { IsSuccessful = true });
        }
    }
}
