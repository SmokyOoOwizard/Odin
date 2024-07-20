using Odin.Abstractions.Components;

namespace Odin.Abstractions.Entities;

public interface IReadOnlyEntityRepository
{
    bool Get<T>(Entity entity, out T? component) where T : IComponent;
    bool GetOld<T>(Entity entity, out T? component) where T : IComponent;
    
    ComponentWrapper[] GetComponents(Entity entity);
    
    IEntitiesCollection GetEntities();
}
