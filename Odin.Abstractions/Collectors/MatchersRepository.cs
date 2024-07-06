using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Entities;

namespace Odin.Abstractions.Collectors;

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


    public static ulong GetMatcherId<T>() where T : AComponentMatcher
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
    
    public static Func<ulong, Func<ulong, ulong, bool>, ComponentWrapper[], bool> GetFilter(ulong id)
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