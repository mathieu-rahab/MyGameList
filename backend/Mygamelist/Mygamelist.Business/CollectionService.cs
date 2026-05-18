using System.Net;
using Mygamelist.Contracts.DTOs.Collection;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;
using Mygamelist.Core.Repository;
using Mygamelist.Entity;


namespace Mygamelist.Business
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IUserRepository _userRepository;

        public CollectionService(ICollectionRepository collectionRepository, IUserRepository userRepository)
        {
            _collectionRepository = collectionRepository;
            _userRepository = userRepository;
        }
        
        private static CollectionResponseDto MapToDto(Collection collection) => new CollectionResponseDto
        {
            Id = collection.Id,
            UserId = collection.UserId,
            Label = collection.Label,
            GamesId = collection.GamesId
        };
        
        public IEnumerable<CollectionResponseDto> RetrieveAll(int userId) => _collectionRepository.SelectAll(userId).Select(MapToDto).ToList();

        public CollectionResponseDto Add(int userId, string label)
        {
            var user = _userRepository.SelectById(userId);
            return user is null
                ? throw new BusinessException(HttpStatusCode.NotFound, "USER_NOT_FOUND")
                :  MapToDto(_collectionRepository.Insert( new Collection {Label = label, UserId = userId, GamesId = new List<int>()}));
        }

        public CollectionResponseDto RetrieveById(int id)
        {
            var collection = _collectionRepository.SelectById(id);
            return collection is null
                ? throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND")
                : MapToDto(collection);
        }

        public CollectionResponseDto Update(int id, UpdateCollectionDto dto)
        {
            var collection = _collectionRepository.SelectById(id);
            
            if (collection.UserId != dto.UserId)
                throw new BusinessException(HttpStatusCode.Unauthorized, "NOT_YOUR_COLLECTION");
            
            if (collection == null)
                throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND");

            if (dto.GamesId is not null && dto.GamesId.Count != dto.GamesId.Distinct().Count())
                throw new BusinessException(HttpStatusCode.BadRequest, "ALREADY_IN_COLLECTION");
            
            if (dto.Label is not null)
                collection.Label = dto.Label;
            
            if (dto.GamesId is not null)
                collection.GamesId = dto.GamesId;
            
            return MapToDto( _collectionRepository.Update(id, collection) );
        } 
        
        public bool Remove(int id)
        {
            var deleted = _collectionRepository.Delete(id);
            return (!deleted)
                ? throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND")
                : true;
        }

        public Collection? AddGame(int userId, int collectionId, int gameId)
        {
            var collection = _collectionRepository.SelectById(collectionId);
            if (collection == null)
                throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND");
            
            // La collection doit appartenir à l'User.
            if (collection.UserId != userId)
                throw new BusinessException(HttpStatusCode.Unauthorized, "NOT_YOUR_COLLECTION");

            // Le jeu ne doit pas déjà être dans la collection.
            return (collection.GamesId.Contains(gameId))
                ? throw new BusinessException(HttpStatusCode.Conflict, "ALREADY_IN_COLLECTION")
                :  _collectionRepository.InsertGame(collectionId, gameId);
        }

        public Collection RemoveGame(int userId, int collectionId, int gameId)
        {
            var collection = _collectionRepository.SelectById(collectionId);
            if (collection == null)
                throw new BusinessException(HttpStatusCode.NotFound, "COLLECTION_NOT_FOUND");
            
            // La collection doit appartenir à l'User
            if (collection.UserId != userId)
                throw new BusinessException(HttpStatusCode.Unauthorized, "NOT_YOUR_COLLECTION");

            // Le jeu doit être dans la collection.
            return (!collection.GamesId.Contains(gameId))
                ? throw new BusinessException(HttpStatusCode.Conflict, "NOT_IN_COLLECTION")
                : _collectionRepository.DeleteGame(collectionId, gameId);
        } 
    }
}