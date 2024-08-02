namespace Odin.Abstractions.Components.Declaration;

public struct ComponentDeclaration
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public ComponentFieldDeclaration[] Fields { get; set; }
}