using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using LightProto;

using MiHoYoDownloader.Api.Results;
using MiHoYoDownloader.Entities;
using MiHoYoDownloader.Exceptions;
using MiHoYoDownloader.Results;

using ZstdSharp;

namespace MiHoYoDownloader.Utilities;

public class MiHoYoApi(HttpClient? client, Region region, Language language) {
    private readonly HttpClient _client = client ?? new();
    private readonly Region _region = region;
    private readonly Language _language = language;

    private async Task<T> FetchMiHoYoResultAsync<T>(HttpRequestMessage request, CancellationToken token = default) {
        using HttpResponseMessage response = await _client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            token
        );
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync(token);
        MiHoYoResult<T>? result = await JsonUtility.DeserializeAsync<MiHoYoResult<T>>(stream, token);
        if (result == null) throw new MiHoYoException(0, "Result equals null");
        if (!result.IsSuccess) throw new MiHoYoException(result.Retcode, result.Message);

        return result.Data;
    }

    public async Task<IReadOnlyList<Game>> GetGamesAsync(CancellationToken token) {
        using HttpRequestMessage request = new();
        request.Method = HttpMethod.Get;
        request.RequestUri = new BUri {
            Scheme = "https",
            Host = _region.HypApiHost,
            Path = "/hyp/hyp-connect/api/getGames",
            Querys = {
                { "launcher_id", _region.LauncherId },
                { "language", _language.CategoryField },
            },
        };

        GetGamesResult result = await FetchMiHoYoResultAsync<GetGamesResult>(request, token);
        return result.Games;
    }

    public async Task<Branch> GetBranchAsync(string gameId, bool isPredownload, CancellationToken token = default) {
        using HttpRequestMessage request = new();
        request.Method = HttpMethod.Get;
        request.RequestUri = new BUri {
            Scheme = "https",
            Host = _region.HypApiHost,
            Path = "/hyp/hyp-connect/api/getGameBranches",
            Querys = {
                { "launcher_id", _region.LauncherId },
                { "game_ids[]", gameId },
            },
        };

        GetBranchesResult result = await FetchMiHoYoResultAsync<GetBranchesResult>(request, token);
        if (result.GameBranches.Count == 0) throw new MiHoYoException(0, $"Game({gameId}) not found");

        Branches branches = result.GameBranches[0];
        if (!isPredownload) return branches.Main;
        else return branches.Predownload ?? throw new MiHoYoException(0, $"Predownload not found");
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(string packageId, string branch, string password, CancellationToken token = default) {
        using HttpRequestMessage request = new();
        request.Method = HttpMethod.Get;
        request.RequestUri = new BUri {
            Scheme = "https",
            Host = _region.TakumiApiHost,
            Path = "/downloader/sophon_chunk/api/getBuild",
            Querys = {
                { "package_id", packageId },
                { "branch", branch },
                { "password", password },
            },
        };

        GetCategoriesResult result = await FetchMiHoYoResultAsync<GetCategoriesResult>(request, token);

        return [.. result.Categories.Where(IsMatchCategory)];
    }

    public async Task<IReadOnlyList<SophonFile>> GetFilesAsync(string prefix, string id, CancellationToken token = default) {
        using HttpRequestMessage request = new();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri($"{prefix}/{id}");

        using HttpResponseMessage response = await _client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            token
        );
        response.EnsureSuccessStatusCode();

        using Stream origin = await response.Content.ReadAsStreamAsync(token);
        using Stream input = new DecompressionStream(origin);

        SophonManifest manifest = Serializer.Deserialize<SophonManifest>(input);
        return manifest.Files;
    }

    private bool IsMatchCategory(Category category) {
        return category.MatchingField == "game"
            || category.MatchingField.All(char.IsDigit)
            || category.MatchingField == _language.CategoryField;
    }
}