using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RazorPagesDotCMS.Models;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RazorPagesDotCMS.TagHelpers
{
    /// <summary>
    /// Tag helper for rendering the header section conditionally
    /// </summary>
    [HtmlTargetElement("header-section")]
    public class HeaderTagHelper : TagHelper
    {
        private readonly HtmlEncoder _htmlEncoder;
        
        /// <summary>
        /// ViewContext for accessing Razor view functionality
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public required ViewContext ViewContext { get; set; }

        /// <summary>
        /// Determines whether the header should be displayed
        /// </summary>
        [HtmlAttributeName("show")]
        public bool Show { get; set; }

        /// <summary>
        /// Optional title to display in the header
        /// </summary>
        [HtmlAttributeName("title")]
        public string Title { get; set; } = "DotCMS Header";

        public HeaderTagHelper(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
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

            // Set the tag name to header
            output.TagName = "header";
            
            // Add the class attribute
            output.Attributes.SetAttribute("class", "block-placeholder");
            
            // Get the content passed to the tag helper
            var childContent = await output.GetChildContentAsync();
            
            // If there's no content passed, use default header content
            if (childContent.IsEmptyOrWhiteSpace)
            {
                output.Content.SetHtmlContent($@"
                    <div class=""header-content"">
                        <div class=""logo"">
                            <a href=""/"">
                                <img src=""/images/logo.png"" alt=""Logo"" onerror=""this.style.display='none'"" />
                                <span>{_htmlEncoder.Encode(Title)}</span>
                            </a>
                        </div>
                        <nav class=""main-nav"">
                            <ul>
                                <li><a href=""/"">Home</a></li>
                                <li><a href=""/about"">About</a></li>
                                <li><a href=""/contact"">Contact</a></li>
                            </ul>
                        </nav>
                    </div>
                ");
            }
            else
            {
                // Use the content that was passed to the tag helper
                output.Content.SetHtmlContent(childContent.GetContent());
            }
        }
    }
}
