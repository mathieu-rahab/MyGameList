using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Mygamelist.Contracts.DTOs.Friendship;
using Mygamelist.Entity;
using Mygamelist.Core.Business;
using Mygamelist.Contracts.Hateos;
using Mygamelist.Hateos;


namespace Mygamelist.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FriendshipController
{
    private readonly IFriendshipService _FriendshipService;
    private readonly IHateosLinkGenerator _hateosLinkGenerator;
    public FriendshipController(IFriendshipService FriendshipService, IHateosLinkGenerator hateosLinkGenerator)
    {
        _FriendshipService = FriendshipService;
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
        var Friendships = _FriendshipService.RetrieveAll();
        foreach (FriendshipResponseDto Friendship in Friendships)
        {
            AddHateosLinks(Friendship);
        }
        return Ok(Friendships);
    }
}