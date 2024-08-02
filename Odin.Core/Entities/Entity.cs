namespace Odin.Abstractions.Entities;

public readonly struct Entity
{
    public readonly EntityId Id;

    public readonly IReadOnlyEntityRepository Components;
    public readonly IEntityComponentsRepository Changes;

    public Entity(
        EntityId id,
        IReadOnlyEntityRepository components,
        IEntityComponentsRepository changes 
    )
    {
        Id = id;
        Components = components;
        Changes = changes;
    }
}