namespace Mygamelist.Entity;

public class User
{
    public int Id { get; set; }
    public string Pseudo { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? SteamId { get; set; }
    public string? ProfilePicturePath { get; set; }
}