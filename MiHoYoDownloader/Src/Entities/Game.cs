using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class Game(string id, GameDisplay display) {
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;

    [JsonPropertyName("display")]
    public GameDisplay Display { get; init; } = display;
}