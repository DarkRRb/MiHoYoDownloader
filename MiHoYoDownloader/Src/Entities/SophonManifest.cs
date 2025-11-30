using System.Collections.Generic;

using LightProto;

namespace MiHoYoDownloader.Entities;

[ProtoContract]
public partial class SophonManifest {
    [ProtoMember(1)]
    public required IReadOnlyList<SophonFile> Files { get; init; }
}