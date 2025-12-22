using System;
using System.Threading.Tasks;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Domain.Utils;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using Serilog;

namespace AMT.Application.Services;

public class GateService(IUnitOfWork unitOfWork, ILogger logger) : IGateService
{
    public async Task<Result<Gate, Exception>> CreateGate(GateCreateRequestDto gateCreateRequestDto)
    {
        var airport = unitOfWork.Airports.GetById(gateCreateRequestDto.AirportId);
        if (airport is null)
        {
            logger.Warning("Attempted to create gate for non-existent airport with ID {AirportId}", gateCreateRequestDto.AirportId);
            return Result<Gate, Exception>.Failure(new List<string> { "Airport does not exist." },
                new List<Exception> { new GenericNotFound<Airport,int>(gateCreateRequestDto.AirportId) },
                System.Net.HttpStatusCode.NotFound);
        }
        var newGate = new Gate
        {
            Code = gateCreateRequestDto.Code,
            Airport = airport
        };
        await unitOfWork.Gates.AddAsync(newGate);
        await unitOfWork.SaveChangesAsync();
        logger.Information("Created new gate with ID {GateId} for airport ID {AirportId}", newGate.Id, gateCreateRequestDto.AirportId);
        return Result<Gate, Exception>.Success(newGate);
    }

    public Result<List<Gate>, Exception> GetAllGates()
    {
        var gates = unitOfWork.Gates.GetAll().ToList();
        return Result<List<Gate>, Exception>.Success(gates);
    }

    public Result<Gate, Exception> GetGateById(int id)
    {
        var gate = unitOfWork.Gates.GetById(id);
        if (gate is null)
        {
            logger.Warning("Gate with ID {GateId} not found", id);
            return Result<Gate, Exception>.Failure(new List<string> { "Gate not found." },
                new List<Exception> { new GenericNotFound<Gate,int>(id) },
                System.Net.HttpStatusCode.NotFound);
        }
        return Result<Gate, Exception>.Success(gate);
    }
}
