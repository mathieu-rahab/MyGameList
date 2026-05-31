namespace Mygamelist.Entity;

public class User
{
    public int Id { get; set; }
    public required string Pseudo { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? SteamId { get; set; }
    public string? ProfilePicturePath { get; set; }
    public ICollection<Friendship>? Friendships { get; set; }
    public ICollection<Collection>? Collections { get; set; }
}