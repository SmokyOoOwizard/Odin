using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Entities;

namespace Odin.Entities;

public class RawInMemoryEntitiesCollection : IRawEntitiesCollection
{
    private readonly IEntitiesCollection _entities;
    private readonly FilterComponentDelegate _filter;

    private readonly Queue<FilterComponentDelegate> _filters = new();

    public RawInMemoryEntitiesCollection(
        IEntitiesCollection entities,
        FilterComponentDelegate filter
    )
    {
        _entities = entities;
        _filter = filter;

        _filters.Enqueue(filter);
    }

    public IRawEntitiesCollection Filter<T>() where T : AComponentMatcher
    {
        _filters.Enqueue(_filter);

        return this;
    }

    public IEntitiesCollection Build()
    {
        return new InMemoryFiltredEntitiesCollection(_entities, _filters.ToArray());
    }
}