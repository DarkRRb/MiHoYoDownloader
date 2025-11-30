using System;

namespace MiHoYoDownloader.Entities;

public enum Language {
    ZhCn,
    EnUs,
    JaJp,
    KoKr,
}

public static class LanguageExtension {
    extension(Language language) {
        public string CategoryField => language switch {
            Language.ZhCn => "zh-cn",
            Language.EnUs => "en-us",
            Language.JaJp => "ja-jp",
            Language.KoKr => "ko-kr",
            _ => throw new NotSupportedException($"Unknown language: {language}"),
        };
    }
}