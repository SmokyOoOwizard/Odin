using Odin.Abstractions.Components.Declaration;

namespace Odin.Abstractions.Serialization;

public struct SerializedField
{
    public string Name { get; set; }
    public ECollectionType CollectionType { get; set; }
    public bool IsIndex { get; set; }
    public EFieldType Type { get; set; }

    public object? Value { get; set; }
}