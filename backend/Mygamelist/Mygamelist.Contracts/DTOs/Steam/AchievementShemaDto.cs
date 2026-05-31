namespace Mygamelist.Contracts.DTOs.Steam;

public class AchievementSchemaDto
{
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public double? Rarity { get; set; }
    public string? GameName { get; set; }
}