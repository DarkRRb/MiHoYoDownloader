using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class GameDisplay(string name) {
    [JsonPropertyName("name")]
    public string Name { get; init; } = name;
}