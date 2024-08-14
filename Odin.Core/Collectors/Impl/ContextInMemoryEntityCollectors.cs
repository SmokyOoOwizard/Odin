using System.Collections.Immutable;
using Odin.Core.Entities;

namespace Odin.Core.Collectors.Impl;

internal class ContextInMemoryEntityCollectors
{
    // key - matcherId
    private readonly Dictionary<ulong, List<InMemoryEntityCollectorGenerations>> _matcherToCollectors = new();

    // key - name
    private readonly Dictionary<string, InMemoryEntityCollectorGenerations> _nameToCollector = new();


    public bool TryAddEntity(ulong matcherId, EntityId entityId)
    {
        if (!_matcherToCollectors.TryGetValue(matcherId, out var collectors))
            return false;

        foreach (var collector in collectors)
        {
            collector.AddEntity(entityId);
        }

        return true;
    }

    public bool TryAddCollector(string name, ulong matcherId)
    {
        if (_nameToCollector.ContainsKey(name))
            return false;

        var collector = new InMemoryEntityCollectorGenerations(matcherId);
        _nameToCollector[name] = collector;

        if (!_matcherToCollectors.TryGetValue(matcherId, out var collectors))
        {
            collectors = new List<InMemoryEntityCollectorGenerations>();
            _matcherToCollectors[matcherId] = collectors;
        }

        collectors.Add(_nameToCollector[name]);

        return true;
    }

    public bool TryGetCollectorGeneration(string name, out long generation)
    {
        generation = 0;
        if (!_nameToCollector.TryGetValue(name, out var collector))
            return false;

        generation = collector.Generation;

        return true;
    }

    public bool IncreaseCollectorGeneration(string name, out long generation)
    {
        generation = 0;

        if (!_nameToCollector.TryGetValue(name, out var collector))
            return false;

        generation = collector.IncreaseCollectorGeneration();

        return true;
    }

    public void ClearCollectorGeneration(string name, long generation)
    {
        if (!_nameToCollector.TryGetValue(name, out var collector))
            return;

        collector.ClearCollectorGeneration(generation);
    }

    public void DeleteCollector(string name)
    {
        if (!_nameToCollector.TryGetValue(name, out var collector))
            return;

        _nameToCollector.Remove(name);
        _matcherToCollectors.Remove(collector.MatcherId);
    }

    public ImmutableQueue<EntityId> GetEntities(string name, long generation)
    {
        if (!_nameToCollector.TryGetValue(name, out var collector))
            return ImmutableQueue<EntityId>.Empty;

        return collector.GetEntities(generation);
    }
}