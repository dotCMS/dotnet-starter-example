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
            [FromQuery] string? personaId = null,
            [FromQuery] bool fireRules = false,
            [FromQuery] int depth = 1)
        {
            _logger.LogInformation("DotCmsUVEController.Index called with path: {Path}", catchAll);
            
            try
            {
                // Validate the path parameter
                if (string.IsNullOrWhiteSpace(catchAll))
                {
                    catchAll = "/"; // Default to root path if not provided
                }
                
                PageMode pageMode;
                if (!Enum.TryParse(mode, true, out pageMode))
                {
                    pageMode = PageMode.LIVE_MODE;  // Default to LIVE_MODE if not provided
                }
                
                // Log the query parameters using structured logging
                _logger.LogInformation("Query parameters: siteId={SiteId}, mode={Mode}, language_id={LanguageId}, " +
                                      "persona={PersonaId}, fireRules={FireRules}, depth={Depth}",
                                      siteId, pageMode, language_id, personaId, fireRules, depth);
                
                // Create a PageQueryParams object to pass to the service
                var queryParams = new PageQueryParams
                {
                    Path = catchAll,
                    Site = siteId,
                    PageMode = pageMode.ToString(),  // Use the parsed pageMode instead of the raw string
                    Language = language_id,
                    Persona = personaId,
                    FireRules = fireRules,
                    Depth = depth
                };

                PageResponse pageResponse = await _dotCmsService.GetPageAsync(queryParams);
      

                // Return the view with the pageResponse
                return View("~/Views/DotCmsView/Index.cshtml", pageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for path: {Path}", catchAll);
                
                // Return a generic error message to avoid exposing internal details
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("contentlet-example/{contentletId}")]
        public IActionResult ContentletExample(string contentletId)
        {
            _logger.LogInformation("ContentletExample called with contentletId: {ContentletId}", contentletId);
            
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
                var jsonString = @"{
                    ""image"": ""/path/to/banner-image.jpg"",
                    ""link"": ""https://www.dotcms.com"",
                    ""altText"": ""DotCMS Banner"",
                    ""description"": ""<p>This is an example banner created programmatically.</p>""
                }";
                
                // Parse the JSON string and store the elements without cloning
                // This avoids the resource leak from Clone() method
                using (var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonString))
                {
                    var json = jsonDoc.RootElement.GetRawText();
                    using (var newDoc = System.Text.Json.JsonDocument.Parse(json))
                    {
                        foreach (var property in newDoc.RootElement.EnumerateObject())
                        {
                            contentlet.AdditionalProperties[property.Name] = property.Value;
                        }
                    }
                }

                // Return the example view with the contentlet
                return View("~/Views/Shared/_ContentletHelperExample.cshtml", contentlet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contentlet example request for ID: {ContentletId}", contentletId);
                
                // Return a generic error message to avoid exposing internal details
                return StatusCode(500, "An error occurred while processing the contentlet example.");
            }
        }
    }
}
