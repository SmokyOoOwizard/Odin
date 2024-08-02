using Odin.Core.Matchers;

namespace Odin.Core.Entities.Collections;

public interface IEntitiesCollection : IEnumerable<Entity>
{
    IRawEntitiesCollection Filter<T>() where T : AComponentMatcher;
}