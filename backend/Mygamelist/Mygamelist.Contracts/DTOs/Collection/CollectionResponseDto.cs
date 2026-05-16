namespace Mygamelist.Contracts.DTOs.Collection;

using Mygamelist.Contracts.Hateos;

public class CollectionResponseDto
{
    public required string Label { get; set; }
    
    public List<Link> Links { get; set; } = new();
}