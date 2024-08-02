using Odin.Core.Abstractions.Components;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;

namespace Odin.Core.Repositories.Entities;

public interface IEntityComponentsRepository : IReadOnlyEntityRepository
{
    void Replace<T>(Entity entity, T? component) where T : IComponent;
    void Remove<T>(Entity entity) where T : IComponent;
    
    void Apply(IEntitiesCollection entities);
    
    void Clear();
}