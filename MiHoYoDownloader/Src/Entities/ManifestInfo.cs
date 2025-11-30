using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class ManifestInfo(string id) {
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;
}