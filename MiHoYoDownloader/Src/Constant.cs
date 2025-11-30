namespace MiHoYoDownloader;

public static class Constant {
    public const string MainHelp = """
        Usage: MiHoYoDownloader [options] [command] [command-options]

        Options
            --help, -h      Show command line help.

        Commands:
            games, g        Obtain the ID of the supported game.
            download, d     Download the game.

        Run 'MiHoYoDownloader [command] --help' for more information on a command.
        """;

    public const string GameHelp = """
        Usage: MiHoYoDownloader game [options]
        Usage: MiHoYoDownloader g [options]

        Options:
            --region, -r <REGION>           Server region.
                                            [default: china] Allowed values are c[hina], i[nternational].
            --language, -l <LANGUAGE>       Display Language
                                            [default: zh-cn] Allowed values are zh-cn, en-us, ja-jp, ko-kr.
            --help, -h                      Show command line help.
        """;

    public const string DownloadHelp = """
        Usage: MiHoYoDownloader download [options] <GAME_ID> <TARGET>
        Usage: MiHoYoDownloader d [options] <GAME_ID> <TARGET>

        Options:
            --region, -r        Server region.
                                [default: china] Allowed values are c[hina], i[nternational].
            --predownload       Whether to download pre-downloaded content
            --language, -l      Display Language
                                [default: zh-cn] Allowed values are zh-cn, en-us, ja-jp, ko-kr.
            --source            Folder used for verification to reduce download volume
                                [default: <TARGET>]
            --help, -h          Show command line help.

        Argument:
            <GAME_ID>           The ID of the game to download
            <TARGET>            Target folder for downloaded content
        """;
}