using Microsoft.AspNetCore.Mvc;

namespace Mygamelist.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // GET: api/users
    [HttpGet("")]
    public IActionResult GetUsers()
    {
        return Ok(new { Message = "Liste des utilisateurs" });
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        return Ok(new { Id = id, Message = "Utilisateur trouvé" });
    }

    // POST: api/users
    [HttpPost("")]
    public IActionResult CreateUser()
    {
        return Created($"api/users/1", new { Id = 1, Message = "Utilisateur créé" });
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