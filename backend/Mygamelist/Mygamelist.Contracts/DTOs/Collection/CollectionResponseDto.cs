namespace Mygamelist.Contracts.DTOs.Collection;

using Mygamelist.Contracts.Hateos;

public class CollectionResponseDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public string Label { get; set; }
    
    public ICollection<int> GamesId { get; set; }
    
    public List<Link> Links { get; set; } = new();
}