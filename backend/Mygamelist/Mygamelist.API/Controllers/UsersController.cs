using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;
using Mygamelist.Core;
using Mygamelist.Core.Business;

namespace Mygamelist.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    // GET: api/users
    [HttpGet("")]
    public IActionResult GetUsers()
    {
        var users = _userService.RetrieveAll();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetUser(int id)
    {
        
        try
        {
            var user = _userService.RetrieveById(id);
            if (user == null)
                return NotFound(new { error = "USER_NOT_FOUND" });
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "INTERNAL_ERROR" });
        }
    }

    // POST: api/users
    [HttpPost("")]
    public async Task<IActionResult> CreateUser(User user)
    {
        // Validation email
        if (!Utiles.Utiles.IsValidEmail(user.Email))
            return BadRequest(new { error = "INVALID_EMAIL" });

        // Validation mot de passe
        if (!Utiles.Utiles.IsValidPassword(user.PasswordHash))
            return BadRequest(new { error = "INVALID_PASSWORD" });

        // Vérification unicité email et pseudo AVANT insertion
        bool emailExists = _userService.EmailExists(user.Email);
        if (emailExists)
            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });

        bool pseudoExists = _userService.PseudoExists(user.Pseudo);
        if (pseudoExists)
            return Conflict(new { error = "USERNAME_ALREADY_EXISTS" });

        // Hashage du mot de passe
        user.PasswordHash = Utiles.Utiles.HashPassword(user.PasswordHash);

        // Insertion
        try
        {
            _userService.Add(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                user.Id,
                user.Pseudo,
                user.Email
            });
        }
        catch (DbUpdateException ex)
        {
            // doublon inséré entre la vérif et le SaveChanges
            if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                return Conflict(new { error = "DUPLICATE_FIELD" });

            return StatusCode(500, new { error = "DATABASE_ERROR" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "INTERNAL_ERROR" });
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id:int:min(1)}")]
    public IActionResult UpdateUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    
    // PATCH: api/users/{id}
    [HttpPatch("{id:int:min(1)}")]
    public IActionResult UpdateUserPartial(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur partiellement mis à jour" });
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:int:min(1)}")]
    public IActionResult DeleteUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur supprimé" });
    }


}