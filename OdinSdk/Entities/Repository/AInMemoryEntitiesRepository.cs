using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;
using OdinSdk.Entities.Repository.Impl;

namespace OdinSdk.Entities.Repository;

public abstract class AInMemoryEntitiesRepository : IReadOnlyEntityRepository
{
    protected readonly ulong ContextId;

    // key - entity id, value - (key - component id, value - component)
    protected readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> OldComponents = new();
    protected readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> Components = new();

    protected AInMemoryEntitiesRepository(ulong contextId)
    {
        ContextId = contextId;
    }

    public virtual void Replace<T>(Entity entity, T? component) where T : IComponent
    {
        lock (Components)
        {
            var entityId = entity.Id.Id;

            var componentId = TypeComponentUtils.GetComponentTypeId<T>();
            if (!Components.TryGetValue(entityId, out var components))
                Components[entityId] = components = new();
            else
            {
                if (components.TryGetValue(componentId, out var oldComponent))
                {
                    if (!OldComponents.TryGetValue(entityId, out var oldComponents))
                        OldComponents[entityId] = oldComponents = new();
                    oldComponents[componentId] = oldComponent;
                }
            }

            components[componentId] = component;
        }
    }

    public virtual void Remove<T>(Entity entity) where T : IComponent
    {
        lock (Components)
        {
            var entityId = entity.Id.Id;

            var componentId = TypeComponentUtils.GetComponentTypeId<T>();

            if (!Components.TryGetValue(entityId, out var components))
                Components[entityId] = components = new();
            else
            {
                if (components.TryGetValue(componentId, out var oldComponent))
                {
                    if (!OldComponents.TryGetValue(entityId, out var oldComponents))
                        OldComponents[entityId] = oldComponents = new();
                    oldComponents[componentId] = oldComponent;
                }
            }

            components[componentId] = null;
        }
    }

    public virtual bool Get<T>(Entity entity, out T? component) where T : IComponent
    {
        component = default;
        lock (Components)
        {
            var entityId = entity.Id.Id;

            if (!Components.TryGetValue(entityId, out var components))
                return false;

            var componentId = TypeComponentUtils.GetComponentTypeId<T>();
            if (!components.TryGetValue(componentId, out var rawComponent))
                return false;

            if (rawComponent == null)
                return false;

            component = (T?)rawComponent;

            return true;
        }
    }

    public virtual bool GetOld<T>(Entity entity, out T? component) where T : IComponent
    {
        component = default;
        lock (Components)
        {
            var entityId = entity.Id.Id;

            if (!OldComponents.TryGetValue(entityId, out var components))
                return false;

            var componentId = TypeComponentUtils.GetComponentTypeId<T>();
            if (!components.TryGetValue(componentId, out var rawComponent))
                return false;

            if (rawComponent == null)
                component = default;
            else
                component = (T?)rawComponent;

            return true;
        }
    }

    public IEntitiesCollection GetEntities()
    {
        lock (Components)
        {
            var entities = Components.Keys.ToArray();
            return new InMemoryEntitiesCollection(entities, ContextId, this);
        }
    }

    public ComponentWrapper[] GetComponents(Entity entity)
    {
        lock (Components)
        {
            var entityId = entity.Id.Id;

            if (!Components.TryGetValue(entityId, out var components))
                return Array.Empty<ComponentWrapper>();

            return components
                  .Select(c => new ComponentWrapper
                   {
                       TypeId = c.Key,
                       Component = c.Value
                   })
                  .ToArray();
        }
    }

    public virtual void Clear()
    {
        lock (Components)
        {
            Components.Clear();
            OldComponents.Clear();
        }
    }
}