using Odin.Abstractions.Collectors.Matcher;

namespace Odin.Abstractions.Entities;

public interface IRawEntitiesCollection
{
    IRawEntitiesCollection Filter<T>() where T : AComponentMatcher;
    IEntitiesCollection Build();
}