using ReadingList.src.Domain;
using ReadingList.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Infrastructure.Services
{
    public class JsonExportStrategyService : IExportStrategy
    {
        public Result<ExportResult> ExportDataToPath(string exportPath)
        {
            throw new NotImplementedException();
        }
    }
}
