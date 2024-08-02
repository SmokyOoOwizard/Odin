using Odin.Abstractions.Contexts;
using Odin.Abstractions.Contexts.Utils;
using Odin.Entities.Repository.Impl;

namespace Odin.Contexts;

public class InMemoryEntityContext : AEntityContext
{
    public sealed override string Name { get; }
    public sealed override ulong Id { get; }

    public InMemoryEntityContext(string name)
    {
        Id = EntityContextUtils.ComputeId(name);
        Name = name;

        var rep = new InMemoryEntitiesRepository(Id);
        EntityContextsRepository.AddRepository(Id, rep);
    }

    public override void Dispose()
    {
        EntityContextsRepository.RemoveRepository(Id);
    }
}