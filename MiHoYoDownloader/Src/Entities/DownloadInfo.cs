using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class DownloadInfo(string urlPrefix) {
    [JsonPropertyName("url_prefix")]
    public string UrlPrefix { get; init; } = urlPrefix;
}
