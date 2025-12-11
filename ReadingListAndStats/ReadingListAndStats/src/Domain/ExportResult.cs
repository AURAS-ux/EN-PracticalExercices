using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain
{
    public struct ExportResult
    {
        public string ExportPath { get; set; }
        public ExportedType ExportedType { get; set; }
        public string? Error { get; set; }
    }
}
