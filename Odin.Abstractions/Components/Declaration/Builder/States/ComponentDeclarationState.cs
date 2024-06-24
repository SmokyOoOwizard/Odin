namespace Odin.Abstractions.Components.Declaration.Builder.States;

public struct ComponentDeclarationState : IComponentBuilderState
{
    public string Name { get; set; }
    public ulong Id { get; set; }

    public ComponentDeclaration Add<T>(ComponentDeclaration declaration) where T : IComponent
    {
        declaration.Name = Name ?? typeof(T).FullName ?? throw new ArgumentNullException(nameof(Name));
        declaration.Id = Id != 0 ? Id : throw new ArgumentNullException(nameof(Id));
        return declaration;
    }
}