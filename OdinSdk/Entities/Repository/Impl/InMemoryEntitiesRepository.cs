using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;

namespace OdinSdk.Entities.Repository.Impl;

public class InMemoryEntitiesRepository : AInMemoryEntitiesRepository, IEntityRepository
{
    private readonly ulong _destroyedId = TypeComponentUtils.GetComponentTypeId<DestroyedComponent>();

    // key - matcher id, value - (name, collector)
    private readonly Dictionary<ulong, Dictionary<string, EntityCollector>> _collectors = new();

    // key - collector name, value - matcher id
    private readonly Dictionary<string, ulong> _collectorsToMatchers = new();

    private ulong _lastId;

    public InMemoryEntitiesRepository(ulong contextId) : base(contextId)
    {
    }

    public IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher
    {
        if (_collectorsToMatchers.TryGetValue(name, out var matcherId)
         && _collectors.TryGetValue(matcherId, out var collectors)
         && collectors.TryGetValue(name, out var collector))
        {
            return collector;
        }

        matcherId = MatchersRepository.GetMatcherId<T>();
        if (!_collectors.TryGetValue(matcherId, out collectors))
        {
            collectors = _collectors[matcherId] = new Dictionary<string, EntityCollector>();
        }

        if (collectors.ContainsKey(name))
            throw new InvalidOperationException("Collector already exists.");

        collector = new EntityCollector(name, matcherId, this);

        collectors[name] = collector;

        _collectorsToMatchers[name] = matcherId;

        return collector;
    }


    public void DeleteCollector(string name)
    {
        if (!_collectorsToMatchers.TryGetValue(name, out var matcherId))
            return;

        var collectors = _collectors[matcherId];

        var collector = collectors[name];
        collectors.Remove(name);

        collector.Clear();

        if (collectors.Count == 0)
            _collectors.Remove(matcherId);

        _collectorsToMatchers.Remove(name);
    }

    public IEntitiesCollection GetEntities(ulong[] ids)
    {
        lock (Components)
        {
            var entities = new List<Entity>(ids.Length);

            foreach (var id in ids)
            {
                if (!Components.ContainsKey(id))
                    continue;

                entities.Add(new Entity(new EntityId(id, ContextId), this, this));
            }

            return new InMemoryEntitiesCollection(entities.ToArray());
        }
    }

    public Entity CreateEntity()
    {
        lock (Components)
        {
            _lastId++;
            Components[_lastId] = new();
            return new Entity(new(_lastId, ContextId), this, this);
        }
    }

    public void DestroyEntity(Entity entity)
    {
        lock (Components)
        {
            var entityId = entity.Id.Id;

            Components.Remove(entityId);
            OldComponents.Remove(entityId);
        }
    }

    public override void Apply(IEntitiesCollection entities)
    {
        lock (Components)
        {
            var matchers = _collectors.Select(c => new
            {
                id = c.Key,
                filter = MatchersRepository.GetFilter(c.Key)
            }).ToArray();

            foreach (var entity in entities)
            {
                var id = entity.Id.Id;

                var changes = entity.Components.GetComponents(entity);
                // tmp
                if (changes.Any(c => c.TypeId == _destroyedId))
                {
                    Components.Remove(id);
                    OldComponents.Remove(id);
                    continue;
                }


                if (!Components.TryGetValue(id, out var components))
                    Components[id] = components = new();

                if (!OldComponents.TryGetValue(id, out var oldComponents))
                    OldComponents[id] = oldComponents = new();

                foreach (var component in changes)
                {
                    if (components.TryGetValue(component.TypeId, out var oldComponent))
                        oldComponents[component.TypeId] = oldComponent;
                    components[component.TypeId] = component.Component;
                }

                foreach (var matcher in matchers)
                {
                    if (matcher.filter(id, HasComponent, changes))
                    {
                        var collectors = _collectors[matcher.id];

                        foreach (var (_, collector) in collectors)
                        {
                            collector.Add(id);
                        }
                    }
                }
            }
        }
    }

    private bool HasComponent(ulong entityId, ulong componentId)
    {
        if (!Components.TryGetValue(entityId, out var components))
            return false;

        return components.ContainsKey(componentId);
    }
}