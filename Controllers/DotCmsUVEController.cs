using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using RazorPagesDotCMS.Models;
using RazorPagesDotCMS.Services;
using RazorPagesDotCMS.Filters;

namespace RazorPagesDotCMS.Controllers
{
    [Proxy]
    public class DotCmsUVEController : Controller
    {
        private readonly IDotCmsService _dotCmsService;
        private readonly ILogger<DotCmsUVEController> _logger;
        
        public DotCmsUVEController(IDotCmsService dotCmsService, ILogger<DotCmsUVEController> logger)
        {
            _dotCmsService = dotCmsService;
            _logger = logger;

            // Log when the controller is constructed to verify it's being registered
            _logger.LogInformation("DotCmsUVEController constructed");
        }

        [HttpGet, HttpPost]
        [Route("{**catchAll}")]
        public async Task<IActionResult> Index(
            string catchAll,
            [FromQuery] string? siteId = null,
            [FromQuery] string? mode = null,
            [FromQuery] string? language_id = null,
            [FromQuery] string? persona = null,
            [FromQuery] bool fireRules = false,
            [FromQuery] int depth = 1)
        {
            _logger.LogInformation($"DotCmsUVEController.Index called with path: '{catchAll}'");
            
            try
            {
                PageMode pageMode;
                if (!Enum.TryParse(mode, true, out pageMode))
                {
                    pageMode = PageMode.LIVE_MODE;  // Default to LIVE_MODE if not provided
                }
                
                // Log the query parameters
                _logger.LogInformation($"Query parameters: siteId={siteId}, mode={mode}, language_id={language_id}, " +
                                      $"persona={persona}, fireRules={fireRules}, depth={depth}");
                
                // Create a PageQueryParams object to pass to the service
                var queryParams = new PageQueryParams
                {
                    Path = catchAll,
                    Site = siteId,
                    PageMode = mode,
                    Language = language_id,
                    Persona = persona,
                    FireRules = fireRules,
                    Depth = depth
                };

                PageResponse pageResponse;
                try
                {
                    // Try to get the page using GraphQL first
                    _logger.LogInformation("Attempting to get page using GraphQL API");
                    PageResponse graphqlResponse = await _dotCmsService.GetPageGraphqlAsync(queryParams);
                    pageResponse = await _dotCmsService.GetPageAsync(queryParams);
                    string graphJSON = JsonSerializer.Serialize(graphqlResponse);
                    string restJSON = JsonSerializer.Serialize(pageResponse);
                    if (graphJSON != restJSON)
                    {
                        _logger.LogWarning("GraphQL and REST API responses differ");
                        _logger.LogWarning("GraphQL response: " + graphJSON);
                        _logger.LogWarning("REST API response: " + restJSON);
                    }

                }
                catch (Exception ex)
                {
                    // If GraphQL fails, fall back to the REST API
                    _logger.LogWarning(ex, "GraphQL API failed, falling back to REST API:" + ex.Message);
                    pageResponse = await _dotCmsService.GetPageAsync(queryParams);
                }

                // Return the view with the pageResponse
                return View("~/Views/DotCmsView/Index.cshtml", pageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("contentlet-example/{contentletId}")]
        public IActionResult ContentletExample(string contentletId)
        {
            _logger.LogInformation($"ContentletExample called with contentletId: '{contentletId}'");
            
            try
            {
                // In a real implementation, you would fetch the contentlet from DotCMS
                // For this example, we'll create a sample Banner contentlet
                var contentlet = new Contentlet
                {
                    Title = "Example Banner",
                    ContentType = "Banner",
                    AdditionalProperties = new System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>()
                };

                // Add sample properties that a Banner might have
                using (var jsonDoc = System.Text.Json.JsonDocument.Parse(@"{
                    ""image"": ""/path/to/banner-image.jpg"",
                    ""link"": ""https://www.dotcms.com"",
                    ""altText"": ""DotCMS Banner"",
                    ""description"": ""<p>This is an example banner created programmatically.</p>""
                }"))
                {
                    foreach (var property in jsonDoc.RootElement.EnumerateObject())
                    {
                        contentlet.AdditionalProperties[property.Name] = property.Value.Clone();
                    }
                }

                // Return the example view with the contentlet
                return View("~/Views/Shared/_ContentletHelperExample.cshtml", contentlet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contentlet example request");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
