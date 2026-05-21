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
    
    private const string BaseUrlImage = "https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/";
    private const string HeroImage = "library_hero.jpg";
    private const string VerticalCapsule = "library_600x900.jpg";
    private const string Logo = "logo.png";

    
    private static string GetCacheKey(string key, int id) => $"{key}_{id}";
    private static string GetCacheKey(string key, string id) => $"{key}_{id}";

    private static string GetImageUrl(int appId, string format) => $"{BaseUrlImage}{appId}/{format}";

    
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

    /// <summary>
    /// Retrieves the schema of all achievements for a specified Steam game application.
    /// </summary>
    /// <param name="appId">The Steam application ID of the game for which to fetch the achievement schema.</param>
    /// <param name="l">
    /// The language code for the achievement names and descriptions.
    /// If not provided, defaults to "french".
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of achievement schemas
    /// for the specified application ID. Returns an empty list if no achievements are found or if the game schema is invalid.
    /// </returns>
    /// <exception cref="BusinessException">
    /// Thrown when the game schema cannot be retrieved due to a server error or invalid request.
    /// </exception>
    public async Task<List<AchievementSchemaDto>> GetAchievementsSchema(int appId, string? l = "french")
    {
        string language = ValidateLanguage(l);
        string cacheKey = GetCacheKey("achievement_schema", $"{appId}_{language}");

            if (memoryCache.TryGetValue(cacheKey, out List<AchievementSchemaDto>? cachedSchema) && cachedSchema != null)
                return cachedSchema;

            string apiUrl = $"{BaseUrlApi}ISteamUserStats/GetSchemaForGame/v2/?key={steamKey}&appid={appId}&l={language}";
            Console.WriteLine(apiUrl);
            JsonElement json = await FetchApi(apiUrl);

            if (!json.TryGetProperty("game", out var gameElement))
                throw new BusinessException(HttpStatusCode.NotFound, "GAME_SCHEMA_NOT_FOUND");

            if (!gameElement.TryGetProperty("availableGameStats", out var statsElement))
                return new List<AchievementSchemaDto>();

            if (!statsElement.TryGetProperty("achievements", out var achievementsElement))
                return new List<AchievementSchemaDto>();

            List<AchievementSchemaDto> list = new List<AchievementSchemaDto>();

            foreach (var achievement in achievementsElement.EnumerateArray())
            {
                try
                {
                    // On vérifie juste que le "name" existe
                    if (!achievement.TryGetProperty("name", out var nameElement))
                        continue;

                    string name = nameElement.GetString() ?? "";
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    // Les autres attributs sont optionnels
                    string displayName = achievement.TryGetProperty("displayName", out var displayNameElement) 
                        ? displayNameElement.GetString() ?? "" 
                        : "";
        
                    string description = achievement.TryGetProperty("description", out var descriptionElement) 
                        ? descriptionElement.GetString() ?? "" 
                        : "";
        
                    string icon = achievement.TryGetProperty("icon", out var iconElement) 
                        ? iconElement.GetString() ?? "" 
                        : "";

                    list.Add(new AchievementSchemaDto
                    {
                        Name = name,
                        DisplayName = displayName,
                        Description = description,
                        Icon = icon
                    });
                }
                catch (Exception)
                {
                    continue;
                }
            }

            memoryCache.Set(cacheKey, list, TimeSpan.FromHours(24));
            return list;
        }

    /// <summary>
    /// Retrieves the total number of achievements available for a specified Steam game application.
    /// </summary>
    /// <param name="appId">The Steam application ID for which to retrieve the total achievement count.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the total count of achievements
    /// for the specified application ID. Returns 0 if no achievements are found or an error occurs.
    /// </returns>
    private async Task<int> GetTotalAchievementsCount(int appId)
    {
        string cacheKey = GetCacheKey("total_achievements_count", appId);

            if (memoryCache.TryGetValue(cacheKey, out int cachedCount))
                return cachedCount;

            List<AchievementSchemaDto> schema = await GetAchievementsSchema(appId);
            int count = schema.Count;

            memoryCache.Set(cacheKey, count, TimeSpan.FromHours(24));
            return count;
        }

        /// <summary>
        /// Calcule le pourcentage de progression des trophés pour un utilisateur
        /// </summary>
        public async Task<double> GetAchievementProgressionPercentage(string steamId, int appId)
        {
            string cacheKey = GetCacheKey("achievement_progression", $"{steamId}_{appId}");

            if (memoryCache.TryGetValue(cacheKey, out double cachedPercentage))
                return cachedPercentage;

            List<UserAchievementDto> userAchievements = await GetUserAchievements(steamId, appId);
            int totalAchievements = await GetTotalAchievementsCount(appId);

            if (totalAchievements == 0)
                return 0;

            double percentage = (userAchievements.Count / (double)totalAchievements) * 100;
            memoryCache.Set(cacheKey, percentage, TimeSpan.FromMinutes(10));

            return percentage;
        }

        /// <summary>
        /// Retrieves the list of achievements unlocked by a Steam user for a specific game application.
        /// </summary>
        /// <param name="steamId">The Steam user ID for which achievements are queried.</param>
        /// <param name="appId">The Steam application ID of the game whose achievements are being checked.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of unlocked achievements for the user in the specified game.</returns>
        /// <exception cref="BusinessException">Thrown when the Steam API request fails due to a not found error or when the user profile is not public.</exception>
        public async Task<List<UserAchievementDto>> GetUserAchievements(string steamId, int appId)
        {
            string cacheKey = GetCacheKey("user_achievements", $"{steamId}_{appId}");

            if (memoryCache.TryGetValue(cacheKey, out List<UserAchievementDto>? cachedAchievements) && cachedAchievements != null)
                return cachedAchievements;

            string apiUrl = $"{BaseUrlApi}ISteamUserStats/GetPlayerAchievements/v1/?key={steamKey}&steamid={steamId}&appid={appId}";
            JsonElement json = await FetchApi(apiUrl);

            if (!json.TryGetProperty("playerstats", out var playerStats))
                throw new BusinessException(HttpStatusCode.NotFound, "STATS_NOT_FOUND");

            if (playerStats.TryGetProperty("error", out var error))
                throw new BusinessException(HttpStatusCode.Forbidden, "PROFILE_NOT_PUBLIC", error.GetString() ?? "");

            if (!playerStats.TryGetProperty("achievements", out var achievements))
                return new List<UserAchievementDto>();

            List<UserAchievementDto> list = new List<UserAchievementDto>();

            foreach (var achievement in achievements.EnumerateArray())
            {
                try
                {
                    string apiName = achievement.GetProperty("apiname").GetString() ?? "";
                    int achieved = achievement.GetProperty("achieved").GetInt32();
                    int unlockTime = achievement.GetProperty("unlocktime").GetInt32();

                    if (achieved > 0)
                    {
                        list.Add(new UserAchievementDto()
                        {
                            ApiName = apiName,
                            Achieved = achieved,
                            UnlockTime = unlockTime
                        });
                    }
                }
                catch (BusinessException) { }
            }

            memoryCache.Set(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }


        /// <summary>
        /// Retrieves a list of games owned by the specified Steam user.
        /// </summary>
        /// <param name="steamId">The Steam user ID for which to fetch owned games.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="GameDto"/> objects representing the user's owned games.</returns>
        /// <exception cref="BusinessException">Thrown when the Steam API request fails or the user has no owned games.</exception>
        public async Task<List<GameDto>> UserGames(string steamId)
        {
            string cacheKey = GetCacheKey("user_games", steamId);

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
                //String capsuleFilename = gameElement.GetProperty("capsule_filename").GetString() ?? "";
                

                list.Add(new GameDto
                {
                    Id = appId,
                    Name = name,
                    Image = GetImageUrl(appId, VerticalCapsule),
                    PlaytimeForever = playtimeForever
                });
            }
            catch (BusinessException) {}
        }

        memoryCache.Set(cacheKey, list, TimeSpan.FromHours(3));

        return list;
    }


    /// <summary>
    /// Retrieves a list of recently played games for a Steam user.
    /// </summary>
    /// <param name="steamId">The Steam user ID for which to fetch recently played games.</param>
    /// <param name="count">Optional. The maximum number of games to return. If null, returns all available games.</param>
    public async Task<List<GameDto>> UserRecentlyPlayedGames(string steamId, int? count = null,
        bool? includeAchievements = false)
    {
        string cacheKey = GetCacheKey("user_recently_played_games", steamId);

        if (memoryCache.TryGetValue(cacheKey, out List<GameDto>? cachedGames) && cachedGames != null)
            return cachedGames.Take(count ?? cachedGames.Count).ToList();


        string apiUrl = $"{BaseUrlApi}IPlayerService/GetRecentlyPlayedGames/v1/?key={steamKey}&steamid={steamId}";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty("response", out var responseElement))
            throw new BusinessException(HttpStatusCode.NotFound, "USER_GAMES_NOT_FOUND");
        
        
        if (!responseElement.TryGetProperty("total_count", out var totalCount))
            throw new BusinessException(HttpStatusCode.NotFound, "NOT_ENOUGH_GAMES");

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
                int playtime2Weeks = gameElement.GetProperty("playtime_2weeks").GetInt32();
                
                double userAchiev = 0;
                if (includeAchievements == true)
                    userAchiev =  await GetAchievementProgressionPercentage(steamId, appId);

                list.Add(new GameDto
                {
                    Id = appId,
                    Name = name,
                    Image = GetImageUrl(appId, VerticalCapsule),
                    PlaytimeForever = playtimeForever,
                    Playtime2Weeks = playtime2Weeks,
                    AchievementProgression = userAchiev
                    
                });
            }
            catch (BusinessException) {}
        }
        
        memoryCache.Set(cacheKey, list, TimeSpan.FromMinutes(10));

        return list.Take(count ?? list.Count).ToList();
    }


    
  

}