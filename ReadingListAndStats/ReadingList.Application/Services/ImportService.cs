using ReadingList.Application.Interfaces;
using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Services
{
    public class ImportService(IImporter importer) : IImportService
    {
        public Task<Result<List<Book>>> ImportAsync(string[] files)
        {
            return importer.Import(files);
        }
    }
}
