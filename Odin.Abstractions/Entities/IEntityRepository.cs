using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public interface IEntityRepository : IReadOnlyEntityRepository
{
    void Replace<T>(ulong entityId, T? component) where T : IComponent;
    void Remove<T>(ulong entityId) where T : IComponent;

    ulong CreateEntity();
    void DestroyEntity(ulong entityId);

    void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities);
    void Apply((ulong, ComponentWrapper[]) entity);

    void Clear();
}