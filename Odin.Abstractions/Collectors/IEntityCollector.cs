namespace Odin.Abstractions.Collectors;

public interface IEntityCollector
{
    string Name { get; }
    
    ulong MatcherId { get; }

    ICollectedEntitiesBatch GetBatch();
}