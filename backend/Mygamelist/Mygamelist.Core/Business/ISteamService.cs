using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ISteamService
    {
        Task<GameInfoDto> GameInfo(int id, string? l);
        Task<List<GameDto>> UserGames(string steamId, string? l);


    }
}