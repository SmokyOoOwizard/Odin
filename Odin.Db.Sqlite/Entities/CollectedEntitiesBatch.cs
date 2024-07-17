using Odin.Abstractions.Collectors;

namespace Odin.Db.Sqlite.Entities;

internal class CollectedEntitiesBatch : ICollectedEntitiesBatch
{
    private readonly ulong[] _entities;

    public CollectedEntitiesBatch(ulong[] entities)
    {
        _entities = entities;
    }

    public IEnumerable<ulong> GetEntities()
    {
        return _entities;
    }
}