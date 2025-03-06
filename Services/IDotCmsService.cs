using System.Text.Json;
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
        /// <param name="siteId">Optional site ID</param>
        /// <param name="mode">Optional view mode (EDIT_MODE, PREVIEW_MODE, LIVE_MODE)</param>
        /// <param name="languageId">Optional language ID</param>
        /// <param name="personaId">Optional persona ID</param>
        /// <param name="fireRules">Whether to fire rules (default: false)</param>
        /// <param name="depth">Depth of the content to retrieve (default: 1)</param>
        /// <returns>The page response</returns>
        Task<PageResponse> GetPageAsync(
            string path, 
            string? siteId = null, 
            PageMode? mode = null, 
            string? languageId = null, 
            string? personaId = null, 
            bool fireRules = false, 
            int depth = 1);
            
        /// <summary>
        /// Gets a page from the dotCMS API using GraphQL
        /// </summary>
        /// <param name="graphqlQuery">The GraphQL query</param>
        /// <returns>The page response</returns>
        Task<PageResponse> GetPageGraphqlAsync(
            string path,
            string? siteId = null,
            PageMode? mode = PageMode.LIVE_MODE,
            string? languageId = null,
            string? persona = null,
            bool fireRules = false);
    
        Task<string> GetGraphqlAsync(string graphqlQuery);


        Task<string> GetGraphqlAsync(string graphqlQuery, int cacheSeconds);

    }
}
