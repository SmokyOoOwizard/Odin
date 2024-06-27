namespace Odin.Abstractions.Collectors;

public interface ICollectedEntitiesBatch : IDisposable
{
    IEnumerable<ulong> GetEntities();
    
    void DontDeleteEntity(ulong entity);
}