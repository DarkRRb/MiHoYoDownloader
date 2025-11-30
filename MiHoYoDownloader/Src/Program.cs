using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MiHoYoDownloader.Contexts;
using MiHoYoDownloader.Entities;
using MiHoYoDownloader.Utilities;

using Umrab.Options;

using ZstdSharp;

namespace MiHoYoDownloader;

public class Program {
    public static async Task Main(string[] args) {
        CancellationTokenSource cts = new();
        Console.CancelKeyPress += Cancel;

        ParseResult result = new Command("MiHoYoDownloader")
            .Option<bool>("help", ['h'], isFlag: true, (_, _) => true)
            .SubCommand("games", ['g'], c => c
                .Option<bool>("help", ['h'], isFlag: true, (_, _) => true)
                .Option<Region>("region", ['r'], (v, _) => v switch {
                    "china" or "c" => Region.China,
                    "international" or "i" => Region.International,
                    _ => throw new NotSupportedException($"Unknown region: {v}"),
                })
                .Option<Language>("language", ['l'], (v, _) => v switch {
                    "zh-cn" => Language.ZhCn,
                    "en-us" => Language.EnUs,
                    "ja-jp" => Language.JaJp,
                    "ko-kr" => Language.KoKr,
                    _ => throw new NotSupportedException($"Unknown language: {v}"),
                }))
            .SubCommand("download", ['d'], c => c
                .Option<bool>("help", ['h'], isFlag: true, (_, _) => true)
                .Option<Region>("region", ['r'], (v, _) => v switch {
                    "china" or "c" => Region.China,
                    "international" or "i" => Region.International,
                    _ => throw new NotSupportedException($"Unknown region: {v}"),
                })
                .Option<bool>("predownload", ['p'], isFlag: true, (_, _) => true)
                .Option<Language>("language", ['l'], (v, _) => v switch {
                    "zh-cn" => Language.ZhCn,
                    "en-us" => Language.EnUs,
                    "ja-jp" => Language.JaJp,
                    "ko-kr" => Language.KoKr,
                    _ => throw new NotSupportedException($"Unknown language: {v}"),
                })
                .Option<string>("source", ['s'], (v, _) => v.ToString())
                .Argument(v => v.ToString()) /* Game id */
                .Argument(v => v.ToString()) /* Target */)
            .Parse(args);

        if (result.GetOption("help", () => false)) {
            Console.WriteLine(Constant.MainHelp);
            return;
        }

        await (result.SubCommand switch {
            { Command.Name: "games" } r => GamesProgram.GamesAsync(r, cts.Token),
            { Command.Name: "download" } r => DownloadProgram.DownloadAsync(r, cts.Token),
            _ => throw new NotImplementedException(),
        });

        void Cancel(object? source, ConsoleCancelEventArgs @event) {
            @event.Cancel = true;
            cts.Cancel();
            Console.CancelKeyPress -= Cancel;
        }
    }
}