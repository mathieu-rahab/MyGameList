using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;
using Mygamelist.Utiles;

namespace Mygamelist.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: api/users
    [HttpGet("")]
    public IActionResult GetUsers()
    {
        var users = _context.Users
            .Select(u => new { u.Id, u.Pseudo, u.Email, u.SteamId, u.ProfilePicturePath })
            .ToList();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur trouvé" });
    }

    // POST: api/users
    [HttpPost("")]
    public async Task<IActionResult> CreateUser(User user)
    {
        // 1. Validation email
        if (!Utiles.Utiles.IsValidEmail(user.Email))
            return BadRequest(new { error = "INVALID_EMAIL" });

        // 2. Validation mot de passe
        if (!Utiles.Utiles.IsValidPassword(user.PasswordHash))
            return BadRequest(new { error = "INVALID_PASSWORD" });

        // 3. Vérification unicité email et pseudo AVANT insertion
        bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
        if (emailExists)
            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });

        bool pseudoExists = await _context.Users.AnyAsync(u => u.Pseudo == user.Pseudo);
        if (pseudoExists)
            return Conflict(new { error = "USERNAME_ALREADY_EXISTS" });

        // 4. Hashage du mot de passe
        user.PasswordHash = Utiles.Utiles.HashPassword(user.PasswordHash);

        // Insertion
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    
    // PATCH: api/users/{id}
    [HttpPatch("{id}")]
    public IActionResult UpdateUserPartial(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur partiellement mis à jour" });
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur supprimé" });
    }


}