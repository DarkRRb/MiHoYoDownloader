using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class Branches(Branch main, Branch predownload) {
    [JsonPropertyName("main")]
    public Branch Main { get; init; } = main;

    [JsonPropertyName("predownload")]
    public Branch? Predownload { get; init; } = predownload;
}
