using System;
using AMT.Application.Dtos;
using AMT.Domain.Utils;

namespace AMT.Application.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(string email, string password);
    Task<Result<LoginResultDto, Exception>> LoginAsync(string email, string password);
}
