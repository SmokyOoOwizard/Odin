using Odin.Abstractions.Contexts;
using OdinSdk.Components.Impl;

namespace OdinSdk.Contexts;

public static class EntityContexts
{
    private static readonly Dictionary<ulong, EntityContextLocal> Contexts = new();

    public static void Clear()
    {
        lock (Contexts)
        {
            Contexts.Clear();
        }
    }

    public static void Save()
    {
        lock (Contexts)
        {
            var val = Contexts.ToArray();
            Contexts.Clear();

            foreach (var (key, contextLocal) in val)
            {
                var rep = EntityContextsRepository.GetRepository(key);

                rep?.Apply(contextLocal.Changes.GetEntitiesWithComponents());
            }
        }
    }

    internal static EntityContextLocal GetContext(ulong id)
    {
        lock (Contexts)
        {
            if (!Contexts.TryGetValue(id, out var context))
            {
                context = new()
                {
                    Changes = new InMemoryEntitiesChangedComponents()
                };

                Contexts[id] = context;
            }

            return context;
        }
    }
}