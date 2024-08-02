using Odin.Core.Components;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Repositories.Entities;

namespace Odin.Core.Indexes;

public interface IIndexModule
{
    IEntitiesCollection GetEntities();

    void Add(ComponentWrapper component, EntityId id);
    void Remove(EntityId id);
    
    void SetRepositories(IReadOnlyEntityRepository components, IEntityComponentsRepository changes);
    ulong GetComponentTypeId();
}

