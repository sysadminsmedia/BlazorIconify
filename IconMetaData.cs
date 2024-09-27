using System.Text.Json.Serialization;

namespace SysAdminsMedia.BlazorIconify;

public class IconMetaData
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("content")] public required string Content { get; set; }
    [JsonPropertyName("time_fetched")] public DateTime TimeFetched { get; set; }
    
    [JsonPropertyName("color")] public string? Color { get; set; }
}