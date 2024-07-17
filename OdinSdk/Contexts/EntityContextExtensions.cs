using Odin.Abstractions.Collectors;
using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Contexts;
using OdinSdk.Entities;

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

        var id = rep.CreateEntity();

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

        foreach (var entityId in rep.GetEntities())
        {
            yield return new Entity
            {
                Id = new(entityId, context.Id),
                ColdComponents = rep,
            };
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