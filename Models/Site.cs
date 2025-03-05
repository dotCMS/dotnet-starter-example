using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class Site
    {
        [JsonPropertyName("variantId")]
        public string? VariantId { get; set; }

        [JsonPropertyName("hostThumbnail")]
        public string? HostThumbnail { get; set; }

        [JsonPropertyName("structureInode")]
        public string? StructureInode { get; set; }

        [JsonPropertyName("parent")]
        public bool? Parent { get; set; }

        [JsonPropertyName("systemHost")]
        public bool? SystemHost { get; set; }

        [JsonPropertyName("tagStorage")]
        public string? TagStorage { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }

        [JsonPropertyName("aliases")]
        public string? Aliases { get; set; }

        [JsonPropertyName("default")]
        public bool? Default { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("modDate")]
        public long? ModDate { get; set; }

        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("contentTypeId")]
        public string? ContentTypeId { get; set; }

        [JsonPropertyName("languageVariable")]
        public bool? LanguageVariable { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("languageId")]
        public int? LanguageId { get; set; }

        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("modUser")]
        public string? ModUser { get; set; }


        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("titleImage")]
        public string? TitleImage { get; set; }

        [JsonPropertyName("persona")]
        public bool? Persona { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        // Additional properties that can be arbitrary
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
    }
}
