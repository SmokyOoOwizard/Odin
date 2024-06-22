namespace Odin.Abstractions.Serialization;

public struct SerializedComponent
{
    public ulong Id { get; set; }
    public SerializedField[] Fields { get; set; }
}