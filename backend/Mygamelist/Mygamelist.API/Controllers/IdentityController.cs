using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mygamelist.Core.Business;
using Mygamelist.Entity;
using Mygamelist.Identity;

namespace Mygamelist.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IAuthService _authService;
    public IdentityController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] IdentityRequest request)
    {
        try
        {
            User user = _authService.Authenticate(request.UserEmail, request.Password);
            string userRole = GetUserRole(request.UserEmail);
            var token = BuildToken(new IdentityUser {Id = user.Id, Email = user.Email}, userRole, TimeSpan.FromHours(1));
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("renew")]
    [Authorize] // Only allow authenticated users to renew their token
    public IActionResult Renew()
    {
        var email  = User.FindFirstValue(JwtRegisteredClaimNames.Email) 
                     ?? User.FindFirstValue(ClaimTypes.Email)!;
        var userId = User.FindFirstValue("userId")!;
        var role   = User.FindFirstValue("userRole")!;

        var user = new IdentityUser { Id = int.Parse(userId), Email = email};
        var token = BuildToken(user, role, TimeSpan.FromHours(1));

        return Ok(token);
    }


    /// <summary>
    /// Generates a JWT token for the specified user with a given role and validity duration.
    /// </summary>
    /// <param name="user">The user for whom the token is being generated.</param>
    /// <param name="userRole">The role assigned to the user, which will be included in the token claims.</param>
    /// <param name="duration">The duration for which the token will remain valid.</param>
    /// <returns>A string representation of the generated JWT token.</returns>
    /// <exception cref="Exception">Thrown if the environment variable "AUTH_KEY" is not set.</exception>
    private string BuildToken(IdentityUser user, string userRole, TimeSpan duration)
    {
        Console.WriteLine(user.Email, user.Id.ToString());
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("AUTH_KEY") ?? throw new Exception("AUTH_KEY_NOT_FOUND")));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub,   user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("userRole", userRole),
            new("userId",   user.Id.ToString()),
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Expires            = DateTime.UtcNow.Add(duration),
            Issuer             = "http://localhost:5131/",
            Audience           = "http://localhost:5131/",
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    private static string GetUserRole(string email) => email == "root@root.com" ? "admin" : "user";
}