using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ISteamService
    {
        Task<GameInfoDto> GameInfo(int id, string? l);
        Task<List<GameDto>> UserGames(string steamId);
        Task<List<GameDto>> UserRecentlyPlayedGames(string steamId, int? count, bool? includeProgression, string? l);
        
        
        // Méthodes pour les trophés
        Task<List<AchievementSchemaDto>> GetAchievementsSchema(int appId, string? l);
        Task<List<UserAchievementDto>> GetUserAchievements(string steamId, int appId);
        Task<double> GetAchievementProgressionPercentage(string steamId, int appId, string? l);
        Task<List<AchievementSchemaDto>> GetRecentAchievements(string steamId, int count, bool includeRarity, string l);




    }
}