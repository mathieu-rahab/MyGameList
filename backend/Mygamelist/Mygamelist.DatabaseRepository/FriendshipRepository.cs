using Mygamelist.Core.Repository;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;

namespace Mygamelist.DatabaseRepository;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly AppDbContext _dbContext;

    public FriendshipRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Friendship> SelectAll()
    {
        return _dbContext.Friendships;
    }

    public Friendship? SelectById(int id)
    {
        return _dbContext.Friendships.FirstOrDefault(f => f.Id == id);
    }

    public Friendship Insert(Friendship friendship)
    {
        _dbContext.Friendships.Add(friendship);
        _dbContext.SaveChanges();

        return friendship;
    }

    public Friendship Update(int id, Friendship friendship)
    {
        _dbContext.Friendships.Update(friendship);
        _dbContext.SaveChanges();

        return friendship;
    }

    public bool Delete(int id)
    {
        var friendshipToDelete = _dbContext.Friendships.FirstOrDefault(f => f.Id == id);

        if (friendshipToDelete is null)
            return false;

        _dbContext.Friendships.Remove(friendshipToDelete);
        _dbContext.SaveChanges();

        return true;
    }
}