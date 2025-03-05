using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;
using System.Threading.Tasks;

namespace RazorPagesDotCMS.TagHelpers
{
    /// <summary>
    /// Tag helper for rendering the footer section conditionally
    /// </summary>
    [HtmlTargetElement("footer-section")]
    public class FooterTagHelper : TagHelper
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        
        /// <summary>
        /// ViewContext for accessing Razor view functionality
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public required ViewContext ViewContext { get; set; }

        /// <summary>
        /// Determines whether the footer should be displayed
        /// </summary>
        [HtmlAttributeName("show")]
        public bool Show { get; set; }

        /// <summary>
        /// Optional copyright text to display in the footer
        /// </summary>
        [HtmlAttributeName("copyright")]
        public string Copyright { get; set; } = "© dotCMS";

        public FooterTagHelper(ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        /// <summary>
        /// Process the tag helper asynchronously to support Razor content
        /// </summary>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // If Show is false, don't render anything
            if (!Show)
            {
                output.SuppressOutput();
                return;
            }

            // Set the tag name to footer
            output.TagName = "footer";
            
            // Add the class attribute
            output.Attributes.SetAttribute("class", "block-placeholder");
            
            // Get the content passed to the tag helper
            var childContent = await output.GetChildContentAsync();
            
            // If there's no content passed, use the _FooterExample.cshtml partial view
            if (childContent.IsEmptyOrWhiteSpace)
            {
                // Render the _FooterExample.cshtml partial view
                var partial = await RenderPartialViewToStringAsync("_FooterExample", Copyright);
                output.Content.SetHtmlContent(partial);
            }
            else
            {
                // Use the content that was passed to the tag helper
                output.Content.SetHtmlContent(childContent.GetContent());
            }
        }

        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(ViewContext.HttpContext, ViewContext.RouteData, ViewContext.ActionDescriptor);
            
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                // Create a new ViewDataDictionary with the correct model type
                var viewData = new ViewDataDictionary<string>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model as string
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
