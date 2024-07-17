using Odin.Abstractions.Collectors;

namespace OdinSdk.Entities;

internal class EntityCollector : IEntityCollector
{
    public string Name { get; init; }
    public ulong MatcherId { get; init; }

    private readonly HashSet<ulong> _all = new();

    private readonly Queue<ulong> _entityQueue = new();

    public EntityCollector(string name, ulong matcherId)
    {
        Name = name;
        MatcherId = matcherId;
    }

    public void Add(ulong entityId)
    {
        if (!_all.Add(entityId))
            return;

        _entityQueue.Enqueue(entityId);
    }

    public ICollectedEntitiesBatch GetBatch()
    {
        var p = new CollectedEntitiesBatch(_entityQueue.ToArray());
        _entityQueue.Clear();
        _all.Clear();

        return p;
    }
    
    public void Clear()
    {
        _entityQueue.Clear();
        _all.Clear();
    }
}