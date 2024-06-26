using Odin.Abstractions.Entities;

namespace Odin.Abstractions.Contexts;

public static class EntityContextsRepository
{
    private static readonly Dictionary<ulong, IEntityRepository> Repositories = new();

    public static IEntityRepository? GetRepository(ulong id)
    {
        lock (Repositories)
        {
            if (!Repositories.ContainsKey(id))
                return null;

            return Repositories[id];
        }
    }

    public static void AddRepository(ulong id, IEntityRepository repository)
    {
        lock (Repositories)
        {
            if (Repositories.ContainsKey(id))
                throw new InvalidOperationException("Repository already exists");

            Repositories[id] = repository;
        }
    }

    public static void RemoveRepository(ulong id)
    {
        lock (Repositories)
        {
            Repositories.Remove(id);
        }
    }
}