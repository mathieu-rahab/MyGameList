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
    
    
}