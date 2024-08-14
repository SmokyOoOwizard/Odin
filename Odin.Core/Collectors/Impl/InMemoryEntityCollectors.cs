using System.Collections.Immutable;
using Odin.Core.Entities;

namespace Odin.Core.Collectors.Impl;

internal static class InMemoryEntityCollectors
{
    // key - contextId, value - Collectors
    private static readonly Dictionary<ulong, ContextInMemoryEntityCollectors> Collectors = new();

    public static void AddEntity(ulong matcherId, EntityId entityId)
    {
        var contextId = entityId.ContextId;

        if (!Collectors.TryGetValue(contextId, out var collectors))
            return;

        collectors.TryAddEntity(matcherId, entityId);
    }

    public static void AddCollector(ulong contextId, string name, ulong matcherId)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
        {
            Collectors[contextId] = collectors = new ContextInMemoryEntityCollectors();
        }

        collectors.TryAddCollector(name, matcherId);
    }

    public static ImmutableQueue<EntityId> GetCollector(ulong contextId, string name, long generation)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
            return ImmutableQueue<EntityId>.Empty;

        return collectors.GetEntities(name, generation);
    }

    public static long GetCollectorGeneration(ulong contextId, string name)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
            return 0;

        if (!collectors.TryGetCollectorGeneration(name, out var generation))
            return 0;

        return generation;
    }

    public static long IncreaseCollectorGeneration(ulong contextId, string name)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
            return 0;

        if (!collectors.IncreaseCollectorGeneration(name, out var generation))
            return 0;

        return generation;
    }

    public static void ClearCollectorGeneration(ulong contextId, string name, long generation)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
            return;

        collectors.ClearCollectorGeneration(name, generation);
    }

    public static void DeleteCollector(ulong contextId, string name)
    {
        if (!Collectors.TryGetValue(contextId, out var collectors))
            return;

        collectors.DeleteCollector(name);
    }
}