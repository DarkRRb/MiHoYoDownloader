using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class Branch(string packageId, string name, string password) {
    [JsonPropertyName("package_id")]
    public string PackageId { get; init; } = packageId;

    [JsonPropertyName("branch")]
    public string Name { get; init; } = name;

    [JsonPropertyName("password")]
    public string Password { get; init; } = password;
}