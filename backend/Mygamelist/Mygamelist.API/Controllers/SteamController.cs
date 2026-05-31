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
    public async Task<ActionResult<GameInfoDto>> GetGame(int gameId, [FromQuery(Name = "l")] string? language = null)
    {
        var result = await _steamService.GameInfo(gameId, language);
        return Ok(result);
    }
    
    /*
    [AllowAnonymous]
    [HttpGet]
    [Route("user/{steamId}/games")]
    [EndpointName("GetSteamIdGames")]
    [ActionName("GetStealIdGames")]
    public async Task<ActionResult<List<GameDto>>> GetUserGames(string steamId, [FromQuery(Name = "l")] string? language = null)
    {
        var result = await _steamService.UserGames(steamId);
        return Ok(result);
    }
    */
    [AllowAnonymous]
    [HttpGet]
    [Route("search")]
    [EndpointName("SearchGames")]
    [ActionName("SearchGames")]
    public async Task<ActionResult<List<SearchGameDto>>> SearchGames(
        [FromQuery(Name = "term")] string term,
        [FromQuery(Name = "l")] string l = "french",
        [FromQuery(Name = "cc")] string cc = "fr")
    {
        var result = await _steamService.SearchGames(term, l, cc);
        return Ok(result);
    }
}