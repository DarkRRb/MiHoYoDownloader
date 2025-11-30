using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

using MiHoYoDownloader.Entities;

namespace MiHoYoDownloader.Contexts;

public class FileContext {

    public string DownloadPrefix { get; }

    public string SourcePath { get; }

    public string TargetPath { get; }

    public IReadOnlyList<SophonChunk> Chunks { get; }

    public byte[] Checksum { get; }

    private readonly Lazy<MemoryMappedFile> _mmf;
    public MemoryMappedFile MMF => _mmf.Value;

    private int _finished = 0;

    public FileContext(Category category, SophonFile file, string source, string target) {
        DownloadPrefix = category.ChunkDownload.UrlPrefix;
        SourcePath = Path.Combine(source, file.Path);
        TargetPath = Path.Combine(target, file.Path);
        Chunks = file.Chunks;
        Checksum = Convert.FromHexString(file.Checksum);
        _mmf = new Lazy<MemoryMappedFile>(() => MemoryMappedFile.CreateFromFile(
            TargetPath,
            FileMode.Create,
            null,
            (long)file.Size
        ), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public bool Finish() => Interlocked.Increment(ref _finished) >= Chunks.Count;
}