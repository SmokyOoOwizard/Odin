using Odin.Abstractions.Entities;

namespace OdinSdk.Entities.Repository.Impl;

public class InMemoryEntitiesChangedComponentsRepository : AInMemoryEntitiesRepository, IEntityComponentsRepository
{
    public void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities)
    {
        lock (Components)
        {
            foreach (var entity in entities)
            {
                if (!Components.TryGetValue(entity.Item1, out var components))
                    Components[entity.Item1] = components = new();

                if (!OldComponents.TryGetValue(entity.Item1, out var oldComponents))
                    OldComponents[entity.Item1] = oldComponents = new();

                foreach (var component in entity.Item2)
                {
                    if (components.TryGetValue(component.TypeId, out var oldComponent))
                        oldComponents[component.TypeId] = oldComponent;
                    components[component.TypeId] = component.Component;
                }
            }
        }
    }
}