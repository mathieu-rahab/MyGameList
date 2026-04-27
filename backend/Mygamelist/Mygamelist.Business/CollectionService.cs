using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Core.Business;
using Mygamelist.Core.Repository;
using Mygamelist.Entity;


namespace Mygamelist.Business
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;

        public CollectionService(ICollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }
        
        public IEnumerable<Collection> RetrieveAll(int userId) => _collectionRepository.SelectAll(userId);

        public Collection Add(int userId, string label)
        {
            return _collectionRepository.Insert( new Collection {Label = label, UserId = userId, GamesId = new List<int>()});
        }

        public bool Remove(int id) => _collectionRepository.Delete(id);
        
        
    }
}