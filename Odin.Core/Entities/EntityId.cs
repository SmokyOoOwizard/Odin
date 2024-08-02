namespace Odin.Core.Entities;

public readonly record struct EntityId(ulong Id, ulong ContextId)
{
    public ulong Id { get; } = Id;
    public ulong ContextId { get; } = ContextId;
}