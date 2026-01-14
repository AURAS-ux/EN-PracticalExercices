using System;
using AMT.Application.Dtos;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AMT.Application.Services;

public class AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger logger) : IAuthService
{
    public async Task<Result<LoginResultDto,Exception>> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
            if (user == null || !await userManager.CheckPasswordAsync(user, password))
            {
                return Result<LoginResultDto, Exception>.Failure(new List<string> { "Invalid email or password." },
                    new List<Exception>(){ new UnauthorizedAccessException() },
                    System.Net.HttpStatusCode.Unauthorized);
            }

            var roles = await userManager.GetRolesAsync(user);

            var jwtConfigs = configuration.GetSection("Jwt");
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(jwtConfigs["Key"]!);

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email!)
            };

            claims.AddRange(roles.Select(role => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)));

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtConfigs["Issuer"],
                Audience = jwtConfigs["Audience"],
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Result<LoginResultDto, Exception>.Success(new LoginResultDto(jwtToken));
    }

    public async Task RegisterAsync(string email, string password)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var newUser = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to register user.");
        }
        logger.Information("User registered successfully: {Email}", email);
    }
}
