using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class Page
    {

        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("baseType")]
        public string? BaseType { get; set; }

        [JsonPropertyName("canEdit")]
        public bool? CanEdit { get; set; }

        [JsonPropertyName("canLock")]
        public bool? CanLock { get; set; }

        [JsonPropertyName("canRead")]
        public bool? CanRead { get; set; }

        [JsonPropertyName("canSeeRules")]
        public bool? CanSeeRules { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("creationDate")]
        public long? CreationDate { get; set; }

        [JsonPropertyName("deleted")]
        public bool? Deleted { get; set; }

        [JsonPropertyName("extension")]
        public string? Extension { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("friendlyName")]
        public string? FriendlyName { get; set; }

        [JsonPropertyName("hasLiveVersion")]
        public bool? HasLiveVersion { get; set; }

        [JsonPropertyName("hasTitleImage")]
        public bool? HasTitleImage { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("hostName")]
        public string? HostName { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("isContentlet")]
        public bool? IsContentlet { get; set; }

        [JsonPropertyName("languageId")]
        public int? LanguageId { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }

        [JsonPropertyName("liveInode")]
        public string? LiveInode { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        [JsonPropertyName("modDate")]
        public long? ModDate { get; set; }

        [JsonPropertyName("modUser")]
        public string? ModUser { get; set; }

        [JsonPropertyName("modUserName")]
        public string? ModUserName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("ownerName")]
        public string? OwnerName { get; set; }

        [JsonPropertyName("pageURI")]
        public string? PageURI { get; set; }

        [JsonPropertyName("pageUrl")]
        public string? PageUrl { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("publishDate")]
        public long? PublishDate { get; set; }

        [JsonPropertyName("publishUser")]
        public string? PublishUser { get; set; }

        [JsonPropertyName("publishUserName")]
        public string? PublishUserName { get; set; }

        [JsonPropertyName("shortyId")]
        public string? ShortyId { get; set; }

        [JsonPropertyName("stInode")]
        public string? StInode { get; set; }

        [JsonPropertyName("statusIcons")]
        public string? StatusIcons { get; set; }

        [JsonPropertyName("template")]
        public string? Template { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("titleImage")]
        public string? TitleImage { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("workingInode")]
        public string? WorkingInode { get; set; }

        // Additional properties that can be arbitrary
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
