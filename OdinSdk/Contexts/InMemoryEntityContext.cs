using Odin.Abstractions.Contexts;
using Odin.Abstractions.Contexts.Utils;
using OdinSdk.Components;
using OdinSdk.Components.Impl;

namespace OdinSdk.Contexts;

public class InMemoryEntityContext : AEntityContext
{
    public sealed override string Name { get; }
    public sealed override ulong Id { get; }

    public InMemoryEntityContext(string name)
    {
        Id = EntityContextUtils.ComputeId(name);
        Name = name;

        IEntityRepository rep = new InMemoryEntitiesChangedComponents();
        EntityContextsRepository.AddRepository(Id, rep);
    }

    public override void Dispose()
    {
        EntityContextsRepository.RemoveRepository(Id);
    }
}