using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace ReadingList.src.Infrastructure.Services
{
    public class CsvImporterService : IImportService
    {
        private readonly string pathToDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");

        private async Task<Result<List<Book>>> ImportMultipleFilesAsync(string[] fileName, CancellationToken ct = default)
        {
            if (fileName == null || fileName.Length == 0)
            {
                return Result<List<Book>>.Fail("No files provided for import.");
            }
            List<string> paths = new List<string>();
            foreach (var file in fileName)
            {
                string filePath = Path.Combine(pathToDataFolder, file.EndsWith(".csv") ? file : file + ".csv");
                if (!File.Exists(filePath))
                {
                    return Result<List<Book>>.Fail($"File '{file}' not found in data folder.");
                }
                paths.Add(filePath);
            }

            var tasks = paths.Select(file => ImportSingleFileAsync(file));
            Result<List<Book>>[] results;
            try
            {
                results = await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                return Result<List<Book>>.Fail("Import operation was canceled.");
            }

            var failures = results.Where(r => !r.IsSuccess).ToList();
            if (failures.Count > 0)
            {
                string combinedErrors = string.Join("; ", failures.Select(f => f.Error));
                return Result<List<Book>>.Fail(combinedErrors);
            }

            var booksImported = results.Where(result => result.IsSuccess)
                                        .SelectMany(result => result.Value!)
                                        .ToList();
            return  Result<List<Book>>.Ok(booksImported);
        }

        private async Task<Result<List<Book>>> ImportSingleFileAsync(string fileName)
        {
            string filePath = Path.Combine(pathToDataFolder, fileName.EndsWith(".csv") ? fileName : fileName+"csv");
            if (!File.Exists(filePath))
            {
                return Result<List<Book>>.Fail($"File '{fileName}' not found in data folder.");
            }
            string[] lines = await File.ReadAllLinesAsync(filePath);
            List<Book> books = CsvBookParserService.ParseCsvLines(lines, true, out string? error);

            if (error != null)
            {
                return Result<List<Book>>.Fail("Not implemented yet");
            }

            return Result<List<Book>>.Ok(books);
        }

        public async Task<Result<List<Book>>> Import(string[] files)
        {
            if (files.Length == 1)
            {
                return await ImportSingleFileAsync(files[0]);
            }
            else
            {
                return await ImportMultipleFilesAsync(files);
            }
        }
    }
}