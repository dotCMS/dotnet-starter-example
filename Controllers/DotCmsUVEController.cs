using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RazorPagesDotCMS.Models;
using RazorPagesDotCMS.Services;

namespace RazorPagesDotCMS.Controllers
{
    [Route("{**catchAll}")]
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
                // Parse the mode enum if provided
                PageMode? pageMode = null;
                if (!string.IsNullOrEmpty(mode)){
                    string modeUpper = mode.ToUpper().EndsWith("_MODE") ? mode.ToUpper() : mode.ToUpper() + "_MODE";
                    if (Enum.TryParse<PageMode>(modeUpper, out var parsedMode))
                    {
                        pageMode = parsedMode;
                    }

                }
                
                // Log the query parameters
                _logger.LogInformation($"Query parameters: siteId={siteId}, mode={mode}, language_id={language_id}, " +
                                      $"persona={persona}, fireRules={fireRules}, depth={depth}");
                
                // Get the page from the dotCMS API using our service with all parameters
                var pageResponse = await _dotCmsService.GetPageAsync(
                    catchAll,
                    siteId,
                    pageMode,
                    language_id,
                    persona,
                    fireRules,
                    depth);

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
