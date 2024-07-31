using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities.Indexes;

public interface IIndexModule
{
    IEntitiesCollection GetEntities();

    void Add(ComponentWrapper component, EntityId id);
    void Remove(EntityId id);
}

public interface IIndexModule<T> : IIndexModule where T : struct, IComponent
{
}