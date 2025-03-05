using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RazorPagesDotCMS.Models;

namespace RazorPagesDotCMS.Services
{
    /// <summary>
    /// Service for interacting with the dotCMS API
    /// </summary>
    public class DotCmsService : IDotCmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DotCmsService> _logger;

        private readonly string? _apiHost;
        private readonly string? _apiAuth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotCmsService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="logger">The logger</param>
        public DotCmsService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DotCmsService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
            
            _apiHost = _configuration["dotCMS:ApiHost"];
            _apiAuth = string.IsNullOrEmpty(_configuration["dotCMS:ApiToken"]) 
                ? "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration["dotCMS:ApiUserName"] + ":" + _configuration["dotCMS:ApiPassword"]))
                : "Bearer " + _configuration["dotCMS:ApiToken"];

            _logger.LogInformation("DotCmsService constructed");
        }

        /// <summary>
        /// Gets a page from the dotCMS API by its path
        /// </summary>
        /// <param name="path">The page path</param>
        /// <param name="siteId">Optional site ID</param>
        /// <param name="mode">Optional view mode (EDIT_MODE, PREVIEW_MODE, LIVE_MODE)</param>
        /// <param name="languageId">Optional language ID</param>
        /// <param name="persona">Optional persona ID</param>
        /// <param name="fireRules">Whether to fire rules (default: false)</param>
        /// <param name="depth">Depth of the content to retrieve (default: 1)</param>
        /// <returns>The page response</returns>
        public async Task<PageResponse> GetPageAsync(
            string path, 
            string? siteId = null, 
            PageMode? mode = PageMode.LIVE_MODE, 
            string? languageId = null, 
            string? persona = null, 
            bool fireRules = false, 
            int depth = 1)
        {
            try
            {
                // Normalize the path
                path = path ?? "/";
                path = path.EndsWith("/") ? $"{path}index" : path;
                if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
                {
                    path = "/" + path;
                }

                // Create the request to the dotCMS API
                var requestUrl = $"{_apiHost}/api/v1/page/json{path}";
                
                // Add query parameters if provided
                var uriBuilder = new UriBuilder(requestUrl);
                var query = new System.Collections.Specialized.NameValueCollection();
                
                if (!string.IsNullOrEmpty(siteId))
                {
                    query["siteId"] = siteId;
                }
                
                if (mode.HasValue)
                {
                    query["mode"] = mode.Value.ToString();
                }
                
                if (!string.IsNullOrEmpty(languageId))
                {
                    query["language_id"] = languageId;
                }
                
                if (!string.IsNullOrEmpty(persona))
                {
                    query["persona"] = persona;
                }
                
                // Always include fireRules and depth parameters
                query["fireRules"] = fireRules.ToString().ToLower();
                query["depth"] = depth.ToString();
                
                // Convert the query collection to a query string
                var queryString = string.Join("&", Array.ConvertAll(
                    query.AllKeys, 
                    key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(query[key])}"
                ));
                
                if (!string.IsNullOrEmpty(queryString))
                {
                    uriBuilder.Query = queryString;
                }
                
                var finalRequestUrl = uriBuilder.Uri.ToString();
                _logger.LogInformation($"Requesting page from: {finalRequestUrl}");

                var request = new HttpRequestMessage(HttpMethod.Get, finalRequestUrl);
                request.Headers.Add("Authorization", _apiAuth);

                // Send the request to dotCMS
                var response = await _httpClient.SendAsync(request);

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"dotCMS API returned status code: {response.StatusCode}");
                    throw new HttpRequestException($"dotCMS API returned status code: {response.StatusCode}");
                }

                // Read the response content
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the response to PageResponse model
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var pageResponse = JsonSerializer.Deserialize<PageResponse>(content, options);
                return pageResponse ?? throw new JsonException("Failed to deserialize page response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting page from dotCMS API");
                throw;
            }
        }
    }
}
