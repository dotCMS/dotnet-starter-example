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
        Task<PageResponse> GetPageAsync(PageQueryParams queryParams);
            
        /// <summary>
        /// Gets a page from the dotCMS API using GraphQL
        /// </summary>
        /// <param name="graphqlQuery">The GraphQL query</param>
        /// <returns>The page response</returns>
        Task<PageResponse> GetPageGraphqlAsync(PageQueryParams queryParams);

        Task<string> QueryGraphqlAsync(string graphqlQuery);


        Task<string> QueryGraphqlAsync(string graphqlQuery, int cacheSeconds);

        /// <summary>
        /// Gets the site navigation from the dotCMS API
        /// </summary>
        /// <param name="depth">The depth of the navigation hierarchy to retrieve (default: 4)</param>
        /// <returns>The navigation response</returns>
        Task<NavigationResponse> GetNavigationAsync(int depth = 4);
    }
}
