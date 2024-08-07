﻿using Odin.Core.Abstractions.Components;
using Odin.Core.Components;
using Odin.Core.Contexts;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;

namespace Odin.Core.Repositories.Entities.Impl;

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

    public IEntitiesCollection GetEntities(IEntityComponentsRepository? changes = default)
    {
        var rep = EntityContexts.GetContext(_contextId).Changes;

        return rep.GetEntities(changes);
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