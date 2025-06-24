using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using RazorPagesDotCMS.Models;
using LazyCache;

namespace RazorPagesDotCMS.Services
{
    /// <summary>
    /// Service for interacting with the dotCMS API
    /// </summary>
    public class DotCmsService : IDotCmsService
    {
        private readonly ModelHelper _modelHelper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DotCmsService> _logger;
        private readonly IAppCache _cache;

        private readonly string _apiHost;
        private readonly string _apiAuth;
        private readonly int _defaultCacheTtl;

        // Constants for configuration keys and default values
        private const string ApiHostKey = "dotCMS:ApiHost";
        private const string ApiTokenKey = "dotCMS:ApiToken";
        private const string ApiUserNameKey = "dotCMS:ApiUserName";
        private const string ApiPasswordKey = "dotCMS:ApiPassword";
        private const string CacheTtlKey = "dotCMS:CacheTTL";
        private const int DefaultCacheTtlSeconds = 120;
        private const int DefaultNavigationCacheTtlSeconds = 60;
        private const int DefaultGraphqlCacheTtlSeconds = 60;

        // JSON serializer options (reused across methods)
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCmsService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="logger">The logger</param>
        /// <param name="cache">LazyCache instance</param>
        /// <param name="modelHelper">Model helper for GraphQL conversions</param>
        /// <exception cref="ArgumentNullException">Thrown when required dependencies are null</exception>
        /// <exception cref="InvalidOperationException">Thrown when required configuration is missing</exception>
        public DotCmsService(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<DotCmsService> logger, 
            IAppCache cache,
            ModelHelper modelHelper)
        {
            _httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _modelHelper = modelHelper ?? throw new ArgumentNullException(nameof(modelHelper));

            // Initialize configuration values with validation
            _apiHost = GetRequiredConfigurationValue(ApiHostKey);
            _apiAuth = BuildAuthorizationHeader();
            _defaultCacheTtl = GetCacheTtlFromConfiguration();

            _logger.LogInformation("DotCmsService initialized with API host: {ApiHost}", _apiHost);
        }

        /// <summary>
        /// Gets a page from the dotCMS API by its path
        /// </summary>
        /// <param name="queryParams">Query parameters for the page request</param>
        /// <returns>The page response</returns>
        public async Task<PageResponse> GetPageAsync(PageQueryParams queryParams)
        {
            ArgumentNullException.ThrowIfNull(queryParams);

            try
            {
                var normalizedPath = NormalizePath(queryParams.Path);
                var requestUrl = BuildPageApiUrl(normalizedPath, queryParams);
                var mode = ParsePageMode(queryParams.PageMode);
                var cacheSeconds = queryParams.CacheSeconds ?? (mode == PageMode.LIVE_MODE ? DefaultCacheTtlSeconds : 0);

                return await _cache.GetOrAdd(requestUrl, async () =>
                {
                    _logger.LogInformation("Requesting page from: {RequestUrl}", requestUrl);
                    return await ExecutePageApiRequest(requestUrl);
                }, DateTimeOffset.Now.AddSeconds(cacheSeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPageAsync for path: {Path}", queryParams.Path);
                throw;
            }
        }

        /// <summary>
        /// Gets a page from the dotCMS API using GraphQL
        /// </summary>
        /// <param name="queryParams">Query parameters for the GraphQL request</param>
        /// <returns>The page response</returns>
        public async Task<PageResponse> GetPageGraphqlAsync(PageQueryParams queryParams)
        {
            ArgumentNullException.ThrowIfNull(queryParams);

            try
            {
                var mode = ParsePageMode(queryParams.PageMode);
                var graphqlQuery = BuildGraphqlPageQuery(queryParams.Path, queryParams.Site, mode, 
                    queryParams.Language, queryParams.Persona, queryParams.FireRules ?? false);

                var cacheSeconds = queryParams.CacheSeconds ?? (mode == PageMode.LIVE_MODE ? DefaultGraphqlCacheTtlSeconds : 0);
                var content = await QueryGraphqlAsync(graphqlQuery, cacheSeconds);

                return _modelHelper.ConvertGraphqlToPageResponse(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPageGraphqlAsync for path: {Path}", queryParams.Path);
                throw;
            }
        }

        /// <summary>
        /// Executes a GraphQL query against the dotCMS API
        /// </summary>
        /// <param name="graphqlQuery">The GraphQL query string</param>
        /// <returns>The raw response content</returns>
        public async Task<string> QueryGraphqlAsync(string graphqlQuery)
        {
            return await QueryGraphqlAsync(graphqlQuery, 0);
        }

        /// <summary>
        /// Executes a GraphQL query against the dotCMS API with caching
        /// </summary>
        /// <param name="graphqlQuery">The GraphQL query string</param>
        /// <param name="cacheSeconds">Cache duration in seconds</param>
        /// <returns>The raw response content</returns>
        public async Task<string> QueryGraphqlAsync(string graphqlQuery, int cacheSeconds)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(graphqlQuery);

            try
            {
                var qid = CalculateSHA256(graphqlQuery);

                return await _cache.GetOrAdd(qid, async () =>
                {
                    var requestUrl = BuildGraphqlApiUrl(qid);
                    _logger.LogInformation("Requesting GraphQL from: {RequestUrl}", requestUrl);
                    
                    return await ExecuteGraphqlApiRequest(requestUrl, graphqlQuery);
                }, DateTimeOffset.Now.AddSeconds(cacheSeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QueryGraphqlAsync");
                throw;
            }
        }

        /// <summary>
        /// Gets the site navigation from the dotCMS API
        /// </summary>
        /// <param name="depth">The depth of the navigation hierarchy to retrieve</param>
        /// <returns>The navigation response</returns>
        public async Task<NavigationResponse> GetNavigationAsync(int depth = 4)
        {
            // Validate depth parameter
            if (depth < 0)
            {
                throw new ArgumentException("Depth must be a non-negative value", nameof(depth));
            }
            
            if (depth > 10)
            {
                _logger.LogWarning("Navigation depth {Depth} exceeds recommended maximum of 10. This may impact performance.", depth);
            }
            
            try
            {
                var requestUrl = $"{_apiHost}/api/v1/nav/?depth={depth}";

                return await _cache.GetOrAdd(requestUrl, async () =>
                {
                    _logger.LogInformation("Requesting navigation from: {RequestUrl}", requestUrl);
                    return await ExecuteNavigationApiRequest(requestUrl);
                }, DateTimeOffset.Now.AddSeconds(DefaultNavigationCacheTtlSeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetNavigationAsync with depth: {Depth}", depth);
                throw;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Gets a required configuration value and throws if missing
        /// </summary>
        private string GetRequiredConfigurationValue(string key)
        {
            var value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Required configuration value '{key}' is missing or empty");
            }
            return value;
        }

        /// <summary>
        /// Builds the authorization header based on available configuration
        /// </summary>
        private string BuildAuthorizationHeader()
        {
            var apiToken = _configuration[ApiTokenKey];
            
            if (!string.IsNullOrWhiteSpace(apiToken))
            {
                return $"Bearer {apiToken}";
            }
            _logger.LogWarning($"No API Token found, trying Basic Auth, which is subpar and will force a login every page fetch");

            var userName = GetRequiredConfigurationValue(ApiUserNameKey);
            var password = GetRequiredConfigurationValue(ApiPasswordKey);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
            
            return $"Basic {credentials}";
        }

        /// <summary>
        /// Gets cache TTL from configuration with fallback to default
        /// </summary>
        private int GetCacheTtlFromConfiguration()
        {
            var cacheTtlString = _configuration[CacheTtlKey];
            
            if (string.IsNullOrWhiteSpace(cacheTtlString))
            {
                return DefaultCacheTtlSeconds;
            }

            if (int.TryParse(cacheTtlString, out var cacheTtl))
            {
                return cacheTtl;
            }

            _logger.LogWarning("Invalid cache TTL configuration value: {CacheTtl}. Using default: {DefaultTtl}", 
                cacheTtlString, DefaultCacheTtlSeconds);
            
            return DefaultCacheTtlSeconds;
        }

        /// <summary>
        /// Parses page mode from string with fallback to default
        /// </summary>
        private static PageMode ParsePageMode(string? pageModeString)
        {
            if (Enum.TryParse<PageMode>(pageModeString, true, out var mode))
            {
                return mode;
            }
            return PageMode.LIVE_MODE;
        }

        /// <summary>
        /// Builds the page API URL with query parameters
        /// </summary>
        private string BuildPageApiUrl(string normalizedPath, PageQueryParams queryParams)
        {
            var baseUrl = $"{_apiHost}/api/v1/page/json{normalizedPath}";
            var uriBuilder = new UriBuilder(baseUrl);
            var queryCollection = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(queryParams.Site))
            {
                queryCollection["siteId"] = queryParams.Site;
            }

            var mode = ParsePageMode(queryParams.PageMode);
            queryCollection["mode"] = mode.ToString();

            if (!string.IsNullOrWhiteSpace(queryParams.Language))
            {
                queryCollection["language_id"] = queryParams.Language;
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Persona))
            {
                queryCollection["com.dotmarketing.persona.id"] = queryParams.Persona;
            }

            queryCollection["fireRules"] = (queryParams.FireRules ?? false).ToString().ToLowerInvariant();
            queryCollection["depth"] = (queryParams.Depth ?? 0).ToString();

            uriBuilder.Query = queryCollection.ToString();
            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Builds the GraphQL API URL with query parameters
        /// </summary>
        private string BuildGraphqlApiUrl(string qid)
        {
            var uriBuilder = new UriBuilder($"{_apiHost}/api/v1/graphql");
            var queryCollection = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryCollection["qid"] = qid;
            uriBuilder.Query = queryCollection.ToString();
            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Executes a page API request
        /// </summary>
        private async Task<PageResponse> ExecutePageApiRequest(string requestUrl)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", _apiAuth);

            using var response = await _httpClient.SendAsync(request);
            await EnsureSuccessStatusCode(response);

            var content = await response.Content.ReadAsStringAsync();
            var pageResponse = JsonSerializer.Deserialize<PageResponse>(content, JsonOptions);
            
            return pageResponse ?? throw new JsonException("Failed to deserialize page response");
        }

        /// <summary>
        /// Executes a GraphQL API request
        /// </summary>
        private async Task<string> ExecuteGraphqlApiRequest(string requestUrl, string graphqlQuery)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Add("Authorization", _apiAuth);

            var requestBody = JsonSerializer.Serialize(new { query = graphqlQuery });
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            await EnsureSuccessStatusCode(response);

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Executes a navigation API request
        /// </summary>
        private async Task<NavigationResponse> ExecuteNavigationApiRequest(string requestUrl)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", _apiAuth);

            using var response = await _httpClient.SendAsync(request);
            await EnsureSuccessStatusCode(response);

            var content = await response.Content.ReadAsStringAsync();
            var navigationResponse = JsonSerializer.Deserialize<NavigationResponse>(content, JsonOptions);
            
            return navigationResponse ?? throw new JsonException("Failed to deserialize navigation response");
        }

        /// <summary>
        /// Ensures HTTP response has success status code
        /// </summary>
        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("API returned status code: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"API returned status code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Builds a GraphQL page query
        /// </summary>
        private string BuildGraphqlPageQuery(
            string path,
            string? siteId = null,
            PageMode mode = PageMode.LIVE_MODE,
            string? languageId = null,
            string? personaId = null,
            bool fireRules = false)
        {
            var normalizedPath = NormalizePath(path);
            
            // Escape special characters to prevent GraphQL injection
            normalizedPath = EscapeGraphqlString(normalizedPath);
            
            var queryBuilder = new StringBuilder($"page(url: \"{normalizedPath}\"");
            
            queryBuilder.Append($",pageMode:\"{mode}\"");
            queryBuilder.Append($",fireRules:{fireRules.ToString().ToLowerInvariant()}");
            
            if (!string.IsNullOrWhiteSpace(personaId))
            {
                queryBuilder.Append($",personaId: \"{EscapeGraphqlString(personaId)}\"");
            }
            
            if (!string.IsNullOrWhiteSpace(siteId))
            {
                queryBuilder.Append($",site: \"{EscapeGraphqlString(siteId)}\"");
            }
            
            if (!string.IsNullOrWhiteSpace(languageId))
            {
                queryBuilder.Append($",languageId: \"{EscapeGraphqlString(languageId)}\"");
            }
            
            queryBuilder.Append(')');

            var finalGraphql = GraphqlPageTemplate.Replace("${query}", queryBuilder.ToString());
            _logger.LogDebug("Generated GraphQL query: {GraphqlQuery}", finalGraphql);
            
            return finalGraphql;
        }
        
        /// <summary>
        /// Escapes special characters in GraphQL strings to prevent injection attacks
        /// </summary>
        private static string EscapeGraphqlString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            // Escape backslashes first, then quotes, then other special characters
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f");
        }

        /// <summary>
        /// Normalizes a path for API requests
        /// </summary>
        private static string NormalizePath(string? path)
        {
            path ??= "/";
            
            // Security: Prevent path traversal attacks
            if (path.Contains("..") || path.Contains("~") || path.Contains("\\"))
            {
                throw new ArgumentException("Invalid path: Path traversal patterns are not allowed");
            }
            
            // Remove any double slashes
            path = System.Text.RegularExpressions.Regex.Replace(path, @"/{2,}", "/");
            
            if (!path.StartsWith('/'))
            {
                path = "/" + path;
            }
            
            if (path.EndsWith('/') && path.Length > 1)
            {
                path += "index";
            }
            
            return path;
        }

        /// <summary>
        /// Calculates SHA256 hash of input string
        /// </summary>
        private static string CalculateSHA256(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        #endregion

        #region GraphQL Template

        private const string GraphqlPageTemplate = @"
{
    ${query} 
    {
        _map
        urlContentMap {
            identifier
            modDate
            publishDate
            creationDate
            title
            baseType
            inode
            archived
            _map
            urlMap
            working
            locked
            contentType
            live
        }
        title
        friendlyName
        description
        tags
        canEdit
        canLock
        canRead
        template {
            inode
            identifier
            drawed
        }
        containers {
            path
            identifier
            maxContentlets
            containerStructures {
                contentTypeVar
                containerId
                containerInode
                structureId
            }
            containerContentlets {
                uuid
                contentlets {
                    identifier
                    modDate
                    publishDate
                    creationDate
                    title
                    baseType
                    inode
                    archived
                    _map
                    urlMap
                    working
                    locked
                    contentType
                    live
                }
            }
        }
        layout {
            header
            footer
            sidebar {
                widthPercent
                width
                location
            }
            body {
                rows {
                    columns {
                        leftOffset
                        styleClass
                        width
                        left
                        containers {
                            identifier
                            uuid
                        }
                    }
                }
            }
        }
        viewAs {
            visitor {
                persona {
                    name
                    keyTag
                    identifier
                }
                device
                tags {
                    tag
                    count
                }
                geo {
                    continent
                    country
                    subdivision
                    city
                    timezone
                    latitude
                    longitude
                    continentCode
                }
            }
            language {
                id
                languageCode
                countryCode
                language
                country
            }
        }
    }
}";

        #endregion
    }
}
