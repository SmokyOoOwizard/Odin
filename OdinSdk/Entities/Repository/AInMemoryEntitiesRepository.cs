using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;

namespace OdinSdk.Entities.Repository;

public abstract class AInMemoryEntitiesRepository
{
    // key - entity id, value - (key - component id, value - component)
    protected readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> OldComponents = new();
    protected readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> Components = new();

    public virtual void Replace<T>(ulong entityId, T? component) where T : IComponent
    {
        lock (Components)
        {
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

    public virtual void Remove<T>(ulong entityId) where T : IComponent
    {
        lock (Components)
        {
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

    public virtual bool Get<T>(ulong entityId, out T? component) where T : IComponent
    {
        component = default;
        lock (Components)
        {
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

    public virtual bool GetOld<T>(ulong entityId, out T? component) where T : IComponent
    {
        component = default;
        lock (Components)
        {
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
    
    public IEnumerable<(ulong, ComponentWrapper[])> GetEntitiesWithComponents()
    {
        ulong[] entities;
        lock (Components)
        {
            entities = Components.Keys.ToArray();
        }

        foreach (var entityId in entities)
        {
            lock (Components)
            {
                if (!Components.TryGetValue(entityId, out var components))
                    continue;

                var componentsArray = components.Select(c => new ComponentWrapper(c.Key, c.Value)).ToArray();

                yield return (entityId, componentsArray);
            }
        }
    }
    
    public IEnumerable<ulong> GetEntities()
    {
        ulong[] entities;
        lock (Components)
        {
            entities = Components.Keys.ToArray();
        }

        foreach (var entityId in entities)
        {
            lock (Components)
            {
                if (!Components.ContainsKey(entityId))
                    continue;

                yield return entityId;
            }
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