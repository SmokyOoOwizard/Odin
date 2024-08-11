using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components.Declaration;

public interface IComponentDeclarations
{
    IEnumerable<ComponentDeclaration> GetComponentDeclarations();
    ulong? GetComponentTypeId<TComponent>() where TComponent : IComponent;
    bool TryGet(ulong componentTypeId, out ComponentDeclaration componentDeclaration);
}