using Microsoft.AspNetCore.Mvc;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;
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
        var users = _context.Users.ToList();
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
        //TODO: Validation des paramètres, mdp hash (Bcrypt), etc.
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
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