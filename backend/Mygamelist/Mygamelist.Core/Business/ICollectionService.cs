using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ICollectionService
    {
        IEnumerable<Collection> RetrieveAll(int userId);
        Collection Add(int userId, string label);
        bool Remove(int id);


        
    }
}