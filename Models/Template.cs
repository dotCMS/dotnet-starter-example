using System;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class Template
    {

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("friendlyName")]
        public string? FriendlyName { get; set; }

        [JsonPropertyName("modDate")]
        public long? ModDate { get; set; }

        [JsonPropertyName("modUser")]
        public string? ModUser { get; set; }

        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("showOnMenu")]
        public bool? ShowOnMenu { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("drawed")]
        public bool? Drawed { get; set; }

        [JsonPropertyName("drawedBody")]
        public string? DrawedBody { get; set; }

        [JsonPropertyName("countAddContainer")]
        public int? CountAddContainer { get; set; }

        [JsonPropertyName("countContainers")]
        public int? CountContainers { get; set; }

        [JsonPropertyName("theme")]
        public string? Theme { get; set; }

        [JsonPropertyName("header")]
        public string? Header { get; set; }

        [JsonPropertyName("footer")]
        public string? Footer { get; set; }

        [JsonPropertyName("template")]
        public bool? IsTemplate { get; set; }


        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("versionType")]
        public string? VersionType { get; set; }

        [JsonPropertyName("versionId")]
        public string? VersionId { get; set; }

        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("deleted")]
        public bool? Deleted { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("idate")]
        public long? Idate { get; set; }

        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }

        [JsonPropertyName("new")]
        public bool? New { get; set; }

        [JsonPropertyName("canEdit")]
        public bool? CanEdit { get; set; }
    }
}
