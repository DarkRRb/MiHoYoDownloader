using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MiHoYoDownloader.Entities;
using MiHoYoDownloader.Utilities;

using Umrab.Options;

namespace MiHoYoDownloader;

public class GamesProgram {
    public static Task GamesAsync(ParseResult result, CancellationToken token = default) {
        if (result.GetOption("help", () => false)) {
            Console.WriteLine(Constant.GameHelp);
            return Task.CompletedTask;
        }

        Region region = result.GetOption("region", () => Region.China);
        Language language = result.GetOption("language", () => Language.ZhCn);

        return GamesAsync(region, language, token);
    }

    public static async Task GamesAsync(Region region, Language language, CancellationToken token) {
        HttpClient client = new();
        MiHoYoApi api = new(client, region, language);

        IReadOnlyList<Game> games = await api.GetGamesAsync(token);

        StringBuilder builder = new();
        foreach (Game game in games) {
            builder.Append(game.Id).Append(" - ").Append(game.Display.Name).AppendLine();
        }
        Console.WriteLine(builder);
    }
}