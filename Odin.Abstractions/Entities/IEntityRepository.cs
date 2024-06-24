namespace Odin.Abstractions.Entities;

public interface IEntityRepository : IEntityComponentsRepository
{
    ulong CreateEntity();
    void DestroyEntity(ulong entityId);
}