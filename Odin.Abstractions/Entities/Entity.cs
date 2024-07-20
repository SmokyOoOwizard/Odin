namespace Odin.Abstractions.Entities;

public readonly struct Entity
{
    public readonly EntityId Id;

    public readonly IReadOnlyEntityRepository Components;

    public Entity(
        EntityId id,
        IReadOnlyEntityRepository components
    )
    {
        Id = id;
        Components = components;
    }
}