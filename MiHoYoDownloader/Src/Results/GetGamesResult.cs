using System.Collections.Generic;
using System.Text.Json.Serialization;

using MiHoYoDownloader.Entities;

namespace MiHoYoDownloader.Results;

public class GetGamesResult(IReadOnlyList<Game> games) {
    [JsonPropertyName("games")]
    public IReadOnlyList<Game> Games { get; init; } = games;
}