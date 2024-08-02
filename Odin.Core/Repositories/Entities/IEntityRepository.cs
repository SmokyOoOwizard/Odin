using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Entities.Indexes;

namespace Odin.Abstractions.Entities;

public interface IEntityRepository : IEntityComponentsRepository
{
    IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher;
    void DeleteCollector(string name);

    IIndexModule GetIndex(ulong componentId);

    Entity CreateEntity();
    void DestroyEntity(Entity entity);
}