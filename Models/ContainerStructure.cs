using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class ContainerStructure
    {
        [JsonPropertyName("containerStructures")]
        public List<ContainerStructureItem>? ContainerStructures { get; set; }

        [JsonPropertyName("contentlets")]
        public Dictionary<string, List<Contentlet>>? Contentlets { get; set; }

        [JsonPropertyName("container")]
        public ContainerDetails? Container { get; set; }
    }

    public class ContainerStructureItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("structureId")]
        public string? StructureId { get; set; }

        [JsonPropertyName("containerInode")]
        public string? ContainerInode { get; set; }

        [JsonPropertyName("containerId")]
        public string? ContainerId { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("contentTypeVar")]
        public string? ContentTypeVar { get; set; }
    }

    public class ContainerDetails
    {
        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }

        [JsonPropertyName("categoryId")]
        public string? CategoryId { get; set; }

        [JsonPropertyName("deleted")]
        public bool? Deleted { get; set; }

        [JsonPropertyName("friendlyName")]
        public string? FriendlyName { get; set; }

        [JsonPropertyName("hostId")]
        public string? HostId { get; set; }

        [JsonPropertyName("hostName")]
        public string? HostName { get; set; }

        [JsonPropertyName("iDate")]
        public long? IDate { get; set; }


        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("inode")]
        public string? Inode { get; set; }

        [JsonPropertyName("languageId")]
        public int? LanguageId { get; set; }

        [JsonPropertyName("live")]
        public bool? Live { get; set; }

        [JsonPropertyName("locked")]
        public bool? Locked { get; set; }

        [JsonPropertyName("maxContentlets")]
        public int? MaxContentlets { get; set; }

        [JsonPropertyName("modDate")]
        public long? ModDate { get; set; }

        [JsonPropertyName("modUser")]
        public string? ModUser { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("new")]
        public bool? New { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("permissionId")]
        public string? PermissionId { get; set; }

        [JsonPropertyName("permissionType")]
        public string? PermissionType { get; set; }

        [JsonPropertyName("postLoop")]
        public string? PostLoop { get; set; }

        [JsonPropertyName("preLoop")]
        public string? PreLoop { get; set; }

        [JsonPropertyName("showOnMenu")]
        public bool? ShowOnMenu { get; set; }

        [JsonPropertyName("sortOrder")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("useDiv")]
        public bool? UseDiv { get; set; }

        [JsonPropertyName("versionId")]
        public string? VersionId { get; set; }

        [JsonPropertyName("versionType")]
        public string? VersionType { get; set; }

        [JsonPropertyName("working")]
        public bool? Working { get; set; }

        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }
    }
}
