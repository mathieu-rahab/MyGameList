namespace Mygamelist.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mygamelist.Contracts.DTOs.Collection;
using Mygamelist.Entity;
using Mygamelist.Core.Business;


[ApiController]
[Route("api/users/{userId:int:min(1)}/[controller]")]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;
    public CollectionController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }
    
    // GET: api/users/{userId}/collection
    [HttpGet("")]
    public IActionResult GetAll(int userId)
    {
        return Ok(_collectionService.RetrieveAll(userId));
    }
    
    
    // POST: api/users/{userId}/collection
    [HttpPost("")]
    public IActionResult CreateCollection(int userId, [FromBody] CreateCollectionDto dto)
    {
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
    
    // DELETE: api/users/{userId}/collection/{id}
    [HttpDelete("{id:int:min(1)}")]
    public IActionResult DeleteUser(int id)
    {
        // TODO
        return Ok(new { Id = id, Message = "Collection supprimé" });
    }
}