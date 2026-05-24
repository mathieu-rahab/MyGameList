using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Contracts.DTOs.User;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface IUserService
    {
        IEnumerable<UserResponseDto> RetrieveAll();
        UserResponseDto RetrieveById(int id);

        UserResponseDto Add(CreateUserDto dto);
        UserResponseDto Update(int id, UpdateUserDto user);
        bool Remove(int id);
        Task<List<GameDto>> GetUserGames(int id);
        Task<List<GameDto>> GetUserRecentlyPlayedGames(int id, int? count, bool? includeProgression, string? l);
        Task<List<AchievementSchemaDto>> GetUserRecentAchievements(int id, int count,
            bool includeRarity, string l);

        Task<double> GetUserProgressionGame(int id, int appId, string? l);
    }
}