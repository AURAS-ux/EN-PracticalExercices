using System;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Dtos;

public class BulkImportResultDto
{
    public Dictionary<Guid, ImportResult> ImportResults { get; set; } = null!;
    
    public record ImportResult(ImportStatus Status, Result<FlightSchedule,Exception>? Result);

    public enum ImportStatus
    {
        SUCCESS,
        FAILED
    }
}
