using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Entities;

public class Category(ManifestInfo manifest, DownloadInfo manifestDownload, DownloadInfo chunkDownload, string matchingField) {
    [JsonPropertyName("manifest")]
    public ManifestInfo Manifest { get; init; } = manifest;

    [JsonPropertyName("manifest_download")]
    public DownloadInfo ManifestDownload { get; init; } = manifestDownload;

    [JsonPropertyName("chunk_download")]
    public DownloadInfo ChunkDownload { get; init; } = chunkDownload;

    [JsonPropertyName("matching_field")]
    public string MatchingField { get; init; } = matchingField;
}
