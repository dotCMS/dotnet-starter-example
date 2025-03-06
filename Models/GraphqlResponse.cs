using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    /// <summary>
    /// Represents the root structure of a GraphQL response
    /// </summary>
    public class GraphqlResponse
    {
        [JsonPropertyName("data")]
        public GraphqlData? Data { get; set; }
    }

    /// <summary>
    /// Represents the data section of a GraphQL response
    /// </summary>
    public class GraphqlData
    {
        [JsonPropertyName("page")]
        public GraphqlPage? Page { get; set; }
    }

    /// <summary>
    /// Represents a page in the GraphQL response
    /// </summary>
    public class GraphqlPage
    {
        [JsonPropertyName("_map")]
        public Dictionary<string, JsonElement>? _map { get; set; }

        [JsonPropertyName("urlContentMap")]
        public Contentlet? UrlContentMap { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("friendlyName")]
        public string? FriendlyName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("tags")]
        public object? Tags { get; set; }

        [JsonPropertyName("canEdit")]
        public bool? CanEdit { get; set; }

        [JsonPropertyName("canLock")]
        public bool? CanLock { get; set; }

        [JsonPropertyName("canRead")]
        public bool? CanRead { get; set; }

        [JsonPropertyName("template")]
        public GraphqlTemplate? Template { get; set; }

        [JsonPropertyName("containers")]
        public List<GraphqlContainer>? Containers { get; set; }

        [JsonPropertyName("layout")]
        public Layout? Layout { get; set; }

        [JsonPropertyName("viewAs")]
        public ViewAs? ViewAs { get; set; }

    }

    /// <summary>
    /// Represents a template in the GraphQL response
    /// </summary>
    public class GraphqlTemplate
    {
        [JsonPropertyName("drawed")]
        public bool? Drawed { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }
        
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }
    }

    /// <summary>
    /// Represents a container in the GraphQL response
    /// </summary>
    public class GraphqlContainer
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("maxContentlets")]
        public int? MaxContentlets { get; set; }

        [JsonPropertyName("containerStructures")]
        public List<GraphqlContainerStructure>? ContainerStructures { get; set; }

        [JsonPropertyName("containerContentlets")]
        public List<GraphqlContainerContentlet>? ContainerContentlets { get; set; }
        
        [JsonPropertyName("container")]
        public ContainerDetails? Container { get; set; }
    }

    /// <summary>
    /// Represents a container structure in the GraphQL response
    /// </summary>
    public class GraphqlContainerStructure
    {
        [JsonPropertyName("contentTypeVar")]
        public string? ContentTypeVar { get; set; }
    }

    /// <summary>
    /// Represents a container contentlet in the GraphQL response
    /// </summary>
    public class GraphqlContainerContentlet
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("contentlets")]
        public List<GraphqlContentlet>? Contentlets { get; set; }
    }

    /// <summary>
    /// Represents a contentlet in the GraphQL response
    /// </summary>
    public class GraphqlContentlet
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("modDate")]
        public string? ModDate { get; set; }

        [JsonPropertyName("publishDate")]
        public string? PublishDate { get; set; }

        [JsonPropertyName("creationDate")]
        public string? CreationDate { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("baseType")]
        public string? BaseType { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("_map")]
        public Dictionary<string, JsonElement>? _map { get; set; }

        [JsonPropertyName("urlMap")]
        public string? UrlMap { get; set; }

        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }
    }
}
