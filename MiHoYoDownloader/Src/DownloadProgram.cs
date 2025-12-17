using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using MiHoYoDownloader.Contexts;
using MiHoYoDownloader.Entities;
using MiHoYoDownloader.Utilities;

using Umrab.Options;

using ZstdSharp;

namespace MiHoYoDownloader;

public class DownloadProgram {
    public static Task DownloadAsync(ParseResult result, CancellationToken token = default) {
        if (result.GetOption("help", () => false)) {
            Console.WriteLine(Constant.DownloadHelp);
            return Task.CompletedTask;
        }
        
        Region region = result.GetOption("region", () => Region.China);
        string gameId = result.GetArgument<string>(0, () => {
            throw new Exception("The `GAME_ID` argument is required.");
        });
        bool isPredownload = result.GetOption("predownload", () => false);
        Language language = result.GetOption("language", () => Language.ZhCn);
        string target = result.GetArgument<string>(1, () => {
            throw new Exception("The `TARGET` argument is required.");
        });
        string source = result.GetOption("source", () => target);

        return DownloadAsync(region, gameId, isPredownload, language, source, target, token);
    }

    public static async Task DownloadAsync(Region region, string gameId, bool isPredownload, Language language, string source, string target, CancellationToken token) {
        AsyncConsole console = new();

        await console.WriteAsync("Initialize...\n");
        HttpClient client = new();
        MiHoYoApi api = new(client, region, language);

        await console.WriteAsync("Fetch download information...\n");
        Branch branch = await api.GetBranchAsync(gameId, isPredownload, token);
        IReadOnlyList<Category> categories = await api.GetCategoriesAsync(
            branch.PackageId,
            branch.Name,
            branch.Password,
            token
        );

        ParallelOptions options = new() {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
            CancellationToken = token,
        };

        ConcurrentBag<FileContext> files = [];
        await Parallel.ForEachAsync(categories, options, async (category, token) => {
            IReadOnlyList<SophonFile> files_ = await api.GetFilesAsync(
                category.ManifestDownload.UrlPrefix,
                category.Manifest.Id,
                token
            );
            foreach (SophonFile file in files_) files.Add(new FileContext(category, file, source, target));
        });

        ulong fCount = 0;

        ulong fFinished = 0;
        ConcurrentBag<ChunkContext> chunks = [];
        await Parallel.ForEachAsync(files, options, async (file, token) => {
            try {
                if (await VerifyAsync(file.SourcePath, file.Checksum, token)) return;
            } finally {
                ulong fFinished_ = Interlocked.Increment(ref fFinished);
                await console.WriteAsync("\r[{0}/{1}] Verifying file...", fFinished_, files.Count);
            }

            Interlocked.Increment(ref fCount);
            foreach (SophonChunk chunk in file.Chunks) chunks.Add(new ChunkContext(file, chunk));

            if (File.Exists(file.TargetPath)) File.Delete(file.TargetPath);
        });
        await console.WriteLineAsync();

        ulong cFinished = 0;
        await Parallel.ForEachAsync(chunks, options, async (chunk, token) => {
            try {
                if (await VerifyAsync(chunk.Path, chunk.Checksum, token)) return;

                string? directory = Path.GetDirectoryName(chunk.Path);
                if (directory != null) Directory.CreateDirectory(directory);

                using HttpRequestMessage request = new();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(chunk.DownloadUrl);

                using HttpResponseMessage response = await client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    token
                );
                response.EnsureSuccessStatusCode();

                using Stream origin = await response.Content.ReadAsStreamAsync(token);
                using Stream input = new DecompressionStream(origin);
                using Stream output = File.Open(chunk.Path, FileMode.Create, FileAccess.Write);
                await input.CopyToAsync(output, token);
            } finally {
                ulong cCount_ = Interlocked.Increment(ref cFinished);
                await console.WriteAsync("\r[{0}/{1}] Processing chunk...", cCount_, chunks.Count);
            }
        });
        await console.WriteLineAsync();

        fFinished = 0;
        cFinished = 0;
        await Parallel.ForEachAsync(chunks, options, async (chunk, token) => {
            try {
                {
                    using Stream input = File.OpenRead(chunk.Path);
                    using Stream output = chunk.File.MMF.CreateViewStream(chunk.Offset, chunk.Size);
                    await input.CopyToAsync(output, token);
                }

                File.Delete(chunk.Path);

                if (chunk.File.Finish()) {
                    chunk.File.MMF.Dispose();
                    Interlocked.Increment(ref fFinished);
                }
            } finally {
                ulong fFinished_ = Volatile.Read(ref fFinished);
                ulong cCount_ = Interlocked.Increment(ref cFinished);
                await console.WriteAsync(
                    "\r[file {0}/{1}] [chunk {2}/{3}] Merging...",
                    fFinished_, fCount,
                    cCount_, chunks.Count
                );
            }
        });
        await console.WriteLineAsync();

        await console.WriteAsync("Completed");
    }

    private static async Task<bool> VerifyAsync(string path, byte[] checksum, CancellationToken token) {
        if (!File.Exists(path)) return false;

        using Stream stream = File.OpenRead(path);
        return (await MD5.HashDataAsync(stream, token)).SequenceEqual(checksum);
    }
}