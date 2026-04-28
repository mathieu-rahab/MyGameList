using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Entity;
using Mygamelist.Core.Business;

namespace Mygamelist.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    // GET: api/users
    [Authorize(Policy = "isAdmin")]
    [HttpGet]
    [Route("")]
    [EndpointName("GetUsers")]
    [ActionName("GetUsers")]
    public IActionResult GetUsers()
    {
        var users = _userService.RetrieveAll();
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet]
    [Route("{id:int:min(1)}")]
    [EndpointName("GetUser")]
    [ActionName("GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, id)) return Forbid();
        try
        {
            var user = _userService.RetrieveById(id);
            if (user == null)
                return NotFound(new { error = "USER_NOT_FOUND" });
            return Ok(user);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "INTERNAL_ERROR" });
        }
    }

    // POST: api/users
    [AllowAnonymous]
    [HttpPost]
    [Route("")]
    [EndpointName("CreateUser")]
    [ActionName("CreateUser")]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        // Validation Pseudo
        if (!Utiles.UserSystems.IsValidPseudo(dto.Pseudo))
            return BadRequest(new { error = "INVALID_PSEUDO" });

        // Validation email
        if (!Utiles.UserSystems.IsValidEmail(dto.Email))
            return BadRequest(new { error = "INVALID_EMAIL" });

        // Validation mot de passe
        if (!Utiles.UserSystems.IsValidPassword(dto.Password))
            return BadRequest(new { error = "INVALID_PASSWORD" });

        // Vérification unicité email et pseudo AVANT insertion
        bool emailExists = _userService.EmailExists(dto.Email);
        if (emailExists)
            return Conflict(new { error = "EMAIL_ALREADY_EXISTS" });

        bool pseudoExists = _userService.PseudoExists(dto.Pseudo);
        if (pseudoExists)
            return Conflict(new { error = "USERNAME_ALREADY_EXISTS" });
        
        // Insertion
        try
        {
            var createdUser = _userService.Add(dto);
            return Ok(createdUser);

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
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateUser")]
    [ActionName("UpdateUser")]
    public IActionResult UpdateUser(int id)
    {
        // TODO
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    
    // PATCH: api/users/{id}
    [HttpPatch]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateUserPartial")]
    [ActionName("UpdateUserPartial")]
    public IActionResult UpdateUserPartial(int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, id)) return Forbid();
        // TODO (PATCH facultatif)
        return Ok(new { Id = id, Message = "Utilisateur partiellement mis à jour" });
    }

    // DELETE: api/users/{id}
    [HttpDelete]
    [Route("{id:int:min(1)}")]
    [EndpointName("DeleteUser")]
    [ActionName("DeleteUser")]
    public IActionResult DeleteUser(int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, id)) return Forbid();
        try
        {
            var user = _userService.RetrieveById(id);
            if (user == null)
                return NotFound(new { error = "USER_NOT_FOUND" });
            _userService.Remove(id);
            return Ok(id);
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "INTERNAL_ERROR" });
        }
    }


}