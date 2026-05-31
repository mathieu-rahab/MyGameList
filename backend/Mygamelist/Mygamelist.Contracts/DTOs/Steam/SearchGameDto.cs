namespace Mygamelist.Contracts.DTOs.Steam;

public class SearchGameDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string TinyImage { get; set; }
    public required string Image { get; set; }
}