using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ICollectionService
    {
        IEnumerable<Collection> RetrieveAll(int userId);
        Collection? RetrieveById(int id);
        Collection Add(int userId, string label);
        Collection Update(int id, Collection collection);
        bool Remove(int id);


        
    }
}