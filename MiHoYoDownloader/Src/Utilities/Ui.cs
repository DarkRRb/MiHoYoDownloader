using System;
using System.Collections.Generic;
using System.Text;

namespace MiHoYoDownloader.Utilities;

public static class Ui {
    public static T Select<T>(string tip, IReadOnlyList<T> values) {
        StringBuilder builder = new();
        builder.Append(tip).AppendLine();
        for (int i = 0; i < values.Count; i++) {
            builder.Append(i + 1).Append(". ").Append(values[i]).AppendLine();
        }
        builder.Append("Select an option number: ");

        Console.Write(builder);

        string? input = Console.ReadLine() ?? throw new Exception("Input cannot be empty");
        return values[int.Parse(input)];
    }
}