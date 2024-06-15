﻿using OdinSdk.Entities;

namespace OdinSdk.Contexts;

public static class EntityContextExtensions
{
    public static Entity CreateEntity(this AEntityContext context)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            throw new Exception($"No repository found for context {context.Id}");

        var id = context.Local.Changes.CreateEntity();

        return new Entity
        {
            Id = new EntityId(id, context.Id),
            ColdComponents = rep,
        };
    }

    public static IEnumerable<Entity> GetEntities(this AEntityContext context)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            yield break;

        var changes = context.Local.Changes;
        foreach (var entityId in rep.GetEntities())
        {
            yield return new Entity
            {
                Id = new(entityId, context.Id),
                ColdComponents = rep,
            };
        }
    }
}