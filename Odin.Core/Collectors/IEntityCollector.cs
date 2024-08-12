using Odin.Core.Entities.Collections;

namespace Odin.Core.Collectors;

public interface IEntityCollector : IDisposable
{
    string Name { get; }
    
    ulong MatcherId { get; }

    void SetAutoClear(bool enable);
    IEntitiesCollection GetEntities();
}