using Mygamelist.Contracts.DTOs.Friendship;

namespace Mygamelist.Core.Business;

public interface IFriendshipService
{
    IEnumerable<FriendshipResponseDto> RetrieveAll();
    FriendshipResponseDto RetrieveById(int id);
    FriendshipResponseDto Add(int authenticatedUserId, CreateFriendshipDto dto);
    FriendshipResponseDto Update(int id, int authenticatedUserId, UpdateFriendshipDto dto);
    bool Remove(int id);
    
    IEnumerable<FriendshipResponseDto> RetrievePendingSent(int authenticatedUserId);
    IEnumerable<FriendshipResponseDto> RetrievePendingReceived(int authenticatedUserId);
    IEnumerable<FriendshipResponseDto> RetrieveFriends(int authenticatedUserId);

}