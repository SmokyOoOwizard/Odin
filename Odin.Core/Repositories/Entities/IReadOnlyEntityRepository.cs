using Odin.Core.Abstractions.Components;
using Odin.Core.Components;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;

namespace Odin.Core.Repositories.Entities;

public interface IReadOnlyEntityRepository
{
    bool Has(Entity entity, ulong componentId);
    bool WasRemoved(Entity entity, ulong componentId);
    
    bool Get<T>(Entity entity, out T? component) where T : IComponent;
    bool GetOld<T>(Entity entity, out T? component) where T : IComponent;
    
    ComponentWrapper[] GetComponents(Entity entity);

    IEntitiesCollection GetEntities(IEntityComponentsRepository? changes = default);
}
