using Mygamelist.Entity;

namespace Mygamelist.Core.Repository
{
    public interface ICollectionRepository
    {
        public IEnumerable<Collection> SelectAll(int userId);
        Collection Insert(Collection collection);
        Collection? SelectById(int id);
        Collection Update(int id, Collection collection);
        public bool Delete(int id);

    }
}