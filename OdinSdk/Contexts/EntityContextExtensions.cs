using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Contexts;
using Odin.Abstractions.Entities;
using OdinSdk.Entities.Repository.Impl;

namespace OdinSdk.Contexts;

public static class EntityContextExtensions
{
    public static IEntityCollector CreateCollector<T>(this AEntityContext context, string name)
        where T : AComponentMatcher
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            throw new Exception($"No repository found for context {context.Id}");

        return rep.CreateCollector<T>(name);
    }

    public static void DeleteCollector(this AEntityContext context, string name)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            throw new Exception($"No repository found for context {context.Id}");

        rep.DeleteCollector(name);
    }

    public static void DisableCollector(this AEntityContext context, string name)
    {
        throw new NotImplementedException();
    }

    public static Entity CreateEntity(this AEntityContext context)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            throw new Exception($"No repository found for context {context.Id}");

        var entity = rep.CreateEntity();
        var changes = new ReferenceInMemoryChangedComponentsRepository(context.Id);

        return new Entity(
            entity.Id,
            entity.Components,
            changes
        );
    }

    public static IEnumerable<Entity> GetEntities(this AEntityContext context)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);

        if (rep == default)
            yield break;
        
        var changes = new ReferenceInMemoryChangedComponentsRepository(context.Id);

        foreach (var entity in rep.GetEntities())
        {
            yield return new(
                entity.Id,
                entity.Components,
                changes
            );
        }
    }

    public static void Clear(this AEntityContext context)
    {
        var rep = EntityContextsRepository.GetRepository(context.Id);
        if (rep == default)
            return;

        rep.Clear();
    }
}