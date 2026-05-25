using System.Net;
using Mygamelist.Contracts.DTOs.Friendship;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;
using Mygamelist.Core.Repository;
using Mygamelist.Entity;

namespace Mygamelist.Business;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;

    public FriendshipService(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    private static FriendshipResponseDto MapToDto(Friendship friendship)
    {
        return new FriendshipResponseDto
        {
            Id = friendship.Id,
            User1Id = friendship.User1Id,
            User2Id = friendship.User2Id,
            Status = (Mygamelist.Contracts.DTOs.Friendship.FriendshipStatus)friendship.Status,
            CreatedAt = friendship.CreatedAt
        };
    }

    public IEnumerable<FriendshipResponseDto> RetrieveAll()
    {
        return _friendshipRepository.SelectAll()
            .Select(MapToDto)
            .ToList();
    }

    public FriendshipResponseDto RetrieveById(int id)
    {
        var friendship = _friendshipRepository.SelectById(id);

        return friendship is null
            ? throw new BusinessException(HttpStatusCode.NotFound, "FRIENDSHIP_NOT_FOUND")
            : MapToDto(friendship);
    }

    public FriendshipResponseDto Add(int authenticatedUserId, CreateFriendshipDto dto)
    {
        var friendship = new Friendship
        {
            User1Id = authenticatedUserId,
            User2Id = dto.User2Id,
            Status = Mygamelist.Entity.FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        return MapToDto(_friendshipRepository.Insert(friendship));
    }

    public FriendshipResponseDto Update(int id, int authenticatedUserId, UpdateFriendshipDto dto)
    {
        var friendship = _friendshipRepository.SelectById(id);

        if (friendship is null)
            throw new BusinessException(HttpStatusCode.NotFound, "FRIENDSHIP_NOT_FOUND");

        if (friendship.User2Id != authenticatedUserId)
            throw new BusinessException(HttpStatusCode.Forbidden, "ONLY_RECEIVER_CAN_UPDATE_FRIENDSHIP");

        if (friendship.Status != Mygamelist.Entity.FriendshipStatus.Pending)
            throw new BusinessException(HttpStatusCode.BadRequest, "FRIENDSHIP_REQUEST_ALREADY_RESOLVED");

        if (dto.Status != Mygamelist.Contracts.DTOs.Friendship.FriendshipStatus.Accepted &&
            dto.Status != Mygamelist.Contracts.DTOs.Friendship.FriendshipStatus.Refused)
            throw new BusinessException(HttpStatusCode.BadRequest, "INVALID_FRIENDSHIP_STATUS");

        friendship.Status = (Mygamelist.Entity.FriendshipStatus)dto.Status;

        return MapToDto(_friendshipRepository.Update(id, friendship));
    }

    public bool Remove(int id)
    {
        var deleted = _friendshipRepository.Delete(id);

        return deleted
            ? true
            : throw new BusinessException(HttpStatusCode.NotFound, "FRIENDSHIP_NOT_FOUND");
    }

    public IEnumerable<FriendshipResponseDto> RetrievePendingSent(int authenticatedUserId)
    {
        return _friendshipRepository.SelectAll()
            .Where(friendship =>
                friendship.User1Id == authenticatedUserId &&
                friendship.Status == Mygamelist.Entity.FriendshipStatus.Pending)
            .Select(MapToDto)
            .ToList();
    }

    public IEnumerable<FriendshipResponseDto> RetrievePendingReceived(int authenticatedUserId)
    {
        return _friendshipRepository.SelectAll()
            .Where(friendship =>
                friendship.User2Id == authenticatedUserId &&
                friendship.Status == Mygamelist.Entity.FriendshipStatus.Pending)
            .Select(MapToDto)
            .ToList();
    }

    public IEnumerable<FriendshipResponseDto> RetrieveFriends(int authenticatedUserId)
    {
        return _friendshipRepository.SelectAll()
            .Where(friendship =>
                friendship.Status == Mygamelist.Entity.FriendshipStatus.Accepted &&
                (friendship.User1Id == authenticatedUserId || friendship.User2Id == authenticatedUserId))
            .Select(MapToDto)
            .ToList();
    }
}