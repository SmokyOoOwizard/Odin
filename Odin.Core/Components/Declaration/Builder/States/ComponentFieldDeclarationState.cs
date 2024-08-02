namespace Odin.Abstractions.Components.Declaration.Builder.States;

public struct ComponentFieldDeclarationState : IComponentBuilderState
{
    public string Name { get; set; }
    public ECollectionType CollectionType { get; set; }
    public bool IsIndex { get; set; }
    public EFieldType FieldType { get; set; }

    public ComponentDeclaration Add<T>(ComponentDeclaration declaration) where T : IComponent
    {
        var field = new ComponentFieldDeclaration
        {
            Name = Name,
            CollectionType = CollectionType,
            IsIndex = IsIndex,
            Type = FieldType
        };

        declaration.Fields = declaration.Fields.Concat(new[] { field })
                                        .ToArray();
        return declaration;
    }
}