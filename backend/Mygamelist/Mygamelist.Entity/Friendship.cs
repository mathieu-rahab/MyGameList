namespace Mygamelist.Entity;

public enum FriendshipStatus
{
    Pending,
    Accepted,
    Refused
}

public class Friendship
{
    public int Id { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}