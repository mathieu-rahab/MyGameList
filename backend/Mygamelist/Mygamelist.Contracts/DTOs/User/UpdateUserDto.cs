namespace Mygamelist.Contracts.DTOs.User;

public class UpdateUserDto
{
    public string? Pseudo { get; set; }
    public string? Email { get; set; }
    public string? SteamId { get; set; }
    public string? ProfilePicturePath { get; set; }
}