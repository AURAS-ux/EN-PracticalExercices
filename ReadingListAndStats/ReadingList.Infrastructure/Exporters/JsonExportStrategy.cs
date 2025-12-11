using ReadingList.src.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Infrastructure.Exporters
{
    public class JsonExportStrategy : IExportStrategy
    {
        public ExportType SupportedType { get; } = ExportType.JSON;

        public async Task<Result<string>> ExportAsync(List<Book> books, string filePath)
        {
            if(books == null || books.Count == 0)
            {
                return Result<string>.Fail("No books available to export.");
            }
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(books, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(filePath, json);
                return Result<string>.Ok(filePath);
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Failed to export books to JSON: {ex.Message}");
            }
        }
    }
}
