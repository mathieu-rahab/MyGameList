namespace Mygamelist.Contracts.DTOs.User;

public class ResetUserDto
{
    public required string Pseudo { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? SteamId { get; set; }
    public string? ProfilePicturePath { get; set; }
}