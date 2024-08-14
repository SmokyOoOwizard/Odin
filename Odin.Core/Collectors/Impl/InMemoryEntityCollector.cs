using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Entities.Collections.Impl;
using Odin.Core.Repositories.Entities.Impl;

namespace Odin.Core.Collectors.Impl;

internal class InMemoryEntityCollector : IEntityCollector
{
    private readonly ulong _contextId;
    private readonly InMemoryEntitiesRepository _inMemoryEntitiesRepository;

    public string Name { get; }
    public ulong MatcherId { get; }

    private bool _autoClear = true;
    private bool _wasUse;
    private readonly long _generation;

    public InMemoryEntityCollector(
        ulong contextId,
        string name,
        ulong matcherId,
        InMemoryEntitiesRepository inMemoryEntitiesRepository
    )
    {
        _contextId = contextId;
        _inMemoryEntitiesRepository = inMemoryEntitiesRepository;
        Name = name;
        MatcherId = matcherId;

        _generation = InMemoryEntityCollectors.GetCollectorGeneration(_contextId, Name);
    }

    public void SetAutoClear(bool enable)
    {
        if (_wasUse)
            throw new Exception(); // TODO

        _autoClear = enable;
    }

    public IEntitiesCollection GetEntities()
    {
        _wasUse = true;

        if (_autoClear)
            InMemoryEntityCollectors.IncreaseCollectorGeneration(_contextId, Name);

        var ids = InMemoryEntityCollectors.GetCollector(_contextId, Name, _generation)
                                  .Select(
                                       id => new Entity(id, _inMemoryEntitiesRepository, _inMemoryEntitiesRepository));

        return new InMemoryEntitiesCollection(ids);
    }

    public void Dispose()
    {
        if (!_autoClear)
            return;

        InMemoryEntityCollectors.ClearCollectorGeneration(_contextId, Name, _generation);
    }
}