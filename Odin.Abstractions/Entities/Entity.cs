namespace Odin.Abstractions.Entities;

public readonly struct Entity
{
    public readonly EntityId Id;

    public Entity(EntityId id)
    {
        Id = id;
    }
}