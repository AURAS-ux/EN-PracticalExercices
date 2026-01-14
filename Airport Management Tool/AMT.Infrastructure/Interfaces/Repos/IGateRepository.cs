using System;
using AMT.Domain.Models;

namespace AMT.Infrastructure.Interfaces.Repos;

public interface IGateRepository : IGenericRepository<Gate>
{
    Gate? GetByGateCode(string gateCode);
}
