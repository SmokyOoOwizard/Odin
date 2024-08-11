using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.Components.Declaration;

public struct ComponentDeclaration
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public ulong Size { get; set; }
    public Type Type { get; set; }

    public ComponentFieldDeclaration[] Fields { get; set; }
}