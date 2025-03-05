using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RazorPagesDotCMS.Models;

namespace RazorPagesDotCMS.Controllers
{
    [Route("{**catchAll}")]
    public class DotCmsUVEController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DotCmsUVEController> _logger;

        private readonly string? _apiHost;
        private readonly string? _apiToken;  
        private readonly string? _username;
        private readonly string? _password;
        
        public DotCmsUVEController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DotCmsUVEController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;

            _apiHost = _configuration["dotCMS:ApiHost"];
            _apiToken = _configuration["dotCMS:ApiToken"];
            _username = _configuration["dotCMS:ApiUserName"];
            _password = _configuration["dotCMS:ApiPassword"];

            // Log when the controller is constructed to verify it's being registered
            _logger.LogInformation("DotCmsUVEController constructed");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string catchAll)
        {
            _logger.LogInformation($"DotCmsUVEController.Index called with path: '{catchAll}'");
            
            try
            {
                // Normalize the path
                string path = catchAll ?? "/";
                path = path.EndsWith("/") ? $"{path}index" : path;
                if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
                {
                    path = "/" + path;
                }

                // Create the request to the dotCMS API
                var requestUrl = $"{_apiHost}/api/v1/page/json{path}";
                _logger.LogInformation($"Forwarding request to: {requestUrl}");

                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                
                // Add authentication
                if (!string.IsNullOrEmpty(_apiToken))
                {
                    // Use Bearer token authentication if available
                    _logger.LogInformation("Using Bearer token authentication");
                    request.Headers.Add("Authorization", $"Bearer {_apiToken}");
                }
                else if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    // Fall back to Basic authentication if username and password are available
                    _logger.LogInformation("Using Basic authentication");
                    var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
                }
                else
                {
                    _logger.LogWarning("No authentication credentials available");
                }

                // Send the request to dotCMS
                var response = await _httpClient.SendAsync(request);

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"dotCMS API returned status code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                // Read the response content
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the response to PageResponse model
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var pageResponse = JsonSerializer.Deserialize<PageResponse>(content, options);

                // Return the view with the pageResponse
                return View("~/Views/DotCmsView/Index.cshtml", pageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
