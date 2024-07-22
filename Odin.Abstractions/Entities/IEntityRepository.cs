using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;

namespace Odin.Abstractions.Entities;

public interface IEntityRepository : IEntityComponentsRepository
{
    IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher;
    void DeleteCollector(string name);

    Entity CreateEntity();
    void DestroyEntity(Entity entity);
}