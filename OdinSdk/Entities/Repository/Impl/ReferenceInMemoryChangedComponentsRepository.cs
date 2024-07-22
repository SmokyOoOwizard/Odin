using Odin.Abstractions.Components;
using Odin.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Entities.Repository.Impl;

internal class ReferenceInMemoryChangedComponentsRepository : IEntityComponentsRepository
{
    private readonly ulong _contextId;

    public ReferenceInMemoryChangedComponentsRepository(ulong contextId)
    {
        _contextId = contextId;
    }

    public bool Has(Entity entity, ulong componentId)
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.Has(entity, componentId);
    }

    public bool WasRemoved(Entity entity, ulong componentId)
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.WasRemoved(entity, componentId);
    }

    public bool Get<T>(Entity entity, out T? component) where T : IComponent
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.Get(entity, out component);
    }

    public bool GetOld<T>(Entity entity, out T? component) where T : IComponent
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.GetOld(entity, out component);
    }

    public ComponentWrapper[] GetComponents(Entity entity)
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.GetComponents(entity);
    }

    public IEntitiesCollection GetEntities()
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        return changes.GetEntities();
    }

    public void Replace<T>(Entity entity, T? component) where T : IComponent
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        changes.Replace(entity, component);
    }

    public void Remove<T>(Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        changes.Remove<T>(entity);
    }

    public void Apply(IEntitiesCollection entities)
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        changes.Apply(entities);
    }

    public void Clear()
    {
        var changes = EntityContexts.GetContext(_contextId).Changes;

        changes.Clear();
    }
}