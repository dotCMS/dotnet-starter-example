
using System.Text.Json;

namespace RazorPagesDotCMS.Models
{


    public class ModelHelper
    {
        private readonly ILogger<ModelHelper> _logger;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Converters = { new StringOrNumberConverter() }
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
                    _logger.LogInformation($"Root element properties: {string.Join(", ", root.EnumerateObject().Select(p => p.Name))}");
                    
                    if (root.TryGetProperty("data", out var dataElement))
                    {
                        _logger.LogInformation("Found 'data' property in response");
                        _logger.LogInformation($"Data element properties: {string.Join(", ", dataElement.EnumerateObject().Select(p => p.Name))}");
                        
                        if (dataElement.TryGetProperty("page", out var pageElement))
                        {
                            _logger.LogInformation("Found 'page' property in data");
                            _logger.LogInformation($"Page element properties: {string.Join(", ", pageElement.EnumerateObject().Select(p => p.Name))}");
                        }
                        else
                        {
                            _logger.LogWarning("No 'page' property found in 'data'");
                            // Log the actual structure
                            _logger.LogInformation($"Data element structure: {JsonSerializer.Serialize(dataElement)}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No 'data' property found in response");
                        // Log the actual structure
                        _logger.LogInformation($"Root element structure: {JsonSerializer.Serialize(root)}");
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
                        _logger.LogWarning("GraphQL response has no 'page' property in 'data', attempting to use raw data");
                        
                        // Try to extract page data directly from the JSON
                        using (var document = JsonDocument.Parse(graphqlResponse))
                        {
                            var root = document.RootElement;
                            if (root.TryGetProperty("data", out var dataElement))
                            {
                                // Check if the data element itself is the page data
                                _logger.LogInformation("Attempting to use data element as page data");
                                
                                // Create a new GraphqlData and GraphqlPage
                                graphqlData.Data = new GraphqlData();
                                var pageData = new GraphqlPage();
                                
                                // Try to populate basic properties
                                if (dataElement.TryGetProperty("page", out var pageElement))
                                {
                                    _logger.LogInformation("Found page element in data, using it directly");
                                    // Use the page element directly
                                    try
                                    {
                                        pageData = JsonSerializer.Deserialize<GraphqlPage>(pageElement.GetRawText(), options);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error deserializing page element");
                                        
                                        // Try to extract individual properties
                                        if (pageElement.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String)
                                        {
                                            pageData.Title = titleElement.GetString();
                                        }
                                        
                                        if (pageElement.TryGetProperty("friendlyName", out var friendlyNameElement) && friendlyNameElement.ValueKind == JsonValueKind.String)
                                        {
                                            pageData.FriendlyName = friendlyNameElement.GetString();
                                        }
                                        
                                        // Try to extract layout data
                                        if (pageElement.TryGetProperty("layout", out var layoutElement))
                                        {
                                            pageData.Layout = JsonSerializer.Deserialize<Layout>(layoutElement.GetRawText(), options);
                                        }
                                        
                                        // Try to extract containers data
                                        if (pageElement.TryGetProperty("containers", out var containersElement))
                                        {
                                            pageData.Containers = JsonSerializer.Deserialize<List<GraphqlContainer>>(containersElement.GetRawText(), options);
                                        }
                                        
                                        // Try to extract template data
                                        if (pageElement.TryGetProperty("template", out var templateElement))
                                        {
                                            pageData.Template = JsonSerializer.Deserialize<GraphqlTemplate>(templateElement.GetRawText(), options);
                                        }
                                        
                                        // Try to extract _map data
                                        if (pageElement.TryGetProperty("_map", out var mapElement))
                                        {
                                            pageData._map = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(mapElement.GetRawText(), options);
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("No page element found in data, trying to use data element properties directly");
                                    
                                    // Try to populate basic properties from data element
                                    if (dataElement.TryGetProperty("title", out var titleElement) && titleElement.ValueKind == JsonValueKind.String)
                                    {
                                        pageData.Title = titleElement.GetString();
                                    }
                                    
                                    if (dataElement.TryGetProperty("friendlyName", out var friendlyNameElement) && friendlyNameElement.ValueKind == JsonValueKind.String)
                                    {
                                        pageData.FriendlyName = friendlyNameElement.GetString();
                                    }
                                    
                                    // Try to extract layout data
                                    if (dataElement.TryGetProperty("layout", out var layoutElement))
                                    {
                                        pageData.Layout = JsonSerializer.Deserialize<Layout>(layoutElement.GetRawText(), options);
                                    }
                                    
                                    // Try to extract containers data
                                    if (dataElement.TryGetProperty("containers", out var containersElement))
                                    {
                                        pageData.Containers = JsonSerializer.Deserialize<List<GraphqlContainer>>(containersElement.GetRawText(), options);
                                    }
                                    
                                    // Try to extract template data
                                    if (dataElement.TryGetProperty("template", out var templateElement))
                                    {
                                        pageData.Template = JsonSerializer.Deserialize<GraphqlTemplate>(templateElement.GetRawText(), options);
                                    }
                                    
                                    // Try to extract _map data
                                    if (dataElement.TryGetProperty("_map", out var mapElement))
                                    {
                                        pageData._map = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(mapElement.GetRawText(), options);
                                    }
                                }
                                
                                // Set the page data
                                graphqlData.Data.Page = pageData;
                            }
                            else
                            {
                                _logger.LogError("GraphQL response has no 'data' property");
                                throw new JsonException("GraphQL response has no 'data' property");
                            }
                        }
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

                // Initialize _map if it's null
                if (graphqlPage._map == null)
                {
                    _logger.LogWarning("GraphQL page _map is null, initializing empty dictionary");
                    graphqlPage._map = new Dictionary<string, JsonElement>();
                }

                _logger.LogInformation($"GraphQL page title: {graphqlPage.Title}");

                // Create a new PageResponse with null checks for all properties
                var pageResponse = new PageResponse
                {
                    Entity = new PageEntity
                    {
                        // Map the page data
                        Page = new Page
                        {
                            Title = graphqlPage.Title ?? "Untitled Page",
                            FriendlyName = graphqlPage.FriendlyName,
                            CanEdit = graphqlPage.CanEdit ?? false,
                            CanLock = graphqlPage.CanLock ?? false,
                            CanRead = graphqlPage.CanRead ?? true,
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
