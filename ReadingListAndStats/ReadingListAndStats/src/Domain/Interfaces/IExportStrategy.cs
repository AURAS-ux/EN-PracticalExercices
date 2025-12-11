using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain.Interfaces
{
    public interface IExportStrategy
    {
        public Result<ExportResult> ExportDataToPath(string exportPath);
    }
}
