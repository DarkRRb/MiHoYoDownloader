using System;

namespace MiHoYoDownloader.Entities;

public enum Region {
    China,
    International,
}

public static class RegionExtension {
    extension(Region region) {
        public string HypApiHost => region switch {
            Region.China => "hyp-api.mihoyo.com",
            Region.International => "sg-hyp-api.hoyoverse.com",
            _ => throw new NotSupportedException($"Unknown region: {region}"),
        };

        public string TakumiApiHost => region switch {
            Region.China => "api-takumi.mihoyo.com",
            Region.International => "sg-public-api-static.hoyoverse.com",
            _ => throw new NotSupportedException($"Unknown region: {region}"),
        };

        public string LauncherId => region switch {
            Region.China => "jGHBHlcOq1",
            Region.International => "VYTpXlbWo8",
            _ => throw new NotSupportedException($"Unknown region: {region}"),
        };
    }
}