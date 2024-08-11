using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.Components.Declaration.Builder.States;

public struct ComponentFieldDeclarationState : IComponentBuilderState
{
    public string Name { get; set; }
    public ECollectionType CollectionType { get; set; }
    public bool IsIndex { get; set; }
    public EFieldType FieldType { get; set; }
    public int Offset { get; set; }

    public ComponentDeclaration Add<T>(ComponentDeclaration declaration) where T : IComponent
    {
        var field = new ComponentFieldDeclaration
        {
            Name = Name ?? throw new ArgumentNullException(nameof(Name)),
            CollectionType = CollectionType,
            IsIndex = IsIndex,
            Type = FieldType,
            Offset = Offset
        };

        if (declaration.Fields == null)
        {
            declaration.Fields = new[] { field };
            return declaration;
        }

        declaration.Fields = declaration.Fields.Concat(new[] { field })
                                        .ToArray();
        return declaration;
    }
}