using Microsoft.AspNetCore.Authorization;

namespace Mygamelist.Controllers;

using Microsoft.AspNetCore.Mvc;
using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Core.Business;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SteamController : ControllerBase
{
    private readonly ISteamService _steamService;
    public SteamController(ISteamService steamService)
    {
        _steamService = steamService;
    }
    
    // GET: api/steam/game/{gameId}/?l={language}
    [AllowAnonymous]

    [HttpGet]
    [Route("game/{gameId:int:min(1)}/")]
    [EndpointName("GetGame")]
    [ActionName("GetGame")]
    public async Task<ActionResult<GameDto>> GetGame(int gameId, [FromQuery(Name = "l")] string? language = null)
    {
        var result = await _steamService.GameInfo(gameId, language);
        return Ok(result);
    }
    
    
    
}