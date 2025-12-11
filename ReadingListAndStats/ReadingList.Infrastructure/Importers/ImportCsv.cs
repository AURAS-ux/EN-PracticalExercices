using ReadingList.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;

namespace ReadingList.Application.Services
{
    public class ImportCsv(IParser<Book> parser,IRepository<int,Book> repository) : IImporter
    {
        public static string DataFolder
        {
            get
            {
                var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (dir != null && !dir.GetFiles("*.sln").Any())
                    dir = dir.Parent;

                if (dir == null)
                    throw new DirectoryNotFoundException("Solution directory not found.");

                return dir.FullName;
            }
        }

        public async Task<Result<List<Book>>> Import(string[] files)
        {
            Result<List<Book>> importResult;
            if (files == null || files.Length == 0)
            {
                return Result<List<Book>>.Fail("No files provided for import.");
            }
            switch (files.Length == 1)
            {
                case true:
                    importResult = await ImportSingleFileAsync(files[0]);
                    if(importResult.IsSuccess && importResult.Value != null)
                    {
                        var addResult = await repository.AddMultipleAsync(importResult.Value.Select(b => b.Id).ToArray(), importResult.Value.ToArray());
                        if (!addResult.IsSuccess)
                        {
                            return Result<List<Book>>.Fail($"Failed to add imported books to repository: {addResult.Error}");
                        }
                        return Result<List<Book>>.Ok(addResult.Value!);
                    }
                    break;
                case false:
                    importResult = await ImportMultipleFilesAsync(files);
                    if(importResult.IsSuccess && importResult.Value != null)
                    {
                        var addResult = await repository.AddMultipleAsync(importResult.Value.Select(b => b.Id).ToArray(), importResult.Value.ToArray());
                        if (!addResult.IsSuccess)
                        {
                            return Result<List<Book>>.Fail($"Failed to add imported books to repository: {addResult.Error}");
                        }
                        return Result<List<Book>>.Ok(addResult.Value!);
                    }
                    break;
            }
            return importResult;
        }

        private async Task<Result<List<Book>>> ImportMultipleFilesAsync(string[] fileName, CancellationToken ct = default)
        {
            if (fileName == null || fileName.Length == 0)
            {
                return Result<List<Book>>.Fail("No files provided for import.");
            }
            List<string> paths = new List<string>();
            foreach (var file in fileName)
            {
                string filePath = Path.Combine(DataFolder, file.EndsWith(".csv") ? file : file + ".csv");
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
            return Result<List<Book>>.Ok(booksImported);
        }

        private async Task<Result<List<Book>>> ImportSingleFileAsync(string fileName)
        {
            string filePath = Path.Combine(DataFolder, fileName.EndsWith(".csv") ? fileName : fileName + "csv");
            if (!File.Exists(filePath))
            {
                return Result<List<Book>>.Fail($"File '{fileName}' not found in data folder.");
            }
            string[] lines = await File.ReadAllLinesAsync(filePath);
            var parseResult = parser.Parse(lines, true);

            if (!parseResult.IsSuccess)
            {
                return Result<List<Book>>.Fail(parseResult.Error!);
            }else if(parseResult.Value == null || parseResult.Value.Count == 0)
            {
                return Result<List<Book>>.Fail("No books could be callected.");
            }

            return Result<List<Book>>.Ok(parseResult.Value!);
        }
    }
}
