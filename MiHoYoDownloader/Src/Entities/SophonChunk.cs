using LightProto;

namespace MiHoYoDownloader.Entities;

[ProtoContract]
public partial class SophonChunk {
    [ProtoMember(1)]
    public required string Id { get; init; }

    [ProtoMember(2)]
    public required string Checksum { get; init; }

    [ProtoMember(3)]
    public required ulong Offset { get; init; }

    [ProtoMember(5)]
    public required ulong Size { get; init; }
}