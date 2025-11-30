using System.Collections.Generic;
using System.Text.Json.Serialization;

using MiHoYoDownloader.Entities;

namespace MiHoYoDownloader.Results;

public class GetBranchesResult(IReadOnlyList<Branches> gameBranches) {
    [JsonPropertyName("game_branches")]
    public IReadOnlyList<Branches> GameBranches { get; init; } = gameBranches;
}