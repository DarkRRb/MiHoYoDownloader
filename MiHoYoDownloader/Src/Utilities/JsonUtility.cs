using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using MiHoYoDownloader.Api.Results;
using MiHoYoDownloader.Results;

namespace MiHoYoDownloader.Utilities;

public static partial class JsonUtility {
    [JsonSerializable(typeof(MiHoYoResult<GetGamesResult>))]
    [JsonSerializable(typeof(MiHoYoResult<GetBranchesResult>))]
    [JsonSerializable(typeof(MiHoYoResult<GetCategoriesResult>))]
    private partial class Context : JsonSerializerContext;

    public static ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken token = default) {
        ValueTask<object?> vt = JsonSerializer.DeserializeAsync(stream, typeof(T), Context.Default, token);
        return vt.IsCompleted ? ValueTask.FromResult((T?)vt.Result) : Await(vt);

        static async ValueTask<T?> Await(ValueTask<object?> vt) => (T?)await vt;
    }
}