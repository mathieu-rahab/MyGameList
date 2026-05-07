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
    private const string BaseUrl = "https://api.steampowered.com/";

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
    
    public async Task<GameDto> GameInfo(int id)
    {
        string cacheKey = $"game_{id}";
        // Vérifier si le jeu est déjà en cache
        if (memoryCache.TryGetValue(cacheKey, out GameDto? cachedGame) && cachedGame != null) return cachedGame; 
        
        // Si non, récupérer depuis l'API
        const string language = "french";
        string apiUrl = $"{BaseUrlStore}appdetails?appids={id}&l={language}";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty(id.ToString(), out var gameElement)) 
            throw new BusinessException(HttpStatusCode.NotFound, $"GAME_NOT_FOUND");
        
        var data = gameElement.GetProperty("data");

        GameDto game = new GameDto
        {
            Id   = data.GetProperty("steam_appid").GetInt32(),
            Name = data.GetProperty("name").GetString() ?? "",
            Description = data.GetProperty("detailed_description").GetString() ?? "",
            Image = data.GetProperty("header_image").GetString() ?? ""
        };
        memoryCache.Set(cacheKey, game, TimeSpan.FromHours(3));
        return game;
    }
    
}