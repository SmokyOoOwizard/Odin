using Odin.Abstractions.Entities;

namespace Odin.Entities.Repository.Impl;

public class InMemoryEntitiesChangedComponentsRepository : AInMemoryEntitiesRepository, IEntityComponentsRepository
{
    public InMemoryEntitiesChangedComponentsRepository(ulong contextId) : base(contextId)
    {
    }

    public override void Apply(IEntitiesCollection entities)
    {
        lock (Components)
        {
            foreach (var entity in entities)
            {
                var entityId = entity.Id.Id;
                
                var changes = entity.Components.GetComponents(entity);
                
                if (!Components.TryGetValue(entityId, out var components))
                    Components[entityId] = components = new();

                if (!OldComponents.TryGetValue(entityId, out var oldComponents))
                    OldComponents[entityId] = oldComponents = new();

                foreach (var component in changes)
                {
                    if (components.TryGetValue(component.TypeId, out var oldComponent))
                        oldComponents[component.TypeId] = oldComponent;
                    components[component.TypeId] = component.Component;
                }
            }
        }
    }
}