using Mygamelist.Core.Repository;
using Mygamelist.DatabaseRepository.Context;
using Mygamelist.Entity;

namespace Mygamelist.DatabaseRepository;

public class CollectionRepository : ICollectionRepository
{
    private readonly AppDbContext _dbContext;
        
    public CollectionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IEnumerable<Collection> SelectAll(int userId) => _dbContext.Collections.Where(c => c.UserId == userId);

    public Collection Insert(Collection collection)
    {
        _dbContext.Collections.Add(collection);
        _dbContext.SaveChanges();
        return collection;
    }
    
    public Collection? SelectById(int id)
    {
        return _dbContext.Collections.FirstOrDefault(u => u.Id == id);
    }

    public Collection Update(int id, Collection collection)
    {
        // TODO
        return collection;
    }


    public bool Delete(int id)
    {
        var collectionToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == id);
        if (collectionToDelete == null) return false;
        _dbContext.Users.Remove(collectionToDelete);
        _dbContext.SaveChanges();

        return true;
    }
    
}