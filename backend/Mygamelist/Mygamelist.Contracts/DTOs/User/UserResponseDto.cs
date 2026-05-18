namespace Mygamelist.Contracts.DTOs.User;

using Mygamelist.Contracts.Hateos;

public class UserResponseDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string Pseudo { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? SteamId { get; set; }
    
    public List<Link> Links { get; set; } = new();
}