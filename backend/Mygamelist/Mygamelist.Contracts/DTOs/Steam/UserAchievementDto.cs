namespace Mygamelist.Contracts.DTOs.Steam;

public class UserAchievementDto
{
    public required string ApiName { get; set; }
    public required int Achieved { get; set; }
    public required int UnlockTime { get; set; }
}