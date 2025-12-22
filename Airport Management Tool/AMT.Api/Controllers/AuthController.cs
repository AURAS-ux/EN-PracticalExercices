using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AMT.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            var newUser = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("Invalid email or password.");
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

            return Ok(new { Token = jwtToken });
        }
    }
}
