using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mygamelist.Identity;
using Mygamelist.Utiles;

namespace Mygamelist.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] IdentityRequest request)
    {
        try
        {
            if (request.UserEmail != "root@root.com" && request.Password != "toto" )
            {
                throw new UnauthorizedAccessException("username or password are not valid");
            }

            string userRole = GetUserRole(request.UserEmail);

            var tokenHandler = new JwtSecurityTokenHandler();
            
            EnvReader.Load(".env");
            
            string value = Environment.GetEnvironmentVariable("API_KEY");

            var key = Encoding.UTF8.GetBytes(value);
            
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.UserEmail),
            new(JwtRegisteredClaimNames.Email, request.UserEmail),
            new("userRole", userRole)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
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

    private static string GetUserRole(string userEmail) => userEmail == "root@root.com" ? "admin" : "reader";
}