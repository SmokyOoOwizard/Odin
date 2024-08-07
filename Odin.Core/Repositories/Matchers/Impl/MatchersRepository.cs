﻿using Odin.Core.Abstractions.Matchers;
using Odin.Core.Matchers;

namespace Odin.Core.Repositories.Matchers.Impl;

public static class MatchersRepository
{
    private static IComponentMatcherRepository[] Matchers = Array.Empty<IComponentMatcherRepository>();
    
    static MatchersRepository()
    {
        Reload();
    }

    public static void Reload()
    {
        var type = typeof(IComponentMatcherRepository);
        Matchers = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => type.IsAssignableFrom(p) && p.IsClass)
                            .Select(c => (Activator.CreateInstance(c) as IComponentMatcherRepository)!)
                            .ToArray();
    }


    public static ulong GetMatcherId<T>() where T : AComponentMatcherBase
    {
        foreach (var matcher in Matchers)
        {
            if (matcher.HasMatcher<T>())
            {
                return matcher.GetMatcherId<T>();
            }
        }

        throw new Exception($"Matcher with type {typeof(T).Name} not found");
    }
    
    public static string GetMatcherJson(ulong id)
    {
        foreach (var matcher in Matchers)
        {
            if (matcher.HasMatcher(id))
            {
                return matcher.GetMatcherJson(id);
            }
        }

        throw new Exception($"Matcher with id {id} not found");
    }
    
    public static FilterComponentDelegate GetFilter(ulong id)
    {
        foreach (var matcher in Matchers)
        {
            if (matcher.HasMatcher(id))
            {
                return matcher.GetFilter(id);
            }
        }

        throw new Exception($"Matcher with id {id} not found");
    }
}