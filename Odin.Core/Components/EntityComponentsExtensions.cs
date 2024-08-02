using Odin.Core.Abstractions.Components;
using Odin.Core.Entities;

namespace Odin.Core.Components;

public static class EntityComponentsExtensions
{
    public static T? Get<T>(this Entity entity) where T : IComponent
    {
        var changes = entity.Changes;
        // TODO WTF?
        if (changes == default)
            return default;

        if (changes.Get<T>(entity, out var hotComponent))
            return hotComponent;

        var rep = entity.Components;

        rep.Get(entity, out T? coldComponent);

        return coldComponent;
    }

    public static T? GetOld<T>(this Entity entity) where T : IComponent
    {
        var rep = entity.Components;

        rep.GetOld(entity, out T? coldComponent);

        return coldComponent;
    }

    public static void Replace<T>(this Entity entity, T component) where T : IComponent
    {
        var changes = entity.Changes;
        changes?.Replace(entity, component);
    }

    public static void Remove<T>(this Entity entity) where T : IComponent
    {
        var changes = entity.Changes;
        changes?.Remove<T>(entity);
    }

    public static bool Has<T>(this Entity entity) where T : IComponent
    {
        var changes = entity.Changes;
        if (changes != null)
        {
            if (changes.Get<T>(entity, out var hotComponent))
                return hotComponent != null;
        }

        var rep = entity.Components;

        var hasComponent = rep.Get<T>(entity, out var coldComponent);

        return hasComponent && coldComponent != null;
    }

    public static void Destroy(this Entity entity)
    {
        var changes = entity.Changes;
        changes?.Replace(entity, default(DestroyedComponent));
    }
}