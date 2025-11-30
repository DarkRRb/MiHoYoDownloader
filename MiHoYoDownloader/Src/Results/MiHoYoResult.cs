using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MiHoYoDownloader.Api.Results;

public class MiHoYoResult<T>(long retcode, string message, T? data) {
    [JsonPropertyName("retcode")]
    public long Retcode { get; init; } = retcode;

    [JsonPropertyName("message")]
    public string Message { get; init; } = message;

    [JsonPropertyName("data")]
    public T? Data { get; init; } = data;

    [MemberNotNullWhen(true, nameof(Data))]
    public bool IsSuccess => Retcode == 0;
}