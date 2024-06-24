using Odin.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Entities;

public readonly struct Entity
{
    public EntityId Id { get; internal init; }

    internal IReadOnlyEntityRepository ColdComponents { get; init; }
    internal IEntityComponentsRepository HotComponents => EntityContexts.GetContext(Id.ContextId).Changes;
}