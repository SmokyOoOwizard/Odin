using Odin.Core.Entities.Collections;
using Odin.Core.Repositories.Entities.Impl;

namespace Odin.Core.Collectors.Impl;

internal class EntityCollector : IEntityCollector
{
    private readonly InMemoryEntitiesRepository _inMemoryEntitiesRepository;

    private readonly HashSet<ulong> _all = new();
    private readonly Queue<ulong> _entityQueue = new();

    public string Name { get; }
    public ulong MatcherId { get; }


    public EntityCollector(
        string name,
        ulong matcherId,
        InMemoryEntitiesRepository inMemoryEntitiesRepository
    )
    {
        _inMemoryEntitiesRepository = inMemoryEntitiesRepository;
        Name = name;
        MatcherId = matcherId;
    }

    public void Add(ulong entityId)
    {
        if (!_all.Add(entityId))
            return;

        _entityQueue.Enqueue(entityId);
    }

    public void SetAutoClear(bool enable)
    {
    }

    public IEntitiesCollection GetEntities()
    {
        var ids = _entityQueue.ToArray();
        _entityQueue.Clear();
        _all.Clear();

        // TODO use other changes repository
        return _inMemoryEntitiesRepository.GetEntities(ids);
    }

    public void Clear()
    {
        _entityQueue.Clear();
        _all.Clear();
    }

    public void Dispose()
    {
    }
}