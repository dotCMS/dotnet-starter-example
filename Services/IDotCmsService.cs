using System.Threading.Tasks;
using RazorPagesDotCMS.Models;

namespace RazorPagesDotCMS.Services
{
    /// <summary>
    /// Interface for dotCMS API service
    /// </summary>
    public interface IDotCmsService
    {
        /// <summary>
        /// Gets a page from the dotCMS API by its path
        /// </summary>
        /// <param name="path">The page path</param>
        /// <returns>The page response</returns>
        Task<PageResponse> GetPageAsync(string path);
    }
}
