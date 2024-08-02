using System.Collections;
using Odin.Core.Matchers;
using Odin.Core.Repositories.Matchers.Impl;

namespace Odin.Core.Entities.Collections.Impl;

public class InMemoryFiltredEntitiesCollection : IEntitiesCollection
{
    private readonly IEntitiesCollection _entities;
    private readonly FilterComponentDelegate[] _filters;

    public InMemoryFiltredEntitiesCollection(
        IEntitiesCollection entities,
        FilterComponentDelegate[] filters
    )
    {
        _entities = entities;
        _filters = filters;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var entity in _entities)
        {
            var allFiltersMatch = true;
            foreach (var filter in _filters)
            {
                if (!filter(entity))
                {
                    allFiltersMatch = false;
                    break;
                }
            }

            if (!allFiltersMatch)
                continue;

            yield return entity;
        }
    }

    public IRawEntitiesCollection Filter<T>() where T : AComponentMatcher
    {
        var matcher = MatchersRepository.GetMatcherId<T>();
        var filter = MatchersRepository.GetFilter(matcher);

        return new RawInMemoryEntitiesCollection(this, filter);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}