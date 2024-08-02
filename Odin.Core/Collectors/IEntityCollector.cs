using Odin.Core.Entities.Collections;

namespace Odin.Core.Collectors;

public interface IEntityCollector
{
    string Name { get; }
    
    ulong MatcherId { get; }

    IEntitiesCollection GetEntities();
}