using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;

namespace Odin.Abstractions.Entities;

public interface IEntityRepository : IEntityComponentsRepository
{
    IEntityCollector CreateCollector<T>(string name) where T : AComponentMatcher;
    void DeleteCollector(string name);
    
    ulong CreateEntity();
    void DestroyEntity(ulong entityId);
}