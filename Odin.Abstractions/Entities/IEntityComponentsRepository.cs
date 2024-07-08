using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public interface IEntityComponentsRepository : IReadOnlyEntityRepository
{
    void Replace<T>(ulong entityId, T? component) where T : IComponent;
    void Remove<T>(ulong entityId) where T : IComponent;
    
    void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities);

    void Clear();
}