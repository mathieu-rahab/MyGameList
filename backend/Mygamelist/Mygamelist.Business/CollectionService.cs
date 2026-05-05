using System.Net;
using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;
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

        public Collection? RetrieveById(int id) => _collectionRepository.SelectById(id);
        
        public Collection Update(int id, Collection collection) => _collectionRepository.Update(id, collection);
        

        public bool Remove(int id) => _collectionRepository.Delete(id);

        public Collection? AddGame(int userId, int collectionId, int gameId)
        {
            var collection = _collectionRepository.SelectById(collectionId);
            if (collection == null)
                throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND");
            
            // La collection doit appartenir au User
            if (collection.UserId != userId)
                throw new BusinessException(HttpStatusCode.Unauthorized, "NOT_YOUR_COLLECTION");

            // Le jeu ne doit pas déjà être dans la collection
            return (collection.GamesId.Contains(gameId))
                ? throw new BusinessException(HttpStatusCode.Conflict, "ALREADY_IN_COLLECTION")
                :  _collectionRepository.InsertGame(collectionId, gameId);
        }

        public Collection RemoveGame(int userId, int collectionId, int gameId)
        {
            var collection = _collectionRepository.SelectById(collectionId);
            if (collection == null)
                throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND");
            
            // La collection doit appartenir au User
            if (collection.UserId != userId)
                throw new BusinessException(HttpStatusCode.Unauthorized, "NOT_YOUR_COLLECTION");

            // Le jeu doit être dans la collection
            return (!collection.GamesId.Contains(gameId))
                ? throw new BusinessException(HttpStatusCode.Conflict, "NOT_IN_COLLECTION")
                : _collectionRepository.DeleteGame(collectionId, gameId);
        } 
    }
}