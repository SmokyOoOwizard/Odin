﻿using System.Collections;
using Odin.Core.Matchers;
using Odin.Core.Repositories.Matchers.Impl;

namespace Odin.Core.Entities.Collections.Impl;

public class InMemoryEntitiesCollection : IEntitiesCollection
{
    private readonly Entity[] _entities;
    
    public InMemoryEntitiesCollection(params Entity[] entities)
    {
        _entities = entities;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var entity in _entities)
        {
            yield return entity;
        }
    }

    public IRawEntitiesCollection Filter<T>() where T : AComponentMatcher
    {
        var matcher = MatchersRepository.GetMatcherId<T>();
        var filter = MatchersRepository.GetFilter(matcher);

        return new RawInMemoryEntitiesCollection(this, filter);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}