namespace Mygamelist.Identity;

using Mygamelist.Hateos;

public class UserModel
{
    public int Id { get; set; }
    public required string Pseudo { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? SteamId { get; set; }
    public string? ProfilePicturePath { get; set; }
    
    public List<Link> Links { get; set; } = [];
}