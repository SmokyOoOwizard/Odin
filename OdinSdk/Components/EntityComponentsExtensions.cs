using Odin.Abstractions.Components;
using Odin.Abstractions.Contexts;
using Odin.Abstractions.Entities;
using OdinSdk.Contexts;

namespace OdinSdk.Components;

public static class EntityComponentsExtensions
{
    public static T? Get<T>(this Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        if (changes.Get<T>(entity, out var hotComponent))
            return hotComponent;

        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);

        T? coldComponent = default;
        rep?.Get(entity, out coldComponent);

        return coldComponent;
    }

    public static T? GetOld<T>(this Entity entity) where T : IComponent
    {
        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);

        T? coldComponent = default;
        rep?.GetOld(entity, out coldComponent);

        return coldComponent;
    }

    public static void Replace<T>(this Entity entity, T component) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Replace(entity, component);
    }

    public static void Remove<T>(this Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Remove<T>(entity);
    }

    public static bool Has<T>(this Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        if (changes.Get<T>(entity, out var hotComponent))
            return hotComponent != null;

        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);
        if (rep == default)
            return false;

        var hasComponent = rep.Get<T>(entity, out var coldComponent);

        return hasComponent && coldComponent != null;
    }

    public static void Destroy(this Entity entity)
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Replace(entity, default(DestroyedComponent));
    }
}