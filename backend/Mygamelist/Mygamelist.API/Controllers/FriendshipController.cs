using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mygamelist.Contracts.DTOs.Friendship;
using Mygamelist.Contracts.Hateos;
using Mygamelist.Core.Business;
using Mygamelist.Hateos;

namespace Mygamelist.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;
    private readonly IHateosLinkGenerator _hateosLinkGenerator;

    public FriendshipController(IFriendshipService friendshipService, IHateosLinkGenerator hateosLinkGenerator)
    {
        _friendshipService = friendshipService;
        _hateosLinkGenerator = hateosLinkGenerator;
    }

    // GET: api/friendship
    [Authorize(Policy = "isAdmin")]
    [HttpGet]
    [Route("")]
    [EndpointName("GetFriendships")]
    [ActionName("GetFriendships")]
    public IActionResult GetFriendships()
    {
        var friendships = _friendshipService.RetrieveAll();

        foreach (FriendshipResponseDto friendship in friendships)
        {
            AddHateosLinks(friendship);
        }

        return Ok(friendships);
    }

    // GET: api/friendship/{id}
    [HttpGet]
    [Route("{id:int:min(1)}")]
    [EndpointName("GetFriendship")]
    [ActionName("GetFriendship")]
    public IActionResult GetFriendship(int id)
    {
        var friendship = _friendshipService.RetrieveById(id);
        AddHateosLinks(friendship);

        return Ok(friendship);
    }

    // POST: api/friendship/{UserId}
    [HttpPost]
    [Route("")]
    [EndpointName("CreateFriendship")]
    [ActionName("CreateFriendship")]
    public IActionResult CreateFriendship([FromBody] CreateFriendshipDto dto)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;

        if (!int.TryParse(userIdClaim, out var authenticatedUserId))
            return Unauthorized(new { error = "INVALID_AUTHENTICATED_USER" });

        if (dto.User2Id <= 0)
            return BadRequest(new { error = "INVALID_USER_ID" });

        if (authenticatedUserId == dto.User2Id)
            return BadRequest(new { error = "USERS_MUST_BE_DIFFERENT" });

        var createdFriendship = _friendshipService.Add(authenticatedUserId, dto);
        AddHateosLinks(createdFriendship);

        return Ok(createdFriendship);
    }

    // PUT: api/friendship/{id}
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateFriendship")]
    [ActionName("UpdateFriendship")]
    public IActionResult UpdateFriendship(int id, [FromBody] UpdateFriendshipDto dto)
    {
        if (!TryGetAuthenticatedUserId(out var authenticatedUserId))
            return Unauthorized(new { error = "INVALID_AUTHENTICATED_USER" });

        var updatedFriendship = _friendshipService.Update(id, authenticatedUserId, dto);
        AddHateosLinks(updatedFriendship);

        return Ok(updatedFriendship);
    }

    // DELETE: api/friendship/{id}
    [HttpDelete]
    [Route("{id:int:min(1)}")]
    [EndpointName("DeleteFriendship")]
    [ActionName("DeleteFriendship")]
    public IActionResult DeleteFriendship(int id)
    {
        _friendshipService.Remove(id);

        return Ok(id);
    }
    
    // GET: api/friendship/pending/sent
    [HttpGet]
    [Route("pending/sent")]
    [EndpointName("GetPendingSentFriendships")]
    [ActionName("GetPendingSentFriendships")]
    public IActionResult GetPendingSentFriendships()
    {
        if (!TryGetAuthenticatedUserId(out var authenticatedUserId))
            return Unauthorized(new { error = "INVALID_AUTHENTICATED_USER" });

        var friendships = _friendshipService.RetrievePendingSent(authenticatedUserId).ToList();

        foreach (var friendship in friendships)
        {
            AddHateosLinks(friendship);
        }

        return Ok(friendships);
    }

    // GET: api/friendship/pending/received
    [HttpGet]
    [Route("pending/received")]
    [EndpointName("GetPendingReceivedFriendships")]
    [ActionName("GetPendingReceivedFriendships")]
    public IActionResult GetPendingReceivedFriendships()
    {
        if (!TryGetAuthenticatedUserId(out var authenticatedUserId))
            return Unauthorized(new { error = "INVALID_AUTHENTICATED_USER" });

        var friendships = _friendshipService.RetrievePendingReceived(authenticatedUserId).ToList();

        foreach (var friendship in friendships)
        {
            AddHateosLinks(friendship);
        }

        return Ok(friendships);
    }

    // GET: api/friendship/friends
    [HttpGet]
    [Route("friends")]
    [EndpointName("GetFriends")]
    [ActionName("GetFriends")]
    public IActionResult GetFriends()
    {
        if (!TryGetAuthenticatedUserId(out var authenticatedUserId))
            return Unauthorized(new { error = "INVALID_AUTHENTICATED_USER" });

        var friendships = _friendshipService.RetrieveFriends(authenticatedUserId).ToList();

        foreach (var friendship in friendships)
        {
            AddHateosLinks(friendship);
        }

        return Ok(friendships);
    }

    // POST: api/friendship
    [HttpPost]
    [Route("")]
    [EndpointName("CreateFriendship")]
    [ActionName("CreateFriendship")]
    

    private bool TryGetAuthenticatedUserId(out int authenticatedUserId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;

        return int.TryParse(userIdClaim, out authenticatedUserId);
    }

    private void AddHateosLinks(FriendshipResponseDto friendship)
    {
        friendship.Links.AddRange(new List<Link>
        {
            _hateosLinkGenerator.Generate(
                "GetFriendship",
                new { id = friendship.Id },
                "self",
                "GET"),
            _hateosLinkGenerator.Generate(
                "UpdateFriendship",
                new { id = friendship.Id },
                "update-friendship",
                "PUT"),
            _hateosLinkGenerator.Generate(
                "DeleteFriendship",
                new { id = friendship.Id },
                "delete-friendship",
                "DELETE")
        });
    }
}