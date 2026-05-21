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
            // Vérification de l'authentification
            User user =_authService.Authenticate(request.UserEmail, request.Password);

            string userRole = GetUserRole(request.UserEmail);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var value = Environment.GetEnvironmentVariable("AUTH_KEY") ?? throw new Exception("INTERNAL_ERROR") ;
            var key = Encoding.UTF8.GetBytes(value);
            
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.UserEmail),
            new(JwtRegisteredClaimNames.Email, request.UserEmail),
            new("userRole", userRole),
            new("userId", user.Id.ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(50)),
                Issuer = "http://localhost:5131/",
                Audience = "http://localhost:5131/",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            return Ok(jwt);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    private static string GetUserRole(string userEmail) => userEmail == "root@root.com" ? "admin" : "user";
}