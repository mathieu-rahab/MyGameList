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
    private const string BaseUrlCapsule = "https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/";
    
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
    
    
    public async Task<List<GameDto>> UserGames(string steamId)
    {
        string cacheKey = $"user_games_{steamId}";

        if (memoryCache.TryGetValue(cacheKey, out List<GameDto>? cachedGames) && cachedGames != null)
            return cachedGames;

        string apiUrl = $"{BaseUrlApi}IPlayerService/GetOwnedGames/v1/?key={steamKey}&steamid={steamId}&include_appinfo=true&include_extended_appinfo=true";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty("response", out var responseElement))
            throw new BusinessException(HttpStatusCode.NotFound, "USER_GAMES_NOT_FOUND");

        if (!responseElement.TryGetProperty("games", out var gamesElement))
            return new List<GameDto>();


        List<GameDto> list = new List<GameDto>();
        
        foreach (var gameElement in gamesElement.EnumerateArray())
        {
            try
            {
                int appId = gameElement.GetProperty("appid").GetInt32(); 
                String name = gameElement.GetProperty("name").GetString() ?? "";
                int playtimeForever = gameElement.GetProperty("playtime_forever").GetInt32();
                String capsuleFilename = gameElement.GetProperty("capsule_filename").GetString() ?? "";
                

                list.Add(new GameDto
                {
                    Id = appId,
                    Name = name,
                    Image = $"{BaseUrlCapsule}{appId}/{capsuleFilename}",
                    PlaytimeForever = playtimeForever
                });
            }
            catch (BusinessException) {}
        }
        

        memoryCache.Set(cacheKey, list, TimeSpan.FromHours(3));

        return list;
    }

    
  

}