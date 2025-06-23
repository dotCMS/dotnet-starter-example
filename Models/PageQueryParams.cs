using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class PageQueryParams
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("site")]
        public string? Site { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("pageMode")]
        public string? PageMode { get; set; }

        [JsonPropertyName("cacheSeconds")]
        public int? CacheSeconds { get; set; }

        [JsonPropertyName("personaId")]
        public string? Persona { get; set; }

        [JsonPropertyName("fireRules")]
        public bool? FireRules { get; set; }

        [JsonPropertyName("renderContent")]
        public bool? RenderContent { get; set; }

        [JsonPropertyName("depth")]
        public int? Depth { get; set; }
    }

}
