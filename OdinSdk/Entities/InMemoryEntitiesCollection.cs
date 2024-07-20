using System.Collections;
using Odin.Abstractions.Entities;

namespace OdinSdk.Entities;

public class InMemoryEntitiesCollection : IEntitiesCollection
{
    private readonly ulong[] _entities;
    private readonly ulong _contextId;
    private readonly IReadOnlyEntityRepository _componentsRepository;

    public InMemoryEntitiesCollection(
        ulong[] entities,
        ulong contextId,
        IReadOnlyEntityRepository componentsRepository
    )
    {
        _entities = entities;
        _contextId = contextId;
        _componentsRepository = componentsRepository;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var entity in _entities)
        {
            yield return new Entity(new(entity, _contextId), _componentsRepository);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}