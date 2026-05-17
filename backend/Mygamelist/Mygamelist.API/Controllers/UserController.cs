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
        var user = _userService.RetrieveById(id);
        return Ok(user);
    }

    // POST: api/users
    [AllowAnonymous]
    [HttpPost]
    [Route("")]
    [EndpointName("CreateUser")]
    [ActionName("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
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

        var createdUser = _userService.Add(dto);
        return Ok(createdUser);
    }

    // PUT: api/users/{id}
    /*
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateUser")]
    [ActionName("UpdateUser")]
    public IActionResult UpdateUser(int id)
    {
        // TODO
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    */
    
    // PATCH: api/users/{id}
    [HttpPatch]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateUser")]
    [ActionName("UpdateUser")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, id)) return Forbid();

        if (dto.Pseudo is not null && !Utiles.UserSystems.IsValidPseudo(dto.Pseudo))
            return BadRequest(new { error = "INVALID_PSEUDO" });

        if (dto.Email is not null && !Utiles.UserSystems.IsValidEmail(dto.Email))
            return BadRequest(new { error = "INVALID_EMAIL" });

        var updatedUser = _userService.Update(id, dto);
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    
    // DELETE: api/users/{id}
    [HttpDelete]
    [Route("{id:int:min(1)}")]
    [EndpointName("DeleteUser")]
    [ActionName("DeleteUser")]
    public IActionResult DeleteUser(int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, id)) return Forbid();
        _userService.Remove(id);
        return Ok(id);
    }


}