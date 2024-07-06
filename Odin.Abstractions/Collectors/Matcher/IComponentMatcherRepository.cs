using Odin.Abstractions.Entities;

namespace Odin.Abstractions.Collectors.Matcher;

public interface IComponentMatcherRepository
{
    bool HasMatcher(ulong id);
    bool HasMatcher<T>() where T : AComponentMatcher;
    string GetMatcherJson(ulong matcherId);
    
    ulong GetMatcherId<T>() where T : AComponentMatcher;
    Func<ulong, Func<ulong, ulong, bool>, ComponentWrapper[], bool> GetFilter(ulong matcherId);
}