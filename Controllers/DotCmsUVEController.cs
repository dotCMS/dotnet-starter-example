using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> Index(string catchAll)
        {
            _logger.LogInformation($"DotCmsUVEController.Index called with path: '{catchAll}'");
            
            try
            {
                // Get the page from the dotCMS API using our service
                var pageResponse = await _dotCmsService.GetPageAsync(catchAll);

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
