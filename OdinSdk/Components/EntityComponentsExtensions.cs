using OdinSdk.Entities;

namespace OdinSdk.Components;

public static class EntityComponentsExtensions
{
    public static T? Get<T>(this Entity entity) where T : IComponent
    {
        if (entity.HotComponents.Get<T>(entity.Id.Id, out var hotComponent))
            return hotComponent;

        entity.ColdComponents.Get<T>(entity.Id.Id, out var coldComponent);

        return coldComponent;
    }

    public static T? GetOld<T>(this Entity entity) where T : IComponent
    {
        entity.ColdComponents.GetOld<T>(entity.Id.Id, out var oldComponent);

        return oldComponent;
    }

    public static void Replace<T>(this Entity entity, T component) where T : IComponent
    {
        entity.HotComponents.Replace(entity.Id.Id, component);
    }

    public static void Remove<T>(this Entity entity) where T : IComponent
    {
        entity.HotComponents.Remove<T>(entity.Id.Id);
    }

    public static bool Has<T>(this Entity entity) where T : IComponent
    {
        if (entity.HotComponents.Get<T>(entity.Id.Id, out var hotComponent))
            return hotComponent != null;

        var hasComponent = entity.ColdComponents.Get<T>(entity.Id.Id, out var coldComponent);

        return hasComponent && coldComponent != null;
    }

    public static void Destroy(this Entity entity)
    {
        entity.HotComponents.DestroyEntity(entity.Id.Id);
    }
}