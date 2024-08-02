using Odin.Core.Matchers;

namespace Odin.Core.Entities.Collections;

public interface IRawEntitiesCollection
{
    IRawEntitiesCollection Filter<T>() where T : AComponentMatcher;
    IEntitiesCollection Build();
}