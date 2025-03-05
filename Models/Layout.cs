using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class Layout
    {
        [JsonPropertyName("width")]
        public string? Width { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("header")]
        public bool? Header { get; set; }

        [JsonPropertyName("footer")]
        public bool? Footer { get; set; }

        [JsonPropertyName("body")]
        public LayoutBody? Body { get; set; }

        [JsonPropertyName("sidebar")]
        public object? Sidebar { get; set; }

        [JsonPropertyName("version")]
        public int? Version { get; set; }
    }

    public class LayoutBody
    {
        [JsonPropertyName("rows")]
        public List<Row>? Rows { get; set; }
    }

    public class Row
    {
        [JsonPropertyName("columns")]
        public List<Column>? Columns { get; set; }

        [JsonPropertyName("styleClass")]
        public string? StyleClass { get; set; }
    }

    public class Column
    {
        [JsonPropertyName("containers")]
        public List<Container>? Containers { get; set; }

        [JsonPropertyName("widthPercent")]
        public int? WidthPercent { get; set; }

        [JsonPropertyName("leftOffset")]
        public int? LeftOffset { get; set; }

        [JsonPropertyName("styleClass")]
        public string? StyleClass { get; set; }

        [JsonPropertyName("preview")]
        public bool? Preview { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("left")]
        public int? Left { get; set; }
    }

    public class Container
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("historyUUIDs")]
        public List<string>? HistoryUUIDs { get; set; }
    }
}
