using System.Collections.Generic;
using System.Text.Json.Serialization;

using MiHoYoDownloader.Entities;

namespace MiHoYoDownloader.Results;

public class GetCategoriesResult(IReadOnlyList<Category> categories) {
    [JsonPropertyName("manifests")]
    public IReadOnlyList<Category> Categories { get; init; } = categories;
}
