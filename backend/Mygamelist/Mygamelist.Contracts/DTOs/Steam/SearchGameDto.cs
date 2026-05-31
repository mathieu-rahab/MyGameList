namespace Mygamelist.Contracts.DTOs.Steam;

public class SearchGameDto
{
    public required int AppId { get; set; }
    public required string Name { get; set; }
    public required string TinyImage { get; set; }
}