using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public interface IEntityComponentsRepository : IReadOnlyEntityRepository
{
    void Replace<T>(Entity entity, T? component) where T : IComponent;
    void Remove<T>(Entity entity) where T : IComponent;
    
    void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities);

    void Clear();
}