using Mygamelist.Contracts.Hateos;

namespace Mygamelist.Contracts.DTOs.Friendship;

public enum FriendshipStatus
{
    Pending,
    Accepted,
    Refused
}

public class FriendshipResponseDto
{
    public int Id { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Link> Links { get; set; } = new();
}