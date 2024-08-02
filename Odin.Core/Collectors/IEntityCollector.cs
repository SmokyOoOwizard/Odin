using Odin.Abstractions.Entities;

namespace Odin.Abstractions.Collectors;

public interface IEntityCollector
{
    string Name { get; }
    
    ulong MatcherId { get; }

    IEntitiesCollection GetEntities();
}