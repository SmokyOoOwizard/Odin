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
        if (changes.Get<T>(entity.Id.Id, out var hotComponent))
            return hotComponent;

        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);

        T? coldComponent = default;
        rep?.Get(entity.Id.Id, out coldComponent);

        return coldComponent;
    }

    public static T? GetOld<T>(this Entity entity) where T : IComponent
    {
        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);

        T? coldComponent = default;
        rep?.GetOld(entity.Id.Id, out coldComponent);

        return coldComponent;
    }

    public static void Replace<T>(this Entity entity, T component) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Replace(entity.Id.Id, component);
    }

    public static void Remove<T>(this Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Remove<T>(entity.Id.Id);
    }

    public static bool Has<T>(this Entity entity) where T : IComponent
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        if (changes.Get<T>(entity.Id.Id, out var hotComponent))
            return hotComponent != null;

        var rep = EntityContextsRepository.GetRepository(entity.Id.ContextId);
        if (rep == default)
            return false;

        var hasComponent = rep.Get<T>(entity.Id.Id, out var coldComponent);

        return hasComponent && coldComponent != null;
    }

    public static void Destroy(this Entity entity)
    {
        var changes = EntityContexts.GetContext(entity.Id.ContextId).Changes;
        changes.Replace(entity.Id.Id, default(DestroyedComponent));
    }
}