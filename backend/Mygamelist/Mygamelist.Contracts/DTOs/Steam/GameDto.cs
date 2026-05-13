namespace Mygamelist.Contracts.DTOs.Steam;

public class GameDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Image { get; set; }
    public string? Description { get; set; }
    
}