namespace Odin.Abstractions.Components.Declaration;

public struct ComponentFieldDeclaration
{
    public string Name { get; set; }
    public CollectionType CollectionType { get; set; }
    public bool IsIndex { get; set; }
    public EFieldType Type { get; set; }
}