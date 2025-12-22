using System;
using AMT.Application.Dtos;
using AMT.Domain.Models;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IGateService
{
    Result<Gate, Exception> GetGateById(int id);
    Task<Result<Gate, Exception>> CreateGate(GateCreateRequestDto gateCreateRequestDto);
    Result<List<Gate>, Exception> GetAllGates();
}
