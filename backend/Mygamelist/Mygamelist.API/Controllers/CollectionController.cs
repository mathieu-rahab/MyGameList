using Microsoft.AspNetCore.Authorization;

namespace Mygamelist.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mygamelist.Contracts.DTOs.Collection;
using Mygamelist.Core.Business;
using Mygamelist.Hateos;
using Mygamelist.Contracts.Hateos;

[Authorize]
[ApiController]
[Route("api/user/{userId:int:min(1)}/[controller]")]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;
    private readonly IHateosLinkGenerator _hateosLinkGenerator;
    public CollectionController(ICollectionService collectionService, IHateosLinkGenerator hateosLinkGenerator)
    {
        _collectionService = collectionService;
        _hateosLinkGenerator = hateosLinkGenerator;
    }
    
    // GET: api/users/{userId}/collection
    [HttpGet]
    [Route("")]
    [EndpointName("GetAll")]
    [ActionName("GetAll")]
    public IActionResult GetAll(int userId)
    {
        var collections = _collectionService.RetrieveAll(userId);
        foreach (var collection in collections)
        {
            AddHateosLinks(collection);
        }
        return Ok(collections);
    }
    
    
    // POST: api/users/{userId}/collection
    [HttpPost]
    [Route("")]
    [EndpointName("CreateCollection")]
    [ActionName("CreateCollection")]
    public IActionResult CreateCollection(int userId, [FromBody] CreateCollectionDto dto)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, userId)) return Forbid();
        try
        {
            return Ok(_collectionService.Add(userId, dto.Label));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { error = "DATABASE_ERROR" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "INTERNAL_ERROR" });
        }
    }
    
    // PUT: api/users/{userId}/collection/{id}
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [EndpointName("UpdateCollection")]
    [ActionName("UpdateCollection")]
    public IActionResult UpdateCollection(int userId, int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, userId)) return Forbid();
        // TODO
        return Ok(new { Id = id, Message = "Collection mis à jour" });
    }
    
    // DELETE: api/users/{userId}/collection/{id}
    [HttpDelete]
    [Route("{id:int:min(1)}")]
    [EndpointName("DeleteCollection")]
    [ActionName("DeleteCollection")]
    public IActionResult DeleteCollection(int userId, int id)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, userId)) return Forbid();
        // TODO
        return Ok(new { Id = id, Message = "Collection supprimé" });
    }
    
    
    // POST: api/users/{userId}/collection/{id}/game
    [HttpPost]
    [Route("{id:int:min(1)}/game")]
    [EndpointName("AddGame")]
    [ActionName("AddGame")]
    public IActionResult AddGame(int userId, int id, [FromBody] GameIdDto dto)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, userId)) return Forbid();
        return Ok(_collectionService.AddGame(userId, id, dto.GameId));
    }
    
    // POST: api/users/{userId}/collection/{id}/game
    [HttpDelete]
    [Route("{id:int:min(1)}/game")]
    [EndpointName("RemoveGame")]
    [ActionName("RemoveGame")]
    public IActionResult RemoveGame(int userId, int id, [FromBody] GameIdDto dto)
    {
        if (!Utiles.UserSystems.IsAdminOrSelf(User, userId)) return Forbid();
        return Ok(_collectionService.RemoveGame(userId, id, dto.GameId));
    }
    
    private void AddHateosLinks(CollectionResponseDto collection)
    {
        collection.Links.AddRange(new List<Link>
        {
            _hateosLinkGenerator.Generate(
                "GetCollection",
                new
                {
                    userId = collection.UserId,
                    id = collection.Id
                },
                "self",
                "GET"),

            _hateosLinkGenerator.Generate(
                "UpdateCollection",
                new
                {
                    userId = collection.UserId,
                    id = collection.Id
                },
                "update-collection",
                "PUT"),

            _hateosLinkGenerator.Generate(
                "DeleteCollection",
                new
                {
                    userId = collection.UserId,
                    id = collection.Id
                },
                "delete-collection",
                "DELETE")
        });
    }
}