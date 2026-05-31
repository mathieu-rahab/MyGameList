using Mygamelist.Contracts.DTOs.Collection;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ICollectionService
    {
        IEnumerable<CollectionResponseDto> RetrieveAll(int userId);
        CollectionResponseDto RetrieveById(int id);
        CollectionResponseDto Add(int userId, string label);
        CollectionResponseDto Update(int id, UpdateCollectionDto dto);
        bool Remove(int id);
        Collection? AddGame(int userId, int collectionId, int gameId);
        Collection? RemoveGame(int userId, int collectionId, int gameId);
        
    }
}