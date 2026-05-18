using System.Net;
using System.Text.Json;
using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;


namespace Mygamelist.Business;

public class SteamService(string steamKey, HttpClient httpClient, IMemoryCache memoryCache) : ISteamService
{
    private const string BaseUrlStore = "https://store.steampowered.com/api/";
    private const string BaseUrlApi = "https://api.steampowered.com/";
    
    private static string ValidateLanguage(string? l)
    {
        return l?.ToLowerInvariant() switch
        {
            "french" => "french",
            "english" => "english",
            _ => "french"
        };
    }

    private async Task<JsonElement> FetchApi(string apiUrl)
    {
        try
        {
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            return jsonDoc.RootElement;
        }
        catch (HttpRequestException)
        {
            throw new BusinessException(HttpStatusCode.BadGateway, $"STEAM_ERROR");
        }
    }
    
    public async Task<GameInfoDto> GameInfo(int id, string? l)
    {
        string language = ValidateLanguage(l);
        string cacheKey = $"game_{id}_{language}";
        // Vérifier si le jeu est déjà en cache
        if (memoryCache.TryGetValue(cacheKey, out GameInfoDto? cachedGame) && cachedGame != null) return cachedGame; 
        
        // Si non, récupérer depuis l'API
        string apiUrl = $"{BaseUrlStore}appdetails?appids={id}&l={language}";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty(id.ToString(), out var gameElement)) 
            throw new BusinessException(HttpStatusCode.NotFound, $"GAME_NOT_FOUND");
        
        if (!gameElement.TryGetProperty("success", out var successElement) || !successElement.GetBoolean())
            throw new BusinessException(HttpStatusCode.NotFound, "GAME_NOT_FOUND");

        if (!gameElement.TryGetProperty("data", out var data))
            throw new BusinessException(HttpStatusCode.NotFound, "GAME_DATA_NOT_FOUND");

        
        //var data = gameElement.GetProperty("data");

        GameInfoDto game = new GameInfoDto
        {
            Id   = data.GetProperty("steam_appid").GetInt32(),
            Name = data.GetProperty("name").GetString() ?? "",
            Description = data.GetProperty("detailed_description").GetString() ?? "",
            Image = data.GetProperty("header_image").GetString() ?? ""
        };
        memoryCache.Set(cacheKey, game, TimeSpan.FromHours(3));
        return game;
    }
    
    
    public async Task<List<GameDto>> UserGames(string steamId, string? l)
    {
        string language = ValidateLanguage(l);
        string cacheKey = $"user_games_{steamId}_{language}";

        if (memoryCache.TryGetValue(cacheKey, out List<GameDto>? cachedGames) && cachedGames != null)
            return cachedGames;

        string apiUrl = $"{BaseUrlApi}IPlayerService/GetOwnedGames/v1/?key={steamKey}&steamid={steamId}";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty("response", out var responseElement))
            throw new BusinessException(HttpStatusCode.NotFound, "USER_GAMES_NOT_FOUND");

        if (!responseElement.TryGetProperty("games", out var gamesElement))
            return new List<GameDto>();

        var tasks = gamesElement
            .EnumerateArray()
            .Select(async gameElement =>
            {
                int appId = gameElement.GetProperty("appid").GetInt32();
                int playtimeForever = gameElement.GetProperty("playtime_forever").GetInt32();

                try
                {
                    GameInfoDto gameInfo = await GameInfo(appId, language);

                    return new GameDto
                    {
                        Id = gameInfo.Id,
                        Name = gameInfo.Name,
                        Image = gameInfo.Image,
                        PlaytimeForever = playtimeForever
                    };
                }
                catch (BusinessException)
                {
                    return null;
                }
            });

        GameDto?[] games = await Task.WhenAll(tasks);

        List<GameDto> list = games
            .Where(game => game != null)
            .Select(game => game!)
            .ToList();

        memoryCache.Set(cacheKey, list, TimeSpan.FromHours(3));

        return list;
    }

    
  

}