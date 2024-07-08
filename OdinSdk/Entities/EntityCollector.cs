using Odin.Abstractions.Collectors;

namespace OdinSdk.Entities;

internal class EntityCollector : IEntityCollector
{
    public string Name { get; init; }
    public ulong MatcherId { get; init; }

    private HashSet<ulong> _all = new();

    private Queue<ulong> _ss = new();

    public EntityCollector(string name, ulong matcherId)
    {
        Name = name;
        MatcherId = matcherId;
    }

    public void Add(ulong entityId)
    {
        if (!_all.Add(entityId))
            return;

        _ss.Enqueue(entityId);
    }

    public ICollectedEntitiesBatch GetBatch()
    {
        var p = new CollectedEntitiesBatch(_ss.ToArray());
        _ss.Clear();
        _all.Clear();

        return p;
    }
}