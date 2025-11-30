using System;

namespace MiHoYoDownloader.Exceptions;

public class MiHoYoException(long retcode, string error) : Exception {
    public long Retcode { get; } = retcode;
    public string Error { get; } = error;

    public override string Message { get; } = $"({retcode}){error}";
}