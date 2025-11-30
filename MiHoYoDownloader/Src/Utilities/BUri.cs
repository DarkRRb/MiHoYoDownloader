using System;
using System.Collections.Generic;
using System.Text;

namespace MiHoYoDownloader.Utilities;

public class BUri {
    public required string Scheme { get; init; }

    public required string Host { get; init; }

    public string Path { get; init; } = "/";

    public IDictionary<string, string> Querys { get; } = new SortedDictionary<string, string>();

    public static implicit operator string(BUri uri) {
        StringBuilder builder = new();
        builder.Append(uri.Scheme);
        builder.Append("://");
        builder.Append(uri.Host);
        builder.Append(uri.Path);
        if (uri.Querys.Count != 0) {
            bool first = true;
            foreach (KeyValuePair<string, string> query in uri.Querys) {
                builder.Append(first ? '?' : '&');
                builder.Append(query.Key);
                builder.Append('=');
                builder.Append(query.Value);
                
                if (first) first = !first;
            }
        }

        return builder.ToString();
    }

    public static implicit operator Uri(BUri uri) => new(uri);
}