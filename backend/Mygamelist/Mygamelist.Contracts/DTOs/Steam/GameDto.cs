using Mygamelist.Contracts.Hateos;

namespace Mygamelist.Contracts.DTOs.Steam;

public class GameDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Image { get; set; }
    public int? PlaytimeForever { get; set; }
    public int? Playtime2Weeks { get; set; }
    public double? AchievementProgression { get; set; }
    public List<Link> Links { get; set; } = new();

    
}