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
    /// 
    /// </summary>
    /// <param name="steamId">The Steam user ID for which achievements are queried.</param>
    /// </param>
    public async Task<JsonElement> VerifySteamId(string steamId)
    {
        string apiUrl =
            $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={steamKey}&steamids={steamId}";

        JsonElement json = await FetchApi(apiUrl);
        
        if (!json.TryGetProperty("response", out JsonElement responseElement))
        {
            throw new BusinessException(
                HttpStatusCode.BadGateway,
                "STEAM_INVALID_RESPONSE"
            );
        }
        
        if (!responseElement.TryGetProperty("players", out JsonElement playersElement))
        {
            throw new BusinessException(
                HttpStatusCode.BadGateway,
                "STEAM_INVALID_RESPONSE"
            );
        }
        
        if (playersElement.GetArrayLength() == 0)
        {
            throw new BusinessException(
                HttpStatusCode.NotFound,
                "USER_NOT_FOUND"
            );
        }
        
        return playersElement[0];
    }

    public async Task<string> VanityResolve(string specialId)
    {
        string apiUrl =
            $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={steamKey}&vanityurl={specialId}";

        JsonElement json = await FetchApi(apiUrl);
        
        if (!json.TryGetProperty("response", out JsonElement responseElement))
        {
            throw new BusinessException(
                HttpStatusCode.BadGateway,
                "STEAM_INVALID_RESPONSE"
            );
        }
        
        if (!responseElement.TryGetProperty("success", out JsonElement successElement))
        {
            throw new BusinessException(
                HttpStatusCode.BadGateway,
                "STEAM_INVALID_RESPONSE"
            );
        }

        int successCode = successElement.GetInt32();
        
        if (successCode != 1)
        {
            throw new BusinessException(
                HttpStatusCode.NotFound,
                "USER_NOT_FOUND"
            );
        }
        
        if (!responseElement.TryGetProperty("steamid", out JsonElement steamIdElement))
        {
            throw new BusinessException(
                HttpStatusCode.BadGateway,
                "STEAM_INVALID_RESPONSE"
            );
        }

        return steamIdElement.GetString();
    }

    /// <summary>
    /// Retrieves the schema of all achievements for a specified Steam game application.
    /// </summary>
    /// <param name="appId">The Steam application ID of the game for which to fetch the achievement schema.</param>
    /// <param name="l">
    /// The language code for the achievement names. If not provided, defaults to "french".
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
    /// <param name="l">
    /// The language code for the achievement names. If not provided, defaults to "french".
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the total count of achievements
    /// for the specified application ID. Returns 0 if no achievements are found or an error occurs.
    /// </returns>
    private async Task<int> GetTotalAchievementsCount(int appId, string? l = "french")
    {
        string cacheKey = GetCacheKey("total_achievements_count", appId);

            if (memoryCache.TryGetValue(cacheKey, out int cachedCount))
                return cachedCount;

            List<AchievementSchemaDto> schema = await GetAchievementsSchema(appId, l);
            int count = schema.Count;

            memoryCache.Set(cacheKey, count, TimeSpan.FromHours(24));
            return count;
        }

        /// <summary>
        /// Calcule le pourcentage de progression des trophés pour un utilisateur
        /// </summary>
        public async Task<double> GetAchievementProgressionPercentage(string steamId, int appId, string? l = "french")
        {
            string cacheKey = GetCacheKey("achievement_progression", $"{steamId}_{appId}");

            if (memoryCache.TryGetValue(cacheKey, out double cachedPercentage))
                return cachedPercentage;

            List<UserAchievementDto> userAchievements = await GetUserAchievements(steamId, appId);
            int totalAchievements = await GetTotalAchievementsCount(appId, l);

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
        /// Retrieves a list of recently played games for a specified Steam user.
        /// </summary>
        /// <param name="steamId">The Steam user ID for which to fetch recently played games.</param>
        /// <param name="count">
        /// The maximum number of games to return. If not provided, all available games are returned.
        /// </param>
        /// <param name="includeProgression">
        /// A flag indicating whether to include achievement progression data in the response.
        /// Defaults to <c>false</c>.
        /// </param>
        /// <param name="l">
        /// The language code for the achievement names. If not provided, defaults to "french".
        /// </param>
        /// <returns>
        /// The task result contains a list of recently played games for the specified Steam user,
        /// limited by the provided count or returning all games if no count is specified.
        /// Returns an empty list if no games are found or if the request fails.
        /// </returns>
        /// <exception cref="BusinessException">
        /// Thrown when the Steam API request fails or the response is invalid (e.g., no games found or insufficient data).
        /// </exception>
        public async Task<List<GameDto>> UserRecentlyPlayedGames(string steamId, int? count = null,
            bool? includeProgression = false, string? l = "french")
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
                
                double userProgress = 0;
                if (includeProgression == true)
                    userProgress =  await GetAchievementProgressionPercentage(steamId, appId, l);

                list.Add(new GameDto
                {
                    Id = appId,
                    Name = name,
                    Image = GetImageUrl(appId, VerticalCapsule),
                    PlaytimeForever = playtimeForever,
                    Playtime2Weeks = playtime2Weeks,
                    AchievementProgression = userProgress
                    
                });
            }
            catch (BusinessException) {}
        }
        
        memoryCache.Set(cacheKey, list, TimeSpan.FromMinutes(10));

        return list.Take(count ?? list.Count).ToList();
    }


        /// <summary>
        /// Fetches the global achievement percentages for a specific Steam game application.
        /// This endpoint provides the rarity percentage for each achievement.
        /// </summary>
        /// <param name="appId">The Steam application ID of the game.</param>
        /// <returns>
        /// A dictionary where the key is the achievement name and the value is the rarity percentage.
        /// Returns an empty dictionary if no achievements are found or if the request fails.
        /// </returns>
        private async Task<Dictionary<string, double>> GetGlobalAchievementPercentages(int appId)
        {
            string cacheKey = GetCacheKey("global_achievement_percentages", appId);

            if (memoryCache.TryGetValue(cacheKey, out Dictionary<string, double>? cachedPercentages) && cachedPercentages != null)
                return cachedPercentages;

            string apiUrl = $"{BaseUrlApi}ISteamUserStats/GetGlobalAchievementPercentagesForApp/v2/?gameid={appId}";
            
            try
            {
                JsonElement json = await FetchApi(apiUrl);

                Dictionary<string, double> percentages = new Dictionary<string, double>();

                if (json.TryGetProperty("achievementpercentages", out var achievementPercentagesElement))
                {
                    if (achievementPercentagesElement.TryGetProperty("achievements", out var achievementsElement))
                    {
                        foreach (var achievement in achievementsElement.EnumerateArray())
                        {
                            try
                            {
                                string name = achievement.GetProperty("name").GetString() ?? "";
                                string percentStr = achievement.GetProperty("percent").GetString() ?? "0";
                                
                                if (double.TryParse(percentStr, System.Globalization.CultureInfo.InvariantCulture, out double percent))
                                {
                                    percentages[name] = percent;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }

                memoryCache.Set(cacheKey, percentages, TimeSpan.FromHours(24));
                return percentages;
            }
            catch (BusinessException)
            {
                return new Dictionary<string, double>();
            }
        }

        /// <summary>
        /// Retrieves the most recent achievements unlocked by a Steam user from their recently played games.
        /// </summary>
        /// <param name="steamId">The Steam user ID for which to fetch recent achievements.</param>
        /// <param name="count">The maximum number of recent achievements to return. Default is 10.</param>
        /// <param name="includeProgression">Whether to include the rarity percentage for each achievement. Default is false.</param>
        /// <param name="l">
        /// The language code for the achievement names. If not provided, defaults to "french".
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of recent achievements
        /// ordered by unlock time (most recent first).
        /// </returns>
        public async Task<List<AchievementSchemaDto>> GetRecentAchievements(string steamId, int count = 5, bool includeProgression = false, string l = "french")
        {
            string cacheKey = GetCacheKey("recent_achievements", $"{steamId}_{count}_{includeProgression}_{l}");

            if (memoryCache.TryGetValue(cacheKey, out List<AchievementSchemaDto>? cachedAchievements) && cachedAchievements != null)
                return cachedAchievements;

            // Get recently played games
            List<GameDto> recentGames = await UserRecentlyPlayedGames(steamId);

            if (recentGames.Count == 0)
                return new List<AchievementSchemaDto>();

            List<(AchievementSchemaDto achievement, int unlockTime)> allRecentAchievements = new List<(AchievementSchemaDto, int)>();

            // For each recently played game, get the user's achievements
            foreach (var game in recentGames)
            {
                try
                {
                    List<UserAchievementDto> userAchievements = await GetUserAchievements(steamId, game.Id);
                    
                    if (userAchievements.Count == 0)
                        continue;

                    List<AchievementSchemaDto> achievementSchema = await GetAchievementsSchema(game.Id, l);
                    Dictionary<string, double>? percentages = null;

                    if (includeProgression)
                    {
                        percentages = await GetGlobalAchievementPercentages(game.Id);
                    }

                    // Map user achievements with their schema information
                    foreach (var userAchievement in userAchievements)
                    {
                        var schema = achievementSchema.FirstOrDefault(a => a.Name == userAchievement.ApiName);
                        
                        if (schema != null)
                        {
                            AchievementSchemaDto achievement = new AchievementSchemaDto
                            {
                                Name = schema.Name,
                                DisplayName = schema.DisplayName,
                                Description = schema.Description,
                                Icon = schema.Icon,
                                Rarity = includeProgression && percentages != null && percentages.TryGetValue(userAchievement.ApiName, out var percent) 
                                    ? percent 
                                    : null,
                                GameName = game.Name
                            };

                            allRecentAchievements.Add((achievement, userAchievement.UnlockTime));
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Sort by unlock time (most recent first) and take the specified count
            List<AchievementSchemaDto> result = allRecentAchievements
                .OrderByDescending(a => a.unlockTime)
                .Take(count)
                .Select(a => a.achievement)
                .ToList();

            memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }


        /// <summary>
        /// Searches for Steam games matching the provided search term using the Steam Store API.
        /// </summary>
        /// <param name="term">
        /// The search term to match against game names. Must be a non-empty string.
        /// </param>
        /// <param name="language">
        /// The language code for game names (e.g., "english", "french"). Used to filter results by localized names.
        /// </param>
        /// <param name="countryCode">
        /// The country code (e.g., "US", "FR") to filter games by region-specific availability or pricing.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="SearchGameDto"/>
        /// objects representing the matched games. Returns an empty list if no games are found or if the search fails.
        /// </returns>
        /// <exception cref="BusinessException">
        /// Thrown when the Steam API request fails (e.g., invalid response, no results, or server error).
        /// </exception>
        public async Task<List<SearchGameDto>> SearchGames(string term, string language, string countryCode)
        {
            string cacheKey = $"search_games_{term}_{language}_{countryCode}";
            if (memoryCache.TryGetValue(cacheKey, out List<SearchGameDto>? cachedGames) && cachedGames != null)
            return cachedGames;

        string apiUrl = $"https://store.steampowered.com/api/storesearch/?term={term}&l={language}&cc={countryCode}";
        JsonElement json = await FetchApi(apiUrl);

        if (!json.TryGetProperty("total", out var totalElement) || !json.TryGetProperty("items", out var itemsElement))
            throw new BusinessException(HttpStatusCode.NotFound, "NO_GAMES_FOUND");

        int total = totalElement.GetInt32();
        if (total == 0)
            return new List<SearchGameDto>();

        List<SearchGameDto> games = new List<SearchGameDto>();

        foreach (var gameElement in itemsElement.EnumerateArray())
        {
            try
            {
                if (!gameElement.TryGetProperty("type", out var typeElement) || (!typeElement.GetString()?.Equals("app", StringComparison.OrdinalIgnoreCase) ?? true))
                    continue;

                if (!gameElement.TryGetProperty("id", out var idElement) || !gameElement.TryGetProperty("name", out var nameElement) || !gameElement.TryGetProperty("tiny_image", out var tinyImageElement))
                    continue;

                int appId = idElement.GetInt32();
                string name = nameElement.GetString() ?? "";
                string tinyImage = tinyImageElement.GetString() ?? "";

                games.Add(new SearchGameDto
                {
                    AppId = appId,
                    Name = name,
                    TinyImage = tinyImage
                });
            }
            catch (Exception)
            {
                continue;
            }
        }

        memoryCache.Set(cacheKey, games, TimeSpan.FromHours(1));
        return games;
    }
      

}