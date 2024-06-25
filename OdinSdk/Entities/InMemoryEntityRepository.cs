using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;

namespace OdinSdk.Entities;

public class InMemoryEntityRepository : IEntityRepository
{
    private ulong _lastId;
    
    private readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> _oldComponents = new();
    private readonly Dictionary<ulong, Dictionary<ulong, IComponent?>> _components = new();
    
    private readonly ulong _destroyedId = TypeComponentUtils.GetComponentTypeId<DestroyedComponent>();

    public void Replace<T>(ulong entityId, T? component) where T : IComponent
    {
        lock (_components)
        {
            var componentId = TypeComponentUtils.GetComponentTypeId<T>();
            if (!_components.TryGetValue(entityId, out var components))
                _components[entityId] = components = new();
            else
            {
                if (components.TryGetValue(componentId, out var oldComponent))
                {
                    if (!_oldComponents.TryGetValue(entityId, out var oldComponents))
                        _oldComponents[entityId] = oldComponents = new();
                    oldComponents[componentId] = oldComponent;
                }
            }

            components[componentId] = component;
        }
    }

    public void Remove<T>(ulong entityId) where T : IComponent
    {
        lock (_components)
        {
            var componentId = TypeComponentUtils.GetComponentTypeId<T>();

            if (!_components.TryGetValue(entityId, out var components))
                _components[entityId] = components = new();
            else
            {
                if (components.TryGetValue(componentId, out var oldComponent))
                {
                    if (!_oldComponents.TryGetValue(entityId, out var oldComponents))
                        _oldComponents[entityId] = oldComponents = new();
                    oldComponents[componentId] = oldComponent;
                }
            }

            components[componentId] = null;
        }
    }

    public ulong CreateEntity()
    {
        lock (_components)
        {
            _lastId++;
            _components[_lastId] = new();
            return _lastId;
        }
    }

    public void DestroyEntity(ulong entityId)
    {
        lock (_components)
        {
            _components.Remove(entityId);
            _oldComponents.Remove(entityId);
        }
    }


    public bool Get<T>(ulong entityId, out T? component) where T : IComponent
    {
        component = default;
        lock (_components)
        {
            if (!_components.TryGetValue(entityId, out var components))
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

    public bool GetOld<T>(ulong entityId, out T? component) where T : IComponent
    {
        component = default;
        lock (_components)
        {
            if (!_oldComponents.TryGetValue(entityId, out var components))
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

    public IEnumerable<ulong> GetEntities()
    {
        ulong[] entities;
        lock (_components)
        {
            entities = _components.Keys.ToArray();
        }

        foreach (var entityId in entities)
        {
            lock (_components)
            {
                if (!_components.ContainsKey(entityId))
                    continue;

                yield return entityId;
            }
        }
    }

    public IEnumerable<(ulong, ComponentWrapper[])> GetEntitiesWithComponents()
    {
        ulong[] entities;
        lock (_components)
        {
            entities = _components.Keys.ToArray();
        }

        foreach (var entityId in entities)
        {
            lock (_components)
            {
                if (!_components.TryGetValue(entityId, out var components))
                    continue;

                var componentsArray = components.Select(c => new ComponentWrapper(c.Key, c.Value)).ToArray();

                yield return (entityId, componentsArray);
            }
        }
    }

    public void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities)
    {
        lock (_components)
        {
            foreach (var (id, changes) in entities)
            {
                // tmp
                if (changes.Any(c => c.TypeId == _destroyedId))
                {
                    _components.Remove(id);
                    _oldComponents.Remove(id);
                    continue;
                }


                if (!_components.TryGetValue(id, out var components))
                    _components[id] = components = new();

                if (!_oldComponents.TryGetValue(id, out var oldComponents))
                    _oldComponents[id] = oldComponents = new();

                foreach (var component in changes)
                {
                    if (components.TryGetValue(component.TypeId, out var oldComponent))
                        oldComponents[component.TypeId] = oldComponent;
                    components[component.TypeId] = component.Component;
                }
            }
        }
    }

    public void Apply((ulong, ComponentWrapper[]) entity)
    {
        lock (_components)
        {
            if (entity.Item2.Any(c => c.TypeId == _destroyedId))
            {
                _components.Remove(entity.Item1);
                _oldComponents.Remove(entity.Item1);
                return;
            }

            if (!_components.TryGetValue(entity.Item1, out var components))
                _components[entity.Item1] = components = new();

            if (!_oldComponents.TryGetValue(entity.Item1, out var oldComponents))
                _oldComponents[entity.Item1] = oldComponents = new();

            foreach (var component in entity.Item2)
            {
                if (components.TryGetValue(component.TypeId, out var oldComponent))
                    oldComponents[component.TypeId] = oldComponent;
                components[component.TypeId] = component.Component;
            }
        }
    }

    public void Clear()
    {
        lock (_components)
        {
            _components.Clear();
        }
    }
}