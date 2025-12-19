using System;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Dtos;

public class BulkImportResultDto
{
    public Dictionary<ImportStatus,Result<FlightSchedule,Exception>?> ImportResults { get; set; } = null!;
    
    public enum ImportStatus
    {
        SUCCESS,
        FAILED
    }
}
