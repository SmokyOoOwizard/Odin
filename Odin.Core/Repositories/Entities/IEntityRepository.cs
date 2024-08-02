using Odin.Core.Abstractions.Matchers;
using Odin.Core.Collectors;
using Odin.Core.Entities;
using Odin.Core.Indexes;

namespace Odin.Core.Repositories.Entities;

public interface IEntityRepository : IEntityComponentsRepository
{
    IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher;
    void DeleteCollector(string name);

    IIndexModule GetIndex(ulong componentId);

    Entity CreateEntity();
    void DestroyEntity(Entity entity);
}