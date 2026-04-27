namespace Mygamelist.Contracts.DTOs.User;

public class UserResponseDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string Pseudo { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? SteamId { get; set; }
}