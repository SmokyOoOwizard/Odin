using OdinSdk.Components;
using OdinSdk.Components.Impl;

namespace OdinSdk.Contexts;

public class InMemoryEntityContext : AEntityContext
{
    public override string Name { get; }
    public override ulong Id => (ulong)Name.GetHashCode();

    private IEntityRepository _repository;

    public InMemoryEntityContext(string name)
    {
        Name = name;

        IEntityRepository rep = new InMemoryEntitiesChangedComponents();
        EntityContextsRepository.AddRepository(Id, rep);

        _repository = rep!;
    }

    public override void Dispose()
    {
        EntityContextsRepository.RemoveRepository(Id);
    }
}