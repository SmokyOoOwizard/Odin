using System.Collections.Immutable;
using Odin.Core.Entities;

namespace Odin.Core.Collectors.Impl;

internal class InMemoryEntityCollectorGenerations
{
    // key - generation, value - EntityId
    private readonly Dictionary<long, Queue<EntityId>> _entities = new();

    // key - generation, value - EntityId
    private readonly Dictionary<long, HashSet<EntityId>> _entitiesSet = new();

    public long Generation { get; private set; }
    public ulong MatcherId { get; }


    public InMemoryEntityCollectorGenerations(ulong matcherId)
    {
        MatcherId = matcherId;
        Generation = 0;
    }

    public long IncreaseCollectorGeneration()
    {
        Generation++;

        return Generation;
    }

    public void ClearCollectorGeneration(long generation)
    {
        _entities.Remove(generation);
        _entitiesSet.Remove(generation);
    }

    public void AddEntity(EntityId entityId)
    {
        if (!_entities.TryGetValue(Generation, out var queue))
            _entities[Generation] = queue = new Queue<EntityId>();

        if (!_entitiesSet.TryGetValue(Generation, out var set))
            _entitiesSet[Generation] = set = new HashSet<EntityId>();

        if (!set.Add(entityId))
            return;

        queue.Enqueue(entityId);
    }

    public ImmutableQueue<EntityId> GetEntities(long generation)
    {
        if (!_entities.TryGetValue(generation, out var queue))
            return ImmutableQueue<EntityId>.Empty;

        return ImmutableQueue.CreateRange(queue);
    }
}