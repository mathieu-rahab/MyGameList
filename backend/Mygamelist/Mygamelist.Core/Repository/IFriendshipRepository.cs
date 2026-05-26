using Mygamelist.Entity;
namespace Mygamelist.Core.Repository;

public interface IFriendshipRepository
{
    IEnumerable<Friendship> SelectAll();
    Friendship? SelectById(int id);
    Friendship Insert(Friendship friendship);
    Friendship Update(int id, Friendship friendship);
    bool Delete(int id);
}