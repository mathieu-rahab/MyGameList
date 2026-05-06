using System.Net;
using System.Text.Json;
using Mygamelist.Contracts.DTOs.Steam;
using Mygamelist.Core.Business;
using Mygamelist.Core.Exceptions;

namespace Mygamelist.Business;

public class SteamService(string steamKey, HttpClient httpClient) : ISteamService
{
    private readonly string _steamKey = steamKey;
    private readonly HttpClient _httpClient = httpClient;
    private const string BaseUrl = "https://store.steampowered.com/api/";

    private async Task<JsonElement> FetchApi(string apiUrl)
    {
        try
        {
            var response = await _httpClient.GetAsync(apiUrl);
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
        string apiUrl = $"{BaseUrl}appdetails?appids={id}&l=french";
        JsonElement root = await FetchApi(apiUrl);

        if (!root.TryGetProperty(id.ToString(), out var gameElement)) 
            throw new BusinessException(HttpStatusCode.NotFound, $"GAME_NOT_FOUND");

        bool success = gameElement.GetProperty("success").GetBoolean();
        if (!success)
            throw new BusinessException(HttpStatusCode.NotFound, "GAME_NOT_FOUND");

        var data = gameElement.GetProperty("data");

        return new GameDto
        {
            Id   = data.GetProperty("steam_appid").GetInt32(),
            Name = data.GetProperty("name").GetString() ?? "",
            Description = data.GetProperty("detailed_description").GetString() ?? "",
            Image = data.GetProperty("header_image").GetString() ?? ""
        };
    }
    
}