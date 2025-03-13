using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RazorPagesDotCMS.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RazorPagesDotCMS.TagHelpers
{
    /// <summary>
    /// Tag helper for rendering contentlets based on their ContentType
    /// </summary>
    [HtmlTargetElement("contentlet-renderer")]
    public class ContentletTagHelper : TagHelper
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ILogger<ContentletTagHelper> _logger;
        
        /// <summary>
        /// ViewContext for accessing Razor view functionality
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public required ViewContext ViewContext { get; set; }

        /// <summary>
        /// The contentlet to render
        /// </summary>
        [HtmlAttributeName("contentlet")]
        public required Contentlet Contentlet { get; set; }

        public ContentletTagHelper(
            ICompositeViewEngine viewEngine, 
            ITempDataProvider tempDataProvider,
            ILogger<ContentletTagHelper> logger)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Process the tag helper asynchronously
        /// </summary>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Contentlet == null)
            {
                _logger.LogWarning("Contentlet is null, cannot render");
                output.SuppressOutput();
                return;
            }

            // Remove the original tag
            output.TagName = null;

            try
            {
                // Determine the content type
                string contentType = Contentlet.ContentType ?? "Generic";
                
                // Try to find the view in ContentTypes directory or Shared/ContentTypes
                string viewName = $"../DotCmsView/ContentTypes/{contentType}";
                
                _logger.LogInformation($"Looking for view: {viewName}");
                
                var actionContext = new ActionContext(ViewContext.HttpContext, ViewContext.RouteData, ViewContext.ActionDescriptor);
                var viewResult = _viewEngine.FindView(actionContext, viewName, false);
                
                if (viewResult.View == null)
                {
                    _logger.LogWarning($"View for ContentType '{contentType}' not found. Falling back to generic rendering.");
                    
                    // Fallback to generic rendering
                    output.Content.SetHtmlContent(RenderGenericContentlet(Contentlet));
                    return;
                }

                // Render the appropriate partial view
                var renderedContent = await RenderPartialViewToStringAsync(viewName, Contentlet);
                output.Content.SetHtmlContent(renderedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rendering contentlet of type '{Contentlet.ContentType}'");
                output.Content.SetHtmlContent($"<div class=\"error\">Error rendering contentlet: {ex.Message}</div>");
            }
        }

        private string RenderGenericContentlet(Contentlet contentlet)
        {
            var content = $"<div class=\"contentlet contentlet-{contentlet.ContentType?.ToLower()}\">";
            content += $"<h4>{contentlet.Title}</h4>";
            content += $"<div style='margin:4px;'>No component found for <span style='font-family:monospace;font-weight:bold'>{contentlet.ContentType}</span></div>";
            if (contentlet.AdditionalProperties != null)
            {
                content += "<div class=\"contentlet-properties\">";
                foreach (var prop in contentlet.AdditionalProperties)
                {
                    content += $"<div class=\"contentlet-property\"><strong>{prop.Key}:</strong> ";
                    
                    if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        content += $"{prop.Value.GetString()}";
                    }
                    else
                    {
                        content += $"{prop.Value}";
                    }
                    
                    content += "</div>";
                }
                content += "</div>";
            }
            
            content += "</div>";
            return content;
        }

        private async Task<string> RenderPartialViewToStringAsync(string viewPath, object model)
        {
            var actionContext = new ActionContext(ViewContext.HttpContext, ViewContext.RouteData, ViewContext.ActionDescriptor);
            
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(actionContext, viewPath, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewPath} does not match any available view");
                }

                // Create a new ViewDataDictionary with the model
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                };

                // Copy any other ViewData items from the parent context
                foreach (var kvp in ViewContext.ViewData)
                {
                    if (kvp.Key != "Model")
                    {
                        viewData[kvp.Key] = kvp.Value;
                    }
                }

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
