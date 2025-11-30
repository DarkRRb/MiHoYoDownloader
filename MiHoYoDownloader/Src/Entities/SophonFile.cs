using System.Collections.Generic;

using LightProto;

namespace MiHoYoDownloader.Entities;

[ProtoContract]
public partial class SophonFile {
    [ProtoMember(1)]
    public required string Path { get; init; }

    [ProtoMember(2)]
    public required IReadOnlyList<SophonChunk> Chunks { get; init; }

    [ProtoMember(4)]
    public required ulong Size { get; init; }

    [ProtoMember(5)]
    public required string Checksum { get; init; }
}