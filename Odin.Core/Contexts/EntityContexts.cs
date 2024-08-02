using Odin.Core.Repositories.Contexts;
using Odin.Core.Repositories.Entities.Impl;

namespace Odin.Core.Contexts;

public static class EntityContexts
{
    private static AsyncLocal<Dictionary<ulong, EntityContextLocal>> Contexts = new();

    public static void Clear()
    {
        lock (Contexts)
        {
            Contexts = new()
            {
                Value = new()
            };
        }
    }

    public static void Save()
    {
        lock (Contexts)
        {
            var val = Contexts.Value?.ToArray();
            if (val == null)
                return;

            Contexts.Value?.Clear();

            foreach (var (key, contextLocal) in val)
            {
                var rep = EntityContextsRepository.GetRepository(key);

                rep?.Apply(contextLocal.Changes.GetEntities());
            }
        }
    }

    internal static EntityContextLocal GetContext(ulong id)
    {
        lock (Contexts)
        {
            var val = Contexts.Value;
            if (val == null)
            {
                Contexts.Value = new();
                val = Contexts.Value;
            }

            if (!val.TryGetValue(id, out var context))
            {
                context = new()
                {
                    Changes = new InMemoryEntitiesChangedComponentsRepository(id)
                };

                val[id] = context;
            }

            return context;
        }
    }
}