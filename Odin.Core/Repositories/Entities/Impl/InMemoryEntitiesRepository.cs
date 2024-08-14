using Odin.Core.Abstractions.Matchers;
using Odin.Core.Collectors;
using Odin.Core.Collectors.Impl;
using Odin.Core.Components;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Entities.Collections.Impl;
using Odin.Core.Indexes;
using Odin.Core.Repositories.Matchers.Impl;
using Odin.Utils;

namespace Odin.Core.Repositories.Entities.Impl;

public class InMemoryEntitiesRepository : AInMemoryEntitiesRepository, IEntityRepository
{
    private readonly ulong _destroyedId = TypeComponentUtils.GetComponentTypeId<DestroyedComponent>();

    // key - collector name, value - matcher id
    private readonly Dictionary<string, ulong> _collectorsToMatchers = new();

    // key - component id, value - index
    private readonly Dictionary<ulong, IInMemoryIndexModule> _indexes = new();

    private ulong _lastId;

    public InMemoryEntitiesRepository(ulong contextId) : base(contextId)
    {
        var type = typeof(IInMemoryIndexModule);
        var existsType = AppDomain.CurrentDomain
                                  .GetAssemblies()
                                  .SelectMany(s => s.GetTypes())
                                  .Where(p => type.IsAssignableFrom(p) && p.IsClass);

        var indexes = existsType.Select(c => (IInMemoryIndexModule)Activator.CreateInstance(c)!).ToArray();
        foreach (var index in indexes)
        {
            _indexes[index.GetComponentTypeId()] = index;

            // todo change changes arg!
            index.SetRepositories(this, this);
        }
    }

    public IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher
    {
        if (!_collectorsToMatchers.TryGetValue(name, out var matcherId))
        {
            matcherId = MatchersRepository.GetMatcherId<T>();

            _collectorsToMatchers[name] = matcherId;
            
            InMemoryEntityCollectors.AddCollector(ContextId, name, matcherId);
        }

        return new InMemoryEntityCollector(ContextId, name, matcherId, this);
    }


    public void DeleteCollector(string name)
    {
        if (!_collectorsToMatchers.TryGetValue(name, out _))
            return;

        InMemoryEntityCollectors.DeleteCollector(ContextId, name);

        _collectorsToMatchers.Remove(name);
    }

    public IIndexModule GetIndex(ulong componentId)
    {
        return _indexes[componentId];
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
            var matchers = _collectorsToMatchers.Select(c => new
            {
                id = c.Value,
                filter = MatchersRepository.GetFilter(c.Value)
            }).ToArray();

            foreach (var entity in entities)
            {
                var id = entity.Id.Id;

                var changes = entity.Changes.GetComponents(entity);
                // tmp
                if (changes.Any(c => c.TypeId == _destroyedId))
                {
                    Components.Remove(id);
                    OldComponents.Remove(id);

                    foreach (var (_, index) in _indexes)
                    {
                        index.Remove(entity.Id);
                    }

                    continue;
                }

                var ownEntity = new Entity(entity.Id, this, entity.Changes);
                foreach (var matcher in matchers)
                {
                    if (matcher.filter(ownEntity))
                        InMemoryEntityCollectors.AddEntity(matcher.id, entity.Id);
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

                    if (_indexes.TryGetValue(component.TypeId, out var indexModule))
                        indexModule.Add(component, entity.Id);
                }
            }
        }
    }
}