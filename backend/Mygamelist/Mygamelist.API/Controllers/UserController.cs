using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Mygamelist.Contracts.DTOs.Steam;
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
        AddHateosLinks(user);
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
    
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [EndpointName("ResetUser")]
    [ActionName("ResetUser")]
    public IActionResult ResetUser(int id, [FromBody] ResetUserDto dto)
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

        var resetUser = _userService.Reset(id, dto);
        return Ok(new { Id = id, Message = "Utilisateur mis à jour" });
    }
    
    
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
    
    // GET: api/user/{id}/games
    [HttpGet]
    [Route("{id:int:min(1)}/games")]
    [EndpointName("GetUserGames")]
    [ActionName("GetUserGames")]
    public async Task<IActionResult> GetUserGames(int id)
    {
        var games = await _userService.GetUserGames(id);
        return Ok(games);    
    }

    
    // GET: api/user/{id}/recent-games
    [HttpGet]
    [Route("{id:int:min(1)}/recent-games")]
    [EndpointName("GetUserRecentlyPlayedGames")]
    [ActionName("GetUserRecentlyPlayedGame")]
    public async Task<IActionResult> GetUserRecentlyPlayedGame(int id, [FromQuery] int? count , [FromQuery] bool? includeProgression, [FromQuery] string? l)
    {
        if (count <= 0)
            return BadRequest(new { error = "COUNT_MUST_BE_POSITIVE" });
        
        var games = await _userService.GetUserRecentlyPlayedGames(id, count ?? null, includeProgression ?? false, l ?? "french");
        foreach (var game in games)
            AddHateosGameLinks(game, id);
        
        return Ok(games);    
    }
    
    
    // GET: api/user/{id}/progression-game/{appId}
    [AllowAnonymous]
    [HttpGet]
    [Route("{id:int:min(1)}/progression-game/{appId:int:min(1)}")]
    [EndpointName("GetUserProgressionGame")]
    [ActionName("GetUserProgressionGame")]
    public async Task<IActionResult> GetUserProgressionGame(int id, int appId, [FromQuery] string? l)
    {
        
        var progression = await _userService.GetUserProgressionGame(id, appId, l ?? "french");
        return Ok( new
        {
            id,
            appId,
            progression
        });    
    }
    
    // GET: api/user/{id}/recent-achievements
    [HttpGet]
    [Route("{id:int:min(1)}/recent-achievements")]
    [EndpointName("GetUserRecentAchievements")]
    [ActionName("GetUserRecentAchievements")]
    public async Task<IActionResult> GetUserRecentAchievements(int id, [FromQuery] int? count, [FromQuery] bool? includeRarity, [FromQuery] string? l)
    {
        if (count <= 0)
            return BadRequest(new { error = "COUNT_MUST_BE_POSITIVE" });
        
        var achievements = await _userService.GetUserRecentAchievements(id, count ?? 10, includeRarity ?? false, l ?? "french");
        return Ok(achievements);
    }


    private void AddHateosGameLinks(GameDto game, int userId)
    {
        game.Links = new List<Link> {
            _hateosLinkGenerator.Generate(
                "GetUserProgressionGame",
                new
                {
                    id = userId,
                    appId = game.Id
                },
                "get-user-progression-game",
                "GET"),
           
        };
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
                new {id = user.Id },
                "delete-user",
                "DELETE"),
            _hateosLinkGenerator.Generate(
                "GetUserRecentAchievements",
                new { id = user.Id },
                "recent-achievements",
                "GET"),
            
            // Collections
            _hateosLinkGenerator.Generate(
                "GetAllCollections",
                new { userId = user.Id },
                "collections",
                "GET"),
            _hateosLinkGenerator.Generate(
                "CreateCollection",
                new { userId = user.Id },
                "create-collection",
                "POST"),
        });
    }
}