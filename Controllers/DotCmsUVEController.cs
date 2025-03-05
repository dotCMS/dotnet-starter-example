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
        private readonly string? _apiAuth;
        
        public DotCmsUVEController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DotCmsUVEController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
            _apiHost = _configuration["dotCMS:ApiHost"];
            _apiAuth = string.IsNullOrEmpty(_configuration["dotCMS:ApiToken"]) 
                ? "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration["dotCMS:ApiUserName"] + ":" + _configuration["dotCMS:ApiPassword"]))
                : "Bearer " + _configuration["dotCMS:ApiToken"];

            

            // Log when the controller is constructed to verify it's being registered
            _logger.LogInformation("DotCmsUVEController constructed");
        }

        [HttpGet, HttpPost]
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
                
                request.Headers.Add("Authorization", _apiAuth);
    


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
