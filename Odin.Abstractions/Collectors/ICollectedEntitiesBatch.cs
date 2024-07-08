namespace Odin.Abstractions.Collectors;

public interface ICollectedEntitiesBatch
{
    IEnumerable<ulong> GetEntities();
}