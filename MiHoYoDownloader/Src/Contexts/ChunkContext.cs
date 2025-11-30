using System;

using MiHoYoDownloader.Entities;

namespace MiHoYoDownloader.Contexts;

public class ChunkContext(FileContext file, SophonChunk chunk) {
    public FileContext File { get; } = file;

    public string DownloadUrl { get; } = $"{file.DownloadPrefix}/{chunk.Id}";

    public string Path { get; } = $"{file.TargetPath}_{chunk.Offset}";

    public byte[] Checksum { get; } = Convert.FromHexString(chunk.Checksum);

    public long Offset { get; } = (long)chunk.Offset;

    public long Size { get; } = (long)chunk.Size;
}