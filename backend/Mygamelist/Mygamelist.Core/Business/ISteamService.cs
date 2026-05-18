using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Entity;

namespace Mygamelist.Core.Business
{
    public interface ISteamService
    {
        Task<GameDto> GameInfo(int id, string? l);
        
    }
}