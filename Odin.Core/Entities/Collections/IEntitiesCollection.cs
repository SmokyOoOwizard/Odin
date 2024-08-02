using Odin.Abstractions.Collectors.Matcher;

namespace Odin.Abstractions.Entities;

public interface IEntitiesCollection : IEnumerable<Entity>
{
    IRawEntitiesCollection Filter<T>() where T : AComponentMatcher;
}