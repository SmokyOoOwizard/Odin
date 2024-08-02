namespace Odin.Abstractions.Entities.Indexes;

public interface IIndexModule
{
    IEntitiesCollection GetEntities();

    void Add(ComponentWrapper component, EntityId id);
    void Remove(EntityId id);
    
    void SetRepositories(IReadOnlyEntityRepository components, IEntityComponentsRepository changes);
    ulong GetComponentTypeId();
}

