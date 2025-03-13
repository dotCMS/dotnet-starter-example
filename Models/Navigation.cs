using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    /// <summary>
    /// Represents the response from the navigation API
    /// </summary>
    public class NavigationResponse
    {
        [JsonPropertyName("entity")]
        public NavigationEntity Entity { get; set; } = new NavigationEntity();

        [JsonPropertyName("errors")]
        public List<object> Errors { get; set; } = new List<object>();

        [JsonPropertyName("i18nMessagesMap")]
        public Dictionary<string, string> I18nMessagesMap { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("messages")]
        public List<object> Messages { get; set; } = new List<object>();

        [JsonPropertyName("pagination")]
        public object? Pagination { get; set; }

        [JsonPropertyName("permissions")]
        public List<object> Permissions { get; set; } = new List<object>();
    }

    /// <summary>
    /// Represents the navigation entity
    /// </summary>
    public class NavigationEntity
    {
        [JsonPropertyName("children")]
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("hash")]
        public long Hash { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("languageId")]
        public int LanguageId { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("target")]
        public string? Target { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// Represents a navigation item
    /// </summary>
    public class NavigationItem
    {
        [JsonPropertyName("children")]
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("hash")]
        public long Hash { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("languageId")]
        public int LanguageId { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("target")]
        public string? Target { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
