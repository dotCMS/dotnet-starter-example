using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class PageResponse
    {
        [JsonPropertyName("entity")]
        public PageEntity? Entity { get; set; }

        [JsonPropertyName("errors")]
        public List<object>? Errors { get; set; }

        [JsonPropertyName("i18nMessagesMap")]
        public Dictionary<string, object>? I18nMessagesMap { get; set; }

        [JsonPropertyName("messages")]
        public List<object>? Messages { get; set; }

        [JsonPropertyName("pagination")]
        public object? Pagination { get; set; }

        [JsonPropertyName("permissions")]
        public List<object>? Permissions { get; set; }
    }

    public class PageEntity
    {
        [JsonPropertyName("canCreateTemplate")]
        public bool? CanCreateTemplate { get; set; }

        [JsonPropertyName("containers")]
        public Dictionary<string, ContainerStructure>? Containers { get; set; }

        [JsonPropertyName("layout")]
        public Layout? Layout { get; set; }

        [JsonPropertyName("numberContents")]
        public int? NumberContents { get; set; }

        [JsonPropertyName("page")]
        public Page? Page { get; set; }

        [JsonPropertyName("site")]
        public Site? Site { get; set; }

        [JsonPropertyName("template")]
        public Template? Template { get; set; }

        [JsonPropertyName("viewAs")]
        public ViewAs? ViewAs { get; set; }

       [JsonPropertyName("urlContentMap")]
        public Contentlet? UrlContentMap { get; set; }
    }
}
