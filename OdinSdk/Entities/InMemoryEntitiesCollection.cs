using System.Collections;
using Odin.Abstractions.Entities;

namespace OdinSdk.Entities;

public class InMemoryEntitiesCollection : IEntitiesCollection
{
    private readonly Entity[] _entities;

    public InMemoryEntitiesCollection(Entity[] entities)
    {
        _entities = entities;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var entity in _entities)
        {
            yield return entity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}