using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RazorPagesDotCMS.Models;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RazorPagesDotCMS.TagHelpers
{
    /// <summary>
    /// Tag helper for rendering the footer section conditionally
    /// </summary>
    [HtmlTargetElement("footer-section")]
    public class FooterTagHelper : TagHelper
    {
        private readonly HtmlEncoder _htmlEncoder;
        
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
        public string Copyright { get; set; } = "© DotCMS";

        public FooterTagHelper(HtmlEncoder htmlEncoder)
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

            // Set the tag name to footer
            output.TagName = "footer";
            
            // Add the class attribute
            output.Attributes.SetAttribute("class", "block-placeholder");
            
            // Get the content passed to the tag helper
            var childContent = await output.GetChildContentAsync();
            
            // If there's no content passed, use default footer content
            if (childContent.IsEmptyOrWhiteSpace)
            {
                var currentYear = System.DateTime.Now.Year;
                output.Content.SetHtmlContent($@"
                    <div class=""footer-content"">
                        <div class=""copyright"">
                            <p>{_htmlEncoder.Encode(Copyright)} {currentYear}. All rights reserved.</p>
                        </div>
                        <div class=""footer-links"">
                            <ul>
                                <li><a href=""/privacy"">Privacy Policy</a></li>
                                <li><a href=""/terms"">Terms of Service</a></li>
                                <li><a href=""/contact"">Contact Us</a></li>
                            </ul>
                        </div>
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
