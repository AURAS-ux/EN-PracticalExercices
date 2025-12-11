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
    public class CsvExportStrategy : IExportStrategy
    {
        public ExportType SupportedType { get; } = ExportType.CSV;

        public async Task<Result<string>> ExportAsync(List<Book> books, string filePath)
        {
            if(books == null || books.Count == 0)
            {
                return Result<string>.Fail("No books available to export.");
            }

            try
            {
                var csvLines = new List<string>
                {
                    "Id,Title,Author,Year,Pages,Genre,Finished,Rating"
                };
                foreach (var book in books)
                {
                    var line = $"{book.Id},\"{book.Title}\",\"{book.Author}\",{book.Year},{book.Pages},\"{book.Genre}\",{book.Finished},{book.Rating}";
                    csvLines.Add(line);
                }
                await File.WriteAllLinesAsync(filePath, csvLines);
                return Result<string>.Ok(filePath);
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Failed to export books to CSV: {ex.Message}");
            }
        }
    }
}
