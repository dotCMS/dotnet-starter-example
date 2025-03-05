using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RazorPagesDotCMS.Models
{
    public class ViewAs
    {
        [JsonPropertyName("visitor")]
        public Visitor? Visitor { get; set; }

        [JsonPropertyName("language")]
        public Language? Language { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("variantId")]
        public string? VariantId { get; set; }
    }

    public class Visitor
    {
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("device")]
        public string? Device { get; set; }

        [JsonPropertyName("isNew")]
        public bool? IsNew { get; set; }

        [JsonPropertyName("userAgent")]
        public UserAgent? UserAgent { get; set; }

        [JsonPropertyName("referer")]
        public string? Referer { get; set; }

        [JsonPropertyName("geo")]
        public Geo? Geo { get; set; }

        [JsonPropertyName("personas")]
        public Dictionary<string, object>? Personas { get; set; }
    }

    public class UserAgent
    {
        [JsonPropertyName("operatingSystem")]
        public string? OperatingSystem { get; set; }

        [JsonPropertyName("browser")]
        public string? Browser { get; set; }

        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("browserVersion")]
        public BrowserVersion? BrowserVersion { get; set; }
    }

    public class BrowserVersion
    {
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("majorVersion")]
        public string? MajorVersion { get; set; }

        [JsonPropertyName("minorVersion")]
        public string? MinorVersion { get; set; }
    }

    public class Geo
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("continent")]
        public string? Continent { get; set; }

        [JsonPropertyName("continentCode")]
        public string? ContinentCode { get; set; }

        [JsonPropertyName("company")]
        public string? Company { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("subdivision")]
        public string? Subdivision { get; set; }

        [JsonPropertyName("subdivisionCode")]
        public string? SubdivisionCode { get; set; }

        [JsonPropertyName("ipAddress")]
        public string? IpAddress { get; set; }
    }

    public class Language
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("language")]
        public string? LanguageName { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("isoCode")]
        public string? IsoCode { get; set; }
    }
}
