using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public interface IReadOnlyEntityRepository
{
    bool Get<T>(ulong entityId, out T? component) where T : IComponent;
    bool GetOld<T>(ulong entityId, out T? component) where T : IComponent;
    IEnumerable<ulong> GetEntities();
    IEnumerable<(ulong, ComponentWrapper[])> GetEntitiesWithComponents();
}
