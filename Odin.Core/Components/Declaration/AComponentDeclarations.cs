using Odin.Core.Abstractions.Components;
using Odin.Core.Components.Declaration.Builder;
using Odin.Core.Components.Declaration.Builder.States;

namespace Odin.Core.Components.Declaration;

public abstract class AComponentDeclarations<T> where T : AComponentDeclarations<T>, new()
{
    public static T Instance { get; } = new();

    public AComponentDeclarations()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Configure();
    }

    protected abstract void Configure();

    public ulong? GetComponentTypeId<TComponent>() where TComponent : IComponent
    {
        throw new System.NotImplementedException();
    }

    public bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration)
    {
        throw new System.NotImplementedException();
    }

    protected ComponentBuilder<TComponent, ComponentDeclarationState> Component<TComponent>() where TComponent : IComponent
    {
        return new ComponentBuilder<TComponent, ComponentDeclarationState>(AddComponentDescription);
    }

    protected void AddComponentDescription(ComponentDeclaration componentDeclaration)
    {
    }
}