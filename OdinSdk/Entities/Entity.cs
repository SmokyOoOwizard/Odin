using OdinSdk.Components;
using OdinSdk.Contexts;

namespace OdinSdk.Entities;

public struct Entity
{
    public EntityId Id { get; internal init; }

    internal IReadOnlyEntityRepository ColdComponents { get; init; }
    internal IEntityRepository HotComponents => EntityContexts.GetContext(Id.ContextId).Changes;
}