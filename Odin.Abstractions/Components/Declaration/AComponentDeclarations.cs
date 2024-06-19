using Odin.Abstractions.Components.Declaration.Builder;
using Odin.Abstractions.Components.Declaration.Builder.States;

namespace Odin.Abstractions.Components.Declaration;

public abstract class AComponentDeclarations
{
    protected abstract void Configure();

    public bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration)
    {
        throw new System.NotImplementedException();
    }

    protected ComponentBuilder<T, ComponentDeclarationState> Component<T>() where T : IComponent
    {
        return new ComponentBuilder<T, ComponentDeclarationState>(AddComponentDescription);
    }

    protected void AddComponentDescription(ComponentDeclaration componentDeclaration)
    {
    }
}