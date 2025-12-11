using ReadingList.src.Domain;
using ReadingList.src.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IExportService
    {
        Task<Result<ExportResult>> ExportReadingListAsync(ExportType type, string path);
    }
}
