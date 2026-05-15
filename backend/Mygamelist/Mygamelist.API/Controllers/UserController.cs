using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Entity;
using Mygamelist.Core.Business;
using Mygamelist.Contracts.Hateos;
using Mygamelist.Hateos;

namespace Mygamelist.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IHateosLinkGenerator _hateosLinkGenerator;
    public UserController(IUserService userService, IHateosLinkGenerator hateosLinkGenerator)
    {
        _userService = userService;
        _hateosLinkGenerator = hateosLinkGenerator;
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
        foreach (UserResponseDto user in users)
        {
            AddHateosLinks(user);
        }
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

        var createdUser = _userService.Add(dto);
        return Ok(createdUser);
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
        _userService.Remove(id);
        return Ok(id);
    }

    private void AddHateosLinks(UserResponseDto user)
    {
        user.Links.AddRange(new List<Link> {
            _hateosLinkGenerator.Generate(
                "GetUser",
                new {id = user.Id },
                "self",
                "GET"),
            _hateosLinkGenerator.Generate(
                "UpdateUser",
                new { id = user.Id },
                "update-user",
                "PUT"),
            _hateosLinkGenerator.Generate(
                "DeleteUser",
                new {id = user.Id
                },
                "delete-user",
                "DELETE")
        });
    }
}