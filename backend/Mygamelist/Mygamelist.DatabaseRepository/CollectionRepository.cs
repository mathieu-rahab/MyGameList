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
        _dbContext.Collections.Update(collection);
        _dbContext.SaveChanges();
        
        return collection;
    }


    public bool Delete(int id)
    {
        var collectionToDelete = _dbContext.Collections.FirstOrDefault(u => u.Id == id);
        if (collectionToDelete == null) return false;
        _dbContext.Collections.Remove(collectionToDelete);
        _dbContext.SaveChanges();

        return true;
    }

    public Collection? InsertGame(int collectionId, int gameId)
    {
        var collection = SelectById(collectionId);
        if (collection == null) return null;
        collection.GamesId.Add(gameId);
        _dbContext.SaveChanges();
        return collection;
    }
    
    public Collection? DeleteGame(int collectionId, int gameId)
    {
        var collection = SelectById(collectionId);
        if (collection == null) return null;
        collection.GamesId.Remove(gameId);
        _dbContext.SaveChanges();
        return collection;
    }
    
}