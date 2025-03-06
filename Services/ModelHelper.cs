
using System.Text.Json;

namespace RazorPagesDotCMS.Models
{


    public class ModelHelper
    {
        private readonly ILogger<ModelHelper> _logger;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public ModelHelper(ILogger<ModelHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Converts a GraphQL response to a PageResponse model
        /// </summary>
        /// <param name="graphqlResponse">The JSON string response from the GraphQL API</param>
        /// <returns>A PageResponse object</returns>
        public PageResponse ConvertGraphqlToPageResponse(string graphqlResponse)
        {


            try
            {
                _logger.LogInformation("Starting GraphQL response conversion");
                _logger.LogDebug($"Raw GraphQL response: {graphqlResponse}");

                // First, try to parse the response as a JsonDocument to check its structure
                using (var document = JsonDocument.Parse(graphqlResponse))
                {
                    var root = document.RootElement;
                    if (root.TryGetProperty("data", out var dataElement))
                    {
                        _logger.LogDebug("Found 'data' property in response");
                        if (dataElement.TryGetProperty("page", out var pageElement))
                        {
                            _logger.LogDebug("Found 'page' property in data");
                        }
                        else
                        {
                            _logger.LogWarning("No 'page' property found in 'data'");
                            // Log the actual structure
                            _logger.LogDebug($"Data element structure: {JsonSerializer.Serialize(dataElement)}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No 'data' property found in response");
                        // Log the actual structure
                        _logger.LogDebug($"Root element properties: {string.Join(", ", root.EnumerateObject().Select(p => p.Name))}");
                    }
                }

                // Try to deserialize the GraphQL response
                GraphqlResponse? graphqlData = null;
                try
                {
                    graphqlData = JsonSerializer.Deserialize<GraphqlResponse>(graphqlResponse, options);

                    if (graphqlData == null)
                    {
                        _logger.LogError("Failed to deserialize GraphQL response to GraphqlResponse object");
                        throw new JsonException("Failed to deserialize GraphQL response");
                    }

                    if (graphqlData.Data == null)
                    {
                        _logger.LogError("GraphQL response has no 'data' property");
                        throw new JsonException("GraphQL response has no 'data' property");
                    }

                    if (graphqlData.Data.Page == null)
                    {
                        _logger.LogError("GraphQL response has no 'page' property in 'data'");
                        throw new JsonException("GraphQL response has no 'page' property in 'data'");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing GraphQL response");

                    // Try to handle errors in the GraphQL response
                    using (var document = JsonDocument.Parse(graphqlResponse))
                    {
                        var root = document.RootElement;
                        if (root.TryGetProperty("errors", out var errorsElement))
                        {
                            _logger.LogError($"GraphQL errors: {JsonSerializer.Serialize(errorsElement)}");
                            throw new JsonException($"GraphQL errors: {JsonSerializer.Serialize(errorsElement)}");
                        }
                    }

                    throw; // Re-throw if we couldn't find any errors
                }

                var graphqlPage = graphqlData.Data.Page;

                _logger.LogInformation($"GraphQL page title: {graphqlPage.Title}");

                // Create a new PageResponse
                var pageResponse = new PageResponse
                {
                    Entity = new PageEntity
                    {
                        // Map the page data
                        Page = new Page
                        {
                            Title = graphqlPage.Title,
                            FriendlyName = graphqlPage.FriendlyName,
                            CanEdit = graphqlPage.CanEdit,
                            CanLock = graphqlPage.CanLock,
                            CanRead = graphqlPage.CanRead,
                            // Map additional properties from _map
                            Identifier = GetStringValue(graphqlPage._map, "identifier"),
                            Inode = GetStringValue(graphqlPage._map, "inode"),
                            BaseType = GetStringValue(graphqlPage._map, "baseType"),
                            ContentType = GetStringValue(graphqlPage._map, "contentType"),
                            CreationDate = GetLongValue(graphqlPage._map, "creationDate"),
                            ModDate = GetLongValue(graphqlPage._map, "modDate"),
                            PublishDate = GetLongValue(graphqlPage._map, "publishDate"),
                            Host = GetStringValue(graphqlPage._map, "host"),
                            HostName = GetStringValue(graphqlPage._map, "hostName"),
                            Working = GetBoolValue(graphqlPage._map, "working"),
                            Live = GetBoolValue(graphqlPage._map, "live"),
                            Locked = GetBoolValue(graphqlPage._map, "locked"),
                            Archived = GetBoolValue(graphqlPage._map, "archived"),
                            Owner = GetStringValue(graphqlPage._map, "owner"),
                            OwnerName = GetStringValue(graphqlPage._map, "ownerName"),
                            ModUser = GetStringValue(graphqlPage._map, "modUser"),
                            ModUserName = GetStringValue(graphqlPage._map, "modUserName"),
                            LanguageId = GetIntValue(graphqlPage._map, "languageId"),
                            Path = GetStringValue(graphqlPage._map, "path"),
                            Url = GetStringValue(graphqlPage._map, "url"),
                            PageUrl = GetStringValue(graphqlPage._map, "pageUrl"),
                            PageURI = GetStringValue(graphqlPage._map, "pageURI"),
                            Name = GetStringValue(graphqlPage._map, "name"),
                            ShortyId = GetStringValue(graphqlPage._map, "shortyId"),
                            TitleImage = GetStringValue(graphqlPage._map, "titleImage"),
                            HasLiveVersion = GetBoolValue(graphqlPage._map, "hasLiveVersion"),
                            HasTitleImage = GetBoolValue(graphqlPage._map, "hasTitleImage"),
                            Template = GetStringValue(graphqlPage._map, "template"),
                            Extension = GetStringValue(graphqlPage._map, "extension"),
                            Folder = GetStringValue(graphqlPage._map, "folder"),
                            // Store any additional properties
                            AdditionalProperties = graphqlPage._map
                        },

                        // Map the template data
                        Template = new Template
                        {
                            Drawed = graphqlPage.Template?.Drawed,
                            Identifier = graphqlPage.Template?.Identifier,
                            Inode = graphqlPage.Template?.Inode
                        },

                        // Map the layout data
                        Layout = graphqlPage.Layout,

                        // Map the viewAs data
                        ViewAs = graphqlPage.ViewAs,

                        // Map containers
                        Containers = ConvertContainers(graphqlPage.Containers),

                        // Map urlContentMap
                        UrlContentMap = graphqlPage.UrlContentMap
                    },
                    // Initialize other properties
                    Errors = new List<object>(),
                    Messages = new List<object>(),
                    I18nMessagesMap = new Dictionary<string, object>(),
                    Permissions = new List<object>()
                };

                _logger.LogInformation("GraphQL response conversion completed successfully");

                return pageResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting GraphQL response to PageResponse");
                throw;
            }
        }

        /// <summary>
        /// Converts the GraphQL containers array to the expected Dictionary format
        /// </summary>
        private Dictionary<string, ContainerStructure> ConvertContainers(List<GraphqlContainer> containers)
        {
            if (containers == null)
                return new Dictionary<string, ContainerStructure>();

            var result = new Dictionary<string, ContainerStructure>();

            // First, process each container
            foreach (var container in containers)
            {
                // For each container, we need to create entries for each of its containerContentlets
                if (container.ContainerContentlets != null)
                {
                    foreach (var contentlet in container.ContainerContentlets)
                    {
                        if (string.IsNullOrEmpty(contentlet.Uuid))
                            continue;

                        var containerStructure = new ContainerStructure
                        {
                            Container = new ContainerDetails
                            {
                                Identifier = container.Identifier,
                                Path = container.Path,
                                MaxContentlets = container.MaxContentlets
                            },
                            ContainerStructures = container.ContainerStructures?.Select(cs => new ContainerStructureItem
                            {
                                ContentTypeVar = cs.ContentTypeVar
                            }).ToList(),
                            Contentlets = new Dictionary<string, List<Contentlet>>()
                        };

                        // Add the contentlets to the dictionary with the UUID as the key
                        containerStructure.Contentlets[contentlet.Uuid] = contentlet.Contentlets?.Select(c => new Contentlet
                        {
                            Identifier = c.Identifier,
                            ModDate = ParseLong(c.ModDate),
                            PublishDate = ParseLong(c.PublishDate),
                            CreationDate = ParseLong(c.CreationDate),
                            Title = c.Title,
                            BaseType = c.BaseType,
                            Inode = c.Inode,
                            Archived = c.Archived,
                            Url = c.UrlMap,
                            Working = c.Working,
                            Locked = c.Locked,
                            ContentType = c.ContentType,
                            Live = c.Live,
                            // Copy all properties from _map to AdditionalProperties
                            AdditionalProperties = c._map
                        }).ToList() ?? new List<Contentlet>();

                        // Add to result using the uuid as the key
                        result[contentlet.Uuid] = containerStructure;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Helper method to parse a string to long
        /// </summary>
        private long? ParseLong(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (long.TryParse(value, out long result))
                return result;

            return null;
        }

        /// <summary>
        /// Helper method to get a string value from a dictionary
        /// </summary>
        private string? GetStringValue(Dictionary<string, JsonElement>? dict, string key)
        {
            if (dict == null || !dict.TryGetValue(key, out var element))
                return null;

            if (element.ValueKind == JsonValueKind.String)
                return element.GetString();

            return element.ToString();
        }

        /// <summary>
        /// Helper method to get a long value from a dictionary
        /// </summary>
        private long? GetLongValue(Dictionary<string, JsonElement>? dict, string key)
        {
            if (dict == null || !dict.TryGetValue(key, out var element))
                return null;

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out long result))
                return result;

            if (element.ValueKind == JsonValueKind.String && long.TryParse(element.GetString(), out result))
                return result;

            return null;
        }

        /// <summary>
        /// Helper method to get an int value from a dictionary
        /// </summary>
        private int? GetIntValue(Dictionary<string, JsonElement>? dict, string key)
        {
            if (dict == null || !dict.TryGetValue(key, out var element))
                return null;

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out int result))
                return result;

            if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out result))
                return result;

            return null;
        }

        /// <summary>
        /// Helper method to get a bool value from a dictionary
        /// </summary>
        private bool? GetBoolValue(Dictionary<string, JsonElement>? dict, string key)
        {
            if (dict == null || !dict.TryGetValue(key, out var element))
                return null;

            if (element.ValueKind == JsonValueKind.True)
                return true;

            if (element.ValueKind == JsonValueKind.False)
                return false;

            if (element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out bool result))
                return result;

            return null;
        }
    }
}