using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class Contentlet
    {
        // Common properties found in the JSON example
        [JsonPropertyName("hostName")]
        public string? HostName { get; set; }

        [JsonPropertyName("modDate")]
        public long? ModDate { get; set; }



        [JsonPropertyName("publishDate")]
        public long? PublishDate { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("baseType")]
        public string? BaseType { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("ownerName")]
        public string? OwnerName { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("languageId")]
        public int? LanguageId { get; set; }

        [JsonPropertyName("creationDate")]
        public long? CreationDate { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("modUserName")]
        public string? ModUserName { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("modUser")]
        public string? ModUser { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        // Additional properties that can be arbitrary
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
